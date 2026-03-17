using Awesome.AI.Common;
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
        public m_NoiseGenerator(TheMind mind, MECHANICS type)
        {
            this.mind = mind;
            this.type = type;

            this.ms = new MechSymbolicOut() { };

            this.mh = new MechHelper() { };

            this.mp = new MechParams() { };

            mp.posxy = CONST.STARTXY;

            mp.vv_out_high_peek = -1000.0d;
            mp.vv_out_low_peek = 1000.0d;
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

            double adj = mp.vv_out_low_peek == mp.vv_out_high_peek ? 0.1d : 0.0d;

            mp.peek_norm = mind.calc.Normalize(mp.peek_velocity, mp.vv_out_low_peek - adj, mp.vv_out_high_peek, 0.0d, 100.0d);
        }

        public double Dir()
        {
            throw new NotImplementedException("m_NoiseGenerator, Dir");
        }

        public void Calc(UNIT curr, bool peek, int cycles)
        {
            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
                       
            //momentum: p = m * v

            DeltaTime();

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    mp.damp = 0.2d;
                    mp.inertia_lim = 0.15d;
                    mp.m1 = 500.0d;
                    mp.m2 = 500.0d;
                    break;
                case MECHANICS.BALLONHILL_LOW:
                    mp.damp = 1.0d;
                    mp.inertia_lim = 0.15d;
                    mp.a = 0.1d;            // Parabola coefficient (hill steepness)
                    mp.g = CONST.GRAVITY;   // Gravity (m/s^2)
                    mp.m1 = 0.35d;          // Ball mass (kg)
                    break;
                default: throw new Exception("m_NoiseGenerator, Calc");
            }

            double fnet_prev = mp.fnet_curr;
            double acc = fnet_prev / (mp.m1 + mp.m2);

            mp.f_sta = ApplyStatic(acc, mp, this.type);
            mp.f_dyn = ApplyDynamic(acc, mp, this.type, curr);
            mp.f_friction = Friction(mp, type);

            mp.fnet_curr = mp.f_sta + mp.f_dyn + mp.f_friction;
            
            double dv_curr = acc * mp.dt;

            if (peek) {
                mp.peek_velocity = mp.vv_prev + dv_curr;
            } else {
                mp.dv_prev = mp.dv_curr;
                mp.dv_curr = dv_curr;
                mp.vv_prev = mp.vv_curr;
                mp.vv_curr += mp.dv_curr;

                //mp.velocity += a_system * mp.dt;
                mp.pos_x += mp.vv_curr * mp.dt;
            }

            if (double.IsNaN(mp.dv_prev)) throw new Exception("NAN");
            if (double.IsNaN(mp.dv_curr)) throw new Exception("NAN");
            if (double.IsNaN(mp.vv_prev)) throw new Exception("NAN");
            if (double.IsNaN(mp.vv_curr)) throw new Exception("NAN");
        }

        public void DeltaTime()
        {
            double delta = mind.mech_high.ms.dv_sym_100;

            double sec = 0.1d;
            double mod = delta > 0.0d ? delta / 100.0d : 1.0d;

            mp.dt = sec * mod;
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
                    double _v = curr.Variable;
                    double motor = mp.vv_curr < 0.0 ? (_v) / 100.0d : (_v * 0.5d) / 100.0d;
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

        public double Friction(MechParams mp, MECHANICS type)
        {
            if (mind.goodbye)
                return 0.0d;

            double total_mass = 0.0d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW: total_mass = mp.m1 + mp.m2; break;
                case MECHANICS.BALLONHILL_LOW: total_mass = mp.m1; break;
                default: throw new Exception("m_NoiseGenerator, Friction");
            }

            double u = mh.Friction(mind) * 0.05d;
            double N = total_mass * CONST.GRAVITY;
            //double sign = -Math.Sign(mp.f_sta + mp.f_dyn);
            double sign = -Math.Sign(mp.vv_curr);
            //double sign = -Math.Sign(mp.dv_curr);
            double f_friction = u * N * sign;

            return f_friction;
        }

        public double ApplyStatic(double acc, MechParams mp, MECHANICS type)
        {
            if (acc == 0.0d)
                acc = 0.1d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    // force left
                    double motor = CONST.BASE_SCALE;
                    double Fapplied = mp.m1 * acc;
                    return -(motor * mp.damp * Fapplied);
                case MECHANICS.BALLONHILL_LOW:
                    //slope force
                    double slope = 2 * mp.a * mp.pos_x; // Slope dy/dx
                    double sinTheta = slope / Math.Sqrt(1 + slope * slope);
                    double Fgravity = (mp.m1 * mp.g) * sinTheta;
                    return -(mp.damp * Fgravity);
                default: throw new Exception("m_NoiseGenerator, ApplyStatic");
            }
        }

        public double ApplyDynamic(double acc, MechParams mp, MECHANICS type, UNIT curr)
        {
            if (mind.goodbye)
                return 0.0d;

            if (acc == 0.0d)
                acc = 0.1d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    //force right
                    double motor = Logic(curr);                    
                    double Fapplied = mp.m2 * acc;            
                    return motor * mp.damp * Fapplied;
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
            PATTERN pattern = PATTERN.NONE;

            if (mind.z_current != "z_noise")
                return;

            if (pattern != match)
                return;

            if (cycles == 1)
                mh.ResetNoise(mind, mp);

            Calc(mind.unit_current, false, cycles);

            mh.ExtremesNoise(mp);
            mh.NormalizeNoise(mind, mp);
            ms.Convert(mp, this.type);
        }
    }
}

