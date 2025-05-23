﻿using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class NoiseGenerator : IMechanics
    {
        public double peek_momentum { get; set; }
        public double peek_norm { get; set; }
        public double p_100 { get; set; }
        public double d_100 { get; set; }
        public double p_90 { get; set; }
        public double d_90 { get; set; }
        public double p_curr { get; set; }
        public double p_prev { get; set; }
        public double d_curr { get; set; }
        public double d_prev { get; set; }
        public double m_out_high_p { get; set; }
        public double m_out_low_p { get; set; }
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

            posxy = CONST.STARTXY;

            m_out_high_p = -1000.0d;
            m_out_low_p = 1000.0d;
            m_out_high_n = -1000.0d;
            m_out_low_n = 1000.0d;
            d_out_high = -1000.0d;
            d_out_low = 1000.0d;
            posx_high = -1000.0d;
            posx_low = 1000.0d;
        }

        public FUZZYDOWN FuzzyMom 
        { 
            get { return d_curr.ToFuzzy(mind); } 
        }

        public HARDDOWN HardMom
        {
            get 
            {
                //return p_curr.ToDownPrev(p_prev, mind);
                //return p_curr.ToDownZero(mind);

                //return p_delta.ToDownPrev(p_delta_prev, mind);
                return d_curr.ToDownZero(mind);
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

        List<double> p_avg = new List<double>();
        List<double> d_avg = new List<double>();
        public void Normalize()
        {
            p_avg ??= new List<double>();
            p_avg.Add(p_curr);
            if (p_avg.Count > 1)
                p_avg.RemoveAt(0);

            d_avg ??= new List<double>();
            d_avg.Add(d_curr);
            if (d_avg.Count > 1)
                d_avg.RemoveAt(0);


            double p_av = p_avg.Average();
            double d_av = d_avg.Average();

            p_100 = mind.calc.Normalize(p_av, m_out_low_n, m_out_high_n, 0.0d, 100.0d);
            d_100 = mind.calc.Normalize(d_av, d_out_low, d_out_high, 0.0d, 100.0d);

            p_90 = mind.calc.Normalize(p_av, m_out_low_n, m_out_high_n, 10.0d, 90.0d);
            d_90 = mind.calc.Normalize(d_av, d_out_low, d_out_high, 10.0d, 90.0d);
        }

        public void Peek(UNIT curr)
        {
            if (curr.IsNull())
                throw new Exception("NoiseGenerator, Momentum");

            if (curr.IsIDLE())
                throw new Exception("NoiseGenerator, Momentum");

            Calc(curr, true, -1);

            peek_norm = mind.calc.Normalize(peek_momentum, m_out_low_p, m_out_high_p, 0.0d, 100.0d);
        }

        public void Calc(UNIT curr, bool peek, int cycles)
        {
            if (cycles == 1)
                Reset();

            double deltaT = 0.02d;
            double m = 500.0d;
            double N = m * CONST.GRAVITY;

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
                peek_momentum = p_prev + (m * 2) * deltaVel;            
            } else {
                d_prev = d_curr;
                d_curr = (m * 2) * deltaVel;
                p_prev = p_curr;
                p_curr += d_curr;
            }

            if (peek_momentum <= m_out_low_p) m_out_low_p = peek_momentum;
            if (peek_momentum > m_out_high_p) m_out_high_p = peek_momentum;

            if (p_curr <= m_out_low_n) m_out_low_n = p_curr;
            if (p_curr > m_out_high_n) m_out_high_n = p_curr;

            if (d_curr <= d_out_low) d_out_low = d_curr;
            if (d_curr > d_out_high) d_out_high = d_curr;
        }

        public void CalcPattern1(PATTERN pattern, int cycles)
        {
            if (mind.z_current != "z_noise")
                return;

            if (pattern != PATTERN.NONE)
                return;

            Calc(mind.unit_noise, false, cycles);
            Normalize();
        }

        public void CalcPattern2(PATTERN pattern, int cycles)
        {
            throw new NotImplementedException("NoiseGenerator, CalcPattern2");
        }

        public void CalcPattern3(PATTERN pattern, int cycles)
        {
            throw new NotImplementedException("NoiseGenerator, CalcPattern3");
        }

        private void Reset() 
        {
            //could be done in other ways also
            if (!CONST.SAMPLE200.RandomSample(mind)) 
                return;

            posxy = CONST.STARTXY;

            m_out_high_p = -1000.0d;
            m_out_low_p = 1000.0d;
            m_out_high_n = -1000.0d;
            m_out_low_n = 1000.0d;
            d_out_high = -1000.0d;
            d_out_low = 1000.0d;
            posx_high = -1000.0d;
            posx_low = 1000.0d;
        }

        /*
         * force left
         * */
        public double ApplyStatic()
        {
            double acc = CONST.MAX / 10; //divided by 10 for aprox acc
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

            double max = CONST.MAX;
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

