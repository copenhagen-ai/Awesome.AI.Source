using Awesome.AI.Common;
using Awesome.AI.Core.Internals;
using Awesome.AI.Core.Spaces;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class m_NoiseGenerator : IMechanics
    {
        public MECHANICS type { get; set; }
        public MechSymbolicOut ms { get; set; }
        public MechParams mp { get; set; }
        public MechHelper mh { get; set; }

        private TheMind mind;
        private m_NoiseGenerator() { }
        public m_NoiseGenerator(TheMind mind, MECHANICS type, PROPS mprops)
        {
            this.mind = mind;
            this.type = type;

            this.ms = new MechSymbolicOut() { };

            this.mh = new MechHelper() { };

            this.mp = new MechParams() { };

            this.mp.mprops = new ModProperties(mind, mprops);
            this.mp.eprops = new BaseProperties(ms);

            mp.posxy = CONST.STARTXY;

            mp.peek_vv_out_high = -1000.0d;
            mp.peek_vv_out_low = 1000.0d;
            mp.vv_out_high = -1000.0d;
            mp.vv_out_low = 1000.0d;
            mp.dv_out_high = -1000.0d;
            mp.dv_out_low = 1000.0d;
            mp.posx_high = -1000.0d;
            mp.posx_low = 1000.0d;
        }

        public double PosXY()
        {
            double meter = mh.PosXY(mind, mp);

            return meter;
        }

        public void Peek(UNIT curr)
        {
            if (curr.IsNull())
                throw new Exception("NoiseGenerator, Momentum");

            if (curr.IsIDLE())
                throw new Exception("NoiseGenerator, Momentum");

            Calc(curr, true, -1);

            double adj = mp.peek_vv_out_low == mp.peek_vv_out_high ? 0.1d : 0.0d;

            mp.peek_vv_norm = mind.calc.Normalize(mp.peek_vv_curr, mp.peek_vv_out_low - adj, mp.peek_vv_out_high, 0.0d, 100.0d);
        }

        public double Dir(string ax)
        {
            double dv = ms.dv_sym_100;

            double dir = dv > 50.0d ? 1.0d : -1.0d;

            return dir;
        }

        public double Mean()
        {
            double low = ms.dv_sym_low;
            double high = ms.dv_sym_high;

            double res = (low + high) / 2.0d;

            return res;
        }

        public void Calc(UNIT curr, bool peek, int cycles)
        {
            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m

            //momentum: p = m * v

            //DeltaTime();

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    mp.damp = 1.0d;
                    mp.inertia_lim = 0.15d;
                    mp.m1 = 1500.0d;
                    mp.m2 = 1500.0d;
                    mp.dt = 1.0d;
                    mp.Fmax = 3000.0d;
                    break;
                case MECHANICS.BALLONHILL_LOW:
                    mp.damp = 1.0d;
                    mp.inertia_lim = 0.15d;
                    mp.a = 0.1d;            // Parabola coefficient (hill steepness)
                    mp.g = CONST.GRAVITY;   // Gravity (m/s^2)
                    mp.m1 = 0.35d;          // Ball mass (kg)
                    mp.dt = 1.0d;
                    break;
                default: throw new Exception("m_NoiseGenerator, Calc");
            }

            mp.f_sta = ApplyStatic(mp, type);
            mp.f_dyn = ApplyDynamic(mp, type, curr);
            mp.f_friction = Friction3(mp, type);

            double f = mp.f_sta + mp.f_dyn + mp.f_friction;

            if (peek)
            {
                mp.peek_vv_curr = mp.vv_prev + (f * mp.dt) / (mp.m1 + mp.m2);
            }
            else
            {
                mp.dv_prev = mp.dv_curr;
                mp.vv_prev = mp.vv_curr;
                mp.mom_prev = mp.mom_curr;
                mp.acc_prev = mp.acc_curr;
                mp.ke_prev = mp.ke_curr;
                mp.fnet_prev = mp.fnet_curr;

                mp.dv_curr = (f * mp.dt) / (mp.m1 + mp.m2);
                mp.vv_curr = mp.vv_prev + mp.dv_curr;
                mp.mom_curr = mp.vv_curr * (mp.m1 + mp.m2);
                mp.acc_curr = mp.dv_curr / mp.dt;
                mp.ke_curr = 0.5d * (mp.m1 + mp.m2) * (mp.vv_curr * mp.vv_curr);
                mp.fnet_curr = f;

                mp.pos_x += mp.vv_curr * mp.dt;
            }

            if (double.IsNaN(mp.dv_prev)) throw new Exception("NAN");
            if (double.IsNaN(mp.dv_curr)) throw new Exception("NAN");
            if (double.IsNaN(mp.vv_prev)) throw new Exception("NAN");
            if (double.IsNaN(mp.vv_curr)) throw new Exception("NAN");
        }

        public double Logic(UNIT curr)
        {
            /*
             * logic optional
             * */

            double res = 0;
            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    double _v0 = 0.01d * curr.Variable;
                    double mod1 = 1.8d;
                    double mod2 = 0.8d;
                    double _v1 = Math.Tanh(mod1 * _v0);
                    double _v2 = (mod2 * _v0);

                    double motor = mp.vv_curr < 0.0 ? _v1 : _v2;
                    
                    res = motor;
                    break;
                case MECHANICS.BALLONHILL_LOW:
                    double windAccel = mind.unit_current.Variable * 0.2d; // constant wind acceleration
                    res = windAccel;
                    break;
                default: throw new Exception("m_NoiseGenerator, Friction");
            }

            return res;
        }

        public double Friction1(MechParams mp, MECHANICS type)
        {
            if (mind.goodbye)
                return 0.0d;

            double total_mass = 0.0d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW: total_mass = mp.m1; break;
                case MECHANICS.BALLONHILL_LOW: total_mass = mp.m1; break;
                default: throw new Exception("m_NoiseGenerator, Friction");
            }

            //double sign = -Math.Sign(mp.f_sta + mp.f_dyn);
            //double sign = -Math.Sign(mp.dv_curr);

            double N = total_mass * CONST.GRAVITY;
            double u = mh.Friction(mind) * 0.001d;
            double sign = mp.vv_curr.Sign();
            double f_friction = u * N * -sign;

            return f_friction;
        }

        public double Friction2(MechParams mp, MECHANICS type)
        {
            if (mind.goodbye)
                return 0.0d;

            double fric = 0.0d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW: fric = -(0.1 * mp.vv_curr); break;
                case MECHANICS.BALLONHILL_LOW: fric = -(0.1 * mp.vv_curr); break;
                default: throw new Exception("m_NoiseGenerator, Friction");
            }

            double f_friction = fric;

            return f_friction;
        }

        public double Friction3(MechParams mp, MECHANICS type)
        {
            if (mind.goodbye)
                return 0.0d;

            double k = -500.0d;
            double u = mh.Friction(mind);
            double sign = mp.vv_curr.Sign();

            double f_friction = k * u * sign;

            return f_friction;
        }

        public double ApplyStatic(MechParams mp, MECHANICS type)
        {
            if (mp.acc_curr == 0.0d)
                mp.acc_curr = 1.0d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    // force left
                    double motor = CONST.BASE_SCALE;
                    double Fapplied = motor * mp.Fmax;
                    return -(mp.damp * Fapplied);
                case MECHANICS.BALLONHILL_LOW:
                    //slope force
                    double slope = 2 * mp.a * mp.pos_x; // Slope dy/dx
                    double sinTheta = slope / Math.Sqrt(1 + slope * slope);
                    double Fgravity = (mp.m1 * mp.g) * sinTheta;
                    return -(mp.damp * Fgravity);
                default: throw new Exception("m_NoiseGenerator, ApplyStatic");
            }
        }

        public double ApplyDynamic(MechParams mp, MECHANICS type, UNIT curr)
        {
            if (mind.goodbye)
                return 0.0d;

            if (mp.acc_curr == 0.0d)
                mp.acc_curr = 1.0d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    //force right
                    double motor = Logic(curr);
                    double Fapplied = motor * mp.Fmax;
                    return mp.damp * Fapplied;
                case MECHANICS.BALLONHILL_LOW:
                    //wind force
                    double windAccel = Logic(curr);
                    double windforce = mp.m1 * windAccel;
                    return mp.damp * windforce;
                default: throw new Exception("m_NoiseGenerator, ApplyDynamic");
            }
        }

        public void Calculate(PATTERN match, int cycles)
        {
            PATTERN pattern = mind.bot.pattern;

            if (pattern != match)
                return;

            mp.pattern_curr = pattern;

            if (cycles == 1)
                mh.ResetNoise(mind, mp);

            Calc(mind.unit_current, false, cycles);

            mh.ExtremesNoise(mp);
            mh.NormalizeNoise(mind, mp);
            ms.Convert(mp, this.type);
        }
    }
}

