﻿using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class TugOfWar : IMechanics
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
        public double m_out_high_c { get; set; }
        public double m_out_low_c { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        private TheMind mind;
        private TugOfWar() { }
        public TugOfWar(TheMind mind, Params parms)
        {
            this.mind = mind;

            position_x = CONST.STARTXY;

            m_out_high_c = -1000.0d;
            m_out_low_c = 1000.0d;
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
                
        public double POS_XY
        {
            get
            {
                double x_meter = position_x;

                if (x_meter <= 0.1d && mind.goodbye.IsNo())
                    x_meter = CONST.VERY_LOW;

                if (x_meter < CONST.LOWXY) x_meter = CONST.LOWXY;
                if (x_meter > CONST.HIGHXY) x_meter = CONST.HIGHXY;

                if (x_meter <= posx_low) posx_low = x_meter;
                if (x_meter > posx_high) posx_high = x_meter;

                return x_meter;
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


            double p_high = m_out_high_c;
            double p_low = m_out_low_c;
            double d_high = d_out_high;
            double d_low = d_out_low;

            double p_av = p_avg.Average();
            double d_av = d_avg.Average();

            if (p_av > p_high) p_high = p_av;
            if (p_av < p_low) p_low = p_av;

            if (d_av > d_high) d_high = d_av;
            if (d_av < d_low) d_low = d_av;

            p_100 = mind.calc.Normalize(p_av, p_low, p_high, 0.0d, 100.0d);
            d_100 = mind.calc.Normalize(d_av, d_low, d_high, 0.0d, 100.0d);

            p_90 = mind.calc.Normalize(p_av, p_low, p_high, 10.0d, 90.0d);
            d_90 = mind.calc.Normalize(d_av, d_low, d_high, 10.0d, 90.0d);
        }

        public void Peek(UNIT curr)
        {
            throw new NotImplementedException();
        }

        private double velocity = 0.0; // Initial velocity in m/s
        private double position_x = 5.0; // Initial position in meters
        private void Calc(PATTERN pattern, int cycles)
        {
            if (cycles == 1)
                Reset();

            double Fmax = 5000.0d;                                              // Max oscillating force for F2
            double omega = 2 * Math.PI * 0.5;                                   // Frequency (0.5 Hz)
            double eta = 0.5d;                                                  // Randomness factor
            double m1 = 300.0d;                                                 // Mass of Car 1 in kg
            double m2 = 300.0d;                                                 // Mass of Car 2 in kg
            double totalMass = m1 + m2;
            double dt = 0.1d;                                                   // Time step (s)

            // Friction parameters
            double mu = 0.1;                                                    // Coefficient of kinetic friction
            double g = CONST.GRAVITY;                                       // Gravity in m/s^2
            double frictionForce = mu * totalMass * g;

            double t = cycles * dt;

            double F1 = ApplyStatic(Fmax);                                      // Constant force in Newtons (e.g., truck pulling)
            double F2 = ApplyDynamic(pattern, Fmax, t, omega, eta);
            double friction = frictionForce * Math.Sign(velocity);             // Friction opposes motion
            double Fnet = -F1 + F2 - friction;                                  // Net force with F1 constant and F2 dynamic

            // If friction is stronger than applied force and velocity is near zero, stop motion
            if (Math.Abs(Fnet) < frictionForce && Math.Abs(velocity) < 0.01)
            {
                Fnet = 0;
                velocity = 0;
            }

            double acceleration = Fnet / totalMass;
            velocity += acceleration * dt;                                      // Integrate acceleration to get velocity
            position_x += velocity * dt;                                        // Integrate velocity to get position
            p_prev = p_curr;                                                    // Store current momentum for next iteration
            p_curr = totalMass * velocity;
            d_curr = p_curr - p_prev;                                          // Compute change in momentum

            if (p_curr <= m_out_low_c) m_out_low_c = p_curr;
            if (p_curr > m_out_high_c) m_out_high_c = p_curr;

            if (d_curr <= d_out_low) d_out_low = d_curr;
            if (d_curr > d_out_high) d_out_high = d_curr;
        }

        public void CalcPattern1(PATTERN pattern, int cycles)
        {
            if (mind.z_current != "z_mech")
                return;

            if (pattern != PATTERN.MOODGENERAL)
                return;

            pattern_curr = pattern;
            Calc(pattern, cycles);
            Normalize();
        }

        public void CalcPattern2(PATTERN pattern, int cycles)
        {
            if (mind.z_current != "z_mech")
                return;

            if (pattern != PATTERN.MOODGOOD)
                return;

            pattern_curr = pattern;
            Calc(pattern, cycles);
            Normalize();
        }

        public void CalcPattern3(PATTERN pattern, int cycles)
        {
            if (mind.z_current != "z_mech")
                return;

            if (pattern != PATTERN.MOODBAD)
                return;

            pattern_curr = pattern;
            Calc(pattern, cycles);
            Normalize();
        }

        PATTERN pattern_curr = PATTERN.NONE;
        PATTERN pattern_prev = PATTERN.NONE;
        private void Reset()
        {
            if (pattern_prev == pattern_curr)
                return;

            pattern_prev = pattern_curr;

            velocity = 0.0d;
            position_x = CONST.STARTXY;
            p_curr = 0.0d;
            d_curr = 0.0d;
            p_prev = 0.0d;

            //m_out_high = -1000.0d;
            //m_out_low = 1000.0d;
            //d_out_high = -1000.0d;
            //d_out_low = 1000.0d;
            posx_high = -1E10d;
            posx_low = 1E10d;
        }

        private double GetRandomNoise()
        {
            UNIT curr_unit = mind.unit_noise;

            if (curr_unit == null)
                throw new Exception("TugOfWar, GetRandomNoise");

            double _var = curr_unit.Variable;

            return mind.calc.Normalize(_var, 0.0d, 100.0d, -1.0d, 1.0d);
            //return mind.rand.RandomDouble(-1d, 1d)); // Random value between -1 and 1
        }

        private double Sine(PATTERN pattern, double t, double omega)
        {
            switch (pattern)
            {
                case PATTERN.MOODGENERAL: return (Math.Sin(omega * t) + 1.0d) / 2.0d;
                case PATTERN.MOODGOOD: return 0.6d + (Math.Sin(omega * t) + 1.0d) / 2.0d * 0.4d;
                case PATTERN.MOODBAD: return (Math.Sin(omega * t) + 1.0d) / 2.0d * 0.4d;
                default: throw new Exception("TugOfWar, Sine");
            }
        }

        /*
         * force left
         * */
        public double ApplyStatic(double Fmax)
        {
            double Fapplied = Fmax * CONST.BASE_REDUCTION;

            return Fapplied;           
        }

        /*
         * force right
         * */
        public double ApplyDynamic(PATTERN pattern, double Fmax, double t, double omega, double eta)
        {
            if (mind.goodbye.IsYes())
                return 0.0d;

            double Fapplied = Fmax * (Sine(pattern, t, omega) + eta * GetRandomNoise());  // Dynamic force
            
            return Fapplied;
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

