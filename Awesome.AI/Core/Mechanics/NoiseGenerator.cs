using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class NoiseGenerator : IMechanics
    {
        public double n_momentum { get; set; }
        public double p_curr { get; set; }
        public double p_prev { get; set; }
        public double p_delta { get; set; }
        public double p_delta_prev { get; set; }
        public double m_out_high_c { get; set; }
        public double m_out_low_c { get; set; }
        public double m_out_high_n { get; set; }
        public double m_out_low_n { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        private TheMind mind;
        private NoiseGenerator() { }
        public NoiseGenerator(TheMind mind, Params parms)
        {
            this.mind = mind;

            posxy = Constants.STARTXY;

            m_out_high_n = -1000.0d;
            m_out_low_n = 1000.0d;
            d_out_high = -1000.0d;
            d_out_low = 1000.0d;
            posx_high = -1000.0d;
            posx_low = 1000.0d;
        }

        public FUZZYDOWN FuzzyMom 
        { 
            get { return p_delta.ToFuzzy(mind); } 
        }

        public HARDDOWN HardMom
        {
            get 
            {
                //return p_curr.ToDownPrev(p_prev, mind);
                //return p_curr.ToDownZero(mind);

                //return p_delta.ToDownPrev(p_delta_prev, mind);
                return p_delta.ToDownZero(mind);
            }            
        }

        private double posxy { get; set; }
        public double POS_XY
        {
            get
            {
                throw new NotImplementedException("NoiseGenerator, POS_XY");
            }
        }

        //Momentum
        public void Momentum(UNIT curr)
        {
            if (curr.IsNull())
                throw new Exception("NoiseGenerator, Momentum");

            if (curr.IsIDLE())
                throw new Exception("NoiseGenerator, Momentum");

            Calc(curr, true);
        }

        public void Calc(UNIT curr, bool peek)
        {
            double deltaT = 0.002d;
            double m = 500.0d;
            double N = m * Constants.GRAVITY;

            double Fsta = ApplyStatic();
            double Fdyn = ApplyDynamic(curr);
            double u = Friction(curr.credits, -2.0d);

            double Ffriction = u * N;
            double Fnet = -Fsta + Fdyn + (Ffriction * -Math.Sign(-Fsta + Fdyn));
            
            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
            //double dv = (Fnet * dt) / m;
            double deltaVel = (Fnet * deltaT) / m;
            
            //momentum: p = m * v
            if (peek) {
                n_momentum += (m * 2) * deltaVel;            
            }
            else {
                p_delta_prev = p_delta;
                p_delta = (m * 2) * deltaVel;
                p_curr += p_delta;
            }

            if (n_momentum <= m_out_low_n) m_out_low_n = n_momentum;
            if (n_momentum > m_out_high_n) m_out_high_n = n_momentum;

            if (p_curr <= m_out_low_n) m_out_low_c = p_curr;
            if (p_curr > m_out_high_n) m_out_high_c = p_curr;

            if (p_delta <= d_out_low) d_out_low = p_delta;
            if (p_delta > d_out_high) d_out_high = p_delta;
        }

        public void CalcPattern1(PATTERN pattern, int cycles)
        {
            if (mind.current != "noise")
                return;

            if (pattern != PATTERN.NONE)
                return;

            Calc(mind.unit["noise"], false);
        }

        public void CalcPattern2(PATTERN pattern, int cycles)
        {
            throw new NotImplementedException("NoiseGenerator, CalcPattern2");
        }

        public void CalcPattern3(PATTERN pattern, int cycles)
        {
            throw new NotImplementedException("NoiseGenerator, CalcPattern3");
        }          

        /*
         * force left
         * */
        public double ApplyStatic()
        {
            double acc = Constants.MAX / 10; //divided by 10 for aprox acc
            double m = 500.0d;
            
            double Fapplied = m * acc;
            
            if (Fapplied <= 0.0d)
                Fapplied = 0.0d;

            return Fapplied;
        }

        /*
         * force right
         * */
        public double ApplyDynamic(UNIT curr)
        {
            if (curr.IsNull())
                throw new Exception("ApplyDynamic");

            double max = Constants.MAX;
            double val = curr.Variable;
            double acc = (max - val) / 10.0d; //divided by 10 for aprox acc
            double m = 500.0d;

            if (acc <= 0.0d)
                acc = 0.0d;// jajajaa
                        
            double Fapplied = m * acc;
            
            if (Fapplied <= 0.0d)
                Fapplied = 0.0d;

            return Fapplied;
        }

        public double Friction(double credits, double shift)
        {
            /*
             * friction coeficient
             * should friction be calculated from position???
             * */

            Calc calc = mind.calc;

            double _c = 10.0d - credits;
            double x = 5.0d - _c + shift;
            double friction = calc.Logistic(x);

            return friction;
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

