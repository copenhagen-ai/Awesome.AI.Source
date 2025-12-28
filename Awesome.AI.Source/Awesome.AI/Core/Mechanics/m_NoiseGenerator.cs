using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class m_NoiseGenerator : IMechanics
    {
        MECHANICS type;
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
                
        public double POS_XY
        {
            get
            {
                throw new NotImplementedException("NoiseGenerator, POS_XY");
            }
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

        public void Calc(UNIT curr, bool peek, int cycles)
        {
            DeltaTime();
                        
            double total_mass = 0.0d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    mp.damp = 0.2d;
                    mp.inertia_lim = 0.15d;
                    //mp.acc_max = 5.0d;
                    mp.m1 = CONST.MAX * CONST.BASE_REDUCTION * 5.0d; //0 - 500
                    mp.m2 = (curr.Variable) * 5.0d; //0 - 500
                    total_mass = mp.m1 + mp.m2;
                    break;
                case MECHANICS.BALLONHILL_LOW:
                    mp.damp = 1.0d;
                    mp.inertia_lim = 0.15d;
                    mp.a = 0.1d;// Parabola coefficient (hill steepness)
                    mp.g = CONST.GRAVITY;// Gravity (m/s^2)
                    mp.m1 = 0.35d;// Ball mass (kg)
                    total_mass = mp.m1;
                    break;
                default: throw new Exception("m_NoiseGenerator, Calc");
            }

            mp.f_sta = ApplyStatic(mp, this.type);
            mp.f_dyn = ApplyDynamic(mp, this.type);
            mp.f_friction = Friction(mp, type);

            double f_net = mp.f_sta + mp.f_dyn + mp.f_friction;

            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
                        
            double a_system = f_net / total_mass;
            double dv = a_system * mp.dt;
            
            //momentum: p = m * v
            if (peek) {
                mp.peek_velocity = mp.vv_prev + /*total_mass + */dv;            
            } else {
                mp.dv_prev = mp.dv_curr;
                mp.dv_curr = /*total_mass */dv;
                mp.vv_prev = mp.vv_curr;
                mp.vv_curr += mp.dv_curr;

                //mp.velocity += a_system * mp.dt;
                mp.pos_x += mp.vv_curr * mp.dt;
            }

            if (double.IsNaN(mp.dv_prev))
                throw new Exception("NAN");
            if (double.IsNaN(mp.dv_curr))
                throw new Exception("NAN");
            if (double.IsNaN(mp.vv_prev))
                throw new Exception("NAN");
            if (double.IsNaN(mp.vv_curr))
                throw new Exception("NAN");
        }

        public void DeltaTime()
        {
            double delta = mind.mech_high.ms.dv_sym_100;

            double mod = delta > 0.0d ? delta / 100.0d : 1.0d;

            mp.dt = 0.05d * mod;
        }

        public double Friction(MechParams mp, MECHANICS type)
        {
            double total_mass = 0.0d;

            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW: total_mass = mp.m1 + mp.m2; break;
                case MECHANICS.BALLONHILL_LOW: total_mass = mp.m1; break;
                default: throw new Exception("m_NoiseGenerator, Friction");
            }

            double u = mh.Friction(mind) * 0.05d;
            double N = total_mass * CONST.GRAVITY;
            double f_friction = u * N * -Math.Sign(mp.f_sta + mp.f_dyn);

            return f_friction;
        }

        public double ApplyStatic(MechParams mp, MECHANICS type)
        {
            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    // force left
                    double acc = mp.dv_curr == 0.0d ? 0.1d : mp.dv_curr / mp.dt;
                    double Fapplied = mp.m1 * acc;
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

        public double ApplyDynamic(MechParams mp, MECHANICS type)
        {
            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                    //force right
                    double acc = mp.dv_curr == 0.0d ? 0.1d : mp.dv_curr / mp.dt;
                    double Fapplied = mp.m2 * acc;            
                    return mp.damp * Fapplied;
                case MECHANICS.BALLONHILL_LOW:
                    //wind force
                    double windAccel = mind.unit_current.Variable * 0.2d; // constant wind acceleration
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

        //NewtonForce
        //public double Variable(UNIT curr)
        //{
        //    /*
        //     * I guess this is a changeable function, for now it is just the one I know to calculate force
        //     * */

        //    if (curr.IsNull())
        //        throw new Exception("Variable");

        //    if (curr.IsIDLE())
        //        throw new Exception("Variable");

        //    double acc = curr.HighAtZero;

        //    return acc;
        //}

        //public void CalcPatternOld(MECHVERSION version)
        //{
        //    if (version != MECHVERSION.OLD)
        //        return;

        //    //car left
        //    double Fsta = ApplyStaticOld();

        //    //car right
        //    double Fdyn = ApplyDynamicOld();

        //    double Fnet = mind.goodbye.IsNo() ? -Fsta + Fdyn : -Fsta;
        //    double m = 500.0d;
        //    //double dt = mind.parms.delta_time;                             //delta time, 1sec/500cyc
        //    double deltaT = 0.002d;

        //    //F=m*a
        //    //a=dv/dt
        //    //F=(m*dv)/dt
        //    //F*dt=m*dv
        //    //dv=(F*dt)/m
        //    //double dv = (Fnet * dt) / m;
        //    double deltaVel = (Fnet * deltaT) / m;

        //    //momentum: p = m * v
        //    p_delta_prev = p_delta;
        //    p_delta = (m * 2) * deltaVel;
        //    p_curr += p_delta;

        //    if (p_curr <= m_out_low) m_out_low = p_curr;
        //    if (p_curr > m_out_high) m_out_high = p_curr;

        //    if (p_delta <= d_out_low) d_out_low = p_delta;
        //    if (p_delta > d_out_high) d_out_high = p_delta;

        //    //if (double.IsNaN(velocity))
        //    //    throw new Exception();
        //}

        ///*
        // * car left
        // * */
        //public double ApplyStaticOld()
        //{
        //    double acc = HighestVar; //divided by 10 for aprox acc
        //    double m = 500.0d;
        //    double u = FrictionOld(true, 0.0d, mind.parms.shift);
        //    double N = m * Constants.GRAVITY;

        //    double Ffriction = u * N;
        //    double Fapplied = m * acc; //force, left
        //    double Fnet = Fapplied - Ffriction;

        //    if (Fnet <= 0.0d)
        //        Fnet = Constants.VERY_LOW;

        //    return Fnet;
        //}

        ///*
        // * car right
        // * */
        //public double ApplyDynamicOld()
        //{
        //    UNIT curr_unit_th = mind.curr_unit;

        //    if (curr_unit_th.IsNull())
        //        throw new Exception("ApplyDynamic");

        //    double max = HighestVar; //divided by 10 for aprox acc
        //    double acc = max - curr_unit_th.Variable; //divided by 10 for aprox acc
        //    double m = 500.0d;
        //    double u = FrictionOld(false, curr_unit_th.credits, mind.parms.shift);
        //    double N = m * Constants.GRAVITY;

        //    acc = acc == 0.0d ? Constants.VERY_LOW : acc;// jajajaa

        //    double Ffriction = u * N;
        //    double Fapplied = m * acc; //force, left
        //    double Fnet = Fapplied - Ffriction;

        //    if (Fnet <= 0.0d)
        //        Fnet = Constants.VERY_LOW;

        //    return Fnet;
        //}

        //public double FrictionOld(bool is_static, double credits, double shift)
        //{
        //    /*
        //     * friction coeficient
        //     * should friction be calculated from position???
        //     * */

        //    if (is_static)
        //        return Constants.BASE_REDUCTION;

        //    Calc calc = mind.calc;

        //    double _c = 10.0d - credits;
        //    double x = 5.0d - _c + shift;
        //    double friction = calc.Logistic(x);

        //    return friction;
        //}
    }
}

