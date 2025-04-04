using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class TugOfWar : IMechanics
    {
        public double p_curr { get; set; }
        public double p_prev { get; set; }
        public double p_delta { get; set; }
        public double p_delta_prev { get; set; }
        public double m_out_high { get; set; }
        public double m_out_low { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        private TheMind mind;
        private TugOfWar() { }
        public TugOfWar(TheMind mind, Params parms)
        {
            this.mind = mind;

            posxy = Constants.STARTXY;//10;

            m_out_high = -1000.0d;
            m_out_low = 1000.0d;
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
                if (Constants.Logic == LOGICTYPE.BOOLEAN)
                    //return deltaMom.ToDownPrev(deltaMomPrev, mind);
                    return p_delta.ToDownZero(mind);

                if (Constants.Logic == LOGICTYPE.QUBIT)
                    //return deltaMom.ToDownPrev(deltaMomPrev, mind);
                    return p_delta.ToDownZero(mind);

                throw new Exception("HardMom");
            }            
        }

        public double HighestVar
        {
            get { return Variable(UNIT.GetLow); }
        }

        public double LowestVar
        {
            get { return Variable(UNIT.GetHigh); }
        }

        private double posxy { get; set; }
        public double POS_XY
        {
            get
            {
                //its a hack, yes its cheating..
                double boost = mind.goodbye.IsNo() ? mind.parms.boost : 1.0d;

                //posxy = 10.0d + (boost * momentum);//dosnt seem right
                posxy += (boost * p_delta);
                //posxy = posx + (boost * velocity);
                //posxy = 10.0d + (boost * momentum);

                if (posxy < Constants.LOWXY)
                    posxy = Constants.LOWXY;
                if (posxy > Constants.HIGHXY)
                    posxy = Constants.HIGHXY;

                if (posxy <= posx_low) posx_low = posxy;
                if (posxy > posx_high) posx_high = posxy;

                return posxy;
            }
        }

        public double Variable(UNIT curr)
        {
            if (curr.IsNull())
                throw new Exception("Variable");

            if (curr.IsIDLE())
                throw new Exception("Variable");

            double _var = curr.HighAtZero;

            return _var;
        }

        private double velocity = 0.0;
        public void CalcPattern1(MECHVERSION version, int cycles)
        {
            if (version != MECHVERSION.MOODGENERAL)
                return;

            if (cycles == 1)
                Reset1();


            double Fmax = 5000.0d;                                              // Max oscillating force for F2
            double omega = 2 * Math.PI * 0.5;                                   // Frequency (0.5 Hz)
            double eta = 0.2d;                                                  // Randomness factor
            double m1 = 500.0d;                                        // 1500; // Mass of Car 1 in kg
            double m2 = 500.0d;                                        // 1300; // Mass of Car 2 in kg
            double totalMass = m1 + m2;
            double dt = 0.1d;                                                   // Time step (s)

            // Friction parameters
            double mu = 0.1;                                                    // Coefficient of kinetic friction
            double g = Constants.GRAVITY;                                       // Gravity in m/s^2
            double frictionForce = mu * totalMass * g;

            double t = cycles * dt;

            double F1 = ApplyStatic1(Fmax);                                     // Constant force in Newtons (e.g., truck pulling)
            double F2 = ApplyDynamic1(Fmax, t, omega, eta);
            double friction = frictionForce * Math.Sign(velocity);              // Friction opposes motion
            double Fnet = F1 - F2 - friction;                                   // Net force with F1 constant and F2 dynamic

            // If friction is stronger than applied force and velocity is near zero, stop motion
            if (Math.Abs(Fnet) < frictionForce && Math.Abs(velocity) < 0.01)
            {
                Fnet = 0;
                velocity = 0;
            }

            double acceleration = Fnet / totalMass;
            velocity += acceleration * dt;                                      // Integrate acceleration to get velocity
            p_curr = totalMass * velocity;
            p_delta = p_curr - p_prev;                                          // Compute change in momentum
            p_prev = p_curr;                                                    // Store current momentum for next iteration

            if (p_curr <= m_out_low) m_out_low = p_curr;
            if (p_curr > m_out_high) m_out_high = p_curr;

            if (p_delta <= d_out_low) d_out_low = p_delta;
            if (p_delta > d_out_high) d_out_high = p_delta;
        }

        private void Reset1()
        {
            velocity = 0.0d;
            p_curr = 0.0d;
            p_delta = 0.0d;
            p_prev = 0.0d;

            posxy = Constants.STARTXY;//10;

            m_out_high = -1000.0d;
            m_out_low = 1000.0d;
            d_out_high = -1000.0d;
            d_out_low = 1000.0d;
            posx_high = -1000.0d;
            posx_low = 1000.0d;
        }

        private double GetRandomNoise1()
        {
            UNIT curr_unit = mind.curr_unit;

            if (curr_unit == null)
                throw new Exception("ApplyDynamic");

            double _var = curr_unit.Variable;

            return mind.calc.NormalizeRange(_var, 0.0d, 100.0d, -1.0d, 1.0d);
            //return mind.rand.RandomDouble(-1d, 1d)); // Random value between -1 and 1
        }

        /*
         * car left
         * */
        public double ApplyStatic1(double Fmax)
        {
            double Fapplied = Fmax * Constants.BASE_REDUCTION;

            return Fapplied;           
        }

        /*
         * car right
         * */
        public double ApplyDynamic1(double Fmax, double t, double omega, double eta)
        {
            double Fapplied = Fmax * (Math.Sin(omega * t) + eta * GetRandomNoise1());  // Dynamic force
            
            return Fapplied;
        }

        public double Friction1(double credits, double shift)
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

        public void CalcPattern2(MECHVERSION version, int cycles)
        {
            if (version != MECHVERSION.MOODGOOD)
                return;

            throw new NotImplementedException();
        }

        public void CalcPattern3(MECHVERSION version, int cycles)
        {
            if (version != MECHVERSION.MOODBAD)
                return;

            throw new NotImplementedException();
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

