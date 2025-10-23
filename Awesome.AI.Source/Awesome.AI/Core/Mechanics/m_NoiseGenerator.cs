using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class m_NoiseGenerator : IMechanics
    {
        public MechParams mp { get; set; }
        public MechHelper mh { get; set; }

        private TheMind mind;
        private m_NoiseGenerator() { }
        public m_NoiseGenerator(TheMind mind)
        {
            this.mind = mind;

            this.mh = new MechHelper() { };

            this.mp = new MechParams() { };

            mp.a_max = 5.0d;
            mp.damp = 0.5d;
            
            mp.posxy = CONST.STARTXY;

            mp.m_out_high_p = -1000.0d;
            mp.m_out_low_p = 1000.0d;
            mp.m_out_high = -1000.0d;
            mp.m_out_low = 1000.0d;
            mp.d_out_high = -1000.0d;
            mp.d_out_low = 1000.0d;
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

            mp.peek_norm = mind.calc.Normalize(mp.peek_momentum, mp.m_out_low_p, mp.m_out_high_p, 0.0d, 100.0d);
        }

        public void Calc(UNIT curr, bool peek, int cycles)
        {
            double dt_mod = 1.0d;
            
            if (mind.mech_high.mp.d_100 > 0.0d)
                dt_mod = mind.mech_high.mp.d_100 / 100.0d;

            mp.dt = 0.05d * dt_mod;
            mp.m1 = CONST.MAX * CONST.BASE_REDUCTION * 5.0d; //0 - 500
            mp.m2 = (curr.Variable) * 5.0d; //0 - 500
            double total_mass = mp.m1 + mp.m2;
            mp.N = total_mass * CONST.GRAVITY;

            double f_sta = ApplyStatic(mp);
            double f_dyn = ApplyDynamic(mp);
            double u = mh.Friction(mind, curr.credits, -0.0d, 0.01d);

            double f_friction = u * mp.N * -Math.Sign(-f_sta + f_dyn);
            double f_net = -f_sta + f_dyn + f_friction;

            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
                        
            double a_system = f_net / total_mass;
            double dv = a_system * mp.dt;
            
            //momentum: p = m * v
            if (peek) {
                mp.peek_momentum = mp.p_prev + total_mass * dv;            
            } else {
                mp.d_prev = mp.d_curr;
                mp.d_curr = total_mass * dv;
                mp.p_prev = mp.p_curr;
                mp.p_curr += mp.d_curr;
            }

            if (double.IsNaN(mp.d_curr))
                throw new Exception("NAN");
        }

        /*
         * force left
         * */
        public double ApplyStatic(MechParams mp)
        {
            double acc = mp.a_max;
            
            double Fapplied = mp.damp * mp.m1 * acc;
            
            if (Fapplied <= 0.0d)
                Fapplied = 0.0d;

            return Fapplied;
        }

        /*
         * force right
         * */
        public double ApplyDynamic(MechParams mp)
        {
            double acc = mp.a_max;

            double Fapplied = mp.damp * mp.m2 * acc;
            
            if (Fapplied <= 0.0d)
                Fapplied = 0.0d;

            return Fapplied;
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

            mh.UpdateNoise(mp);
            mh.NormalizeNoise(mind, mp);
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

