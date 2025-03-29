using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class _TheContest : IMechanics
    {
        public double momentum { get; set; }
        public double momentumPrev { get; set; }
        public double deltaMom { get; set; }
        public double deltaMomPrev { get; set; }
        public double m_out_high { get; set; }
        public double m_out_low { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        private TheMind mind;
        private _TheContest() { }
        public _TheContest(TheMind mind, Params parms)
        {
            this.mind = mind;

            posxy = Constants.STARTXY;

            m_out_high = -1000.0d;
            m_out_low = 1000.0d;
            d_out_high = -1000.0d;
            d_out_low = 1000.0d;
            posx_high = -1000.0d;
            posx_low = 1000.0d;
        }

        public FUZZYDOWN FuzzyMom 
        { 
            get { return deltaMom.ToFuzzy(mind); } 
        }

        public HARDDOWN HardMom
        {
            get 
            {
                if (Constants.Logic == LOGICTYPE.BOOLEAN)
                    //return deltaMom.ToDownPrev(deltaMomPrev, mind);
                    return deltaMom.ToDownZero(mind);

                if (Constants.Logic == LOGICTYPE.QUBIT)
                    //return deltaMom.ToDownPrev(deltaMomPrev, mind);
                    return deltaMom.ToDownZero(mind);

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
                posxy += (boost * deltaMom);
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

        //NewtonForce
        public double Variable(UNIT curr)
        {
            /*
             * I guess this is a changeable function, for now it is just the one I know to calculate force
             * */

            if (curr.IsNull())
                throw new Exception("Variable");

            if (curr.IsIDLE())
                throw new Exception("Variable");

            double acc = curr.HighAtZero;

            return acc;
        }

        public void CalculateOld()
        {
            //car left
            double Fsta = ApplyStatic();

            //car right
            double Fdyn = ApplyDynamic();

            double Fnet = mind.goodbye.IsNo() ? -Fsta + Fdyn : -Fsta;
            double m = mind.parms.mass;
            //double dt = mind.parms.delta_time;                             //delta time, 1sec/500cyc
            double deltaT = mind.parms.delta_time;

            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
            //double dv = (Fnet * dt) / m;
            double deltaVel = (Fnet * deltaT) / m;

            //momentum: p = m * v
            deltaMomPrev = deltaMom;
            deltaMom = (m * 2) * deltaVel;
            momentum += deltaMom;

            if (momentum <= m_out_low) m_out_low = momentum;
            if (momentum > m_out_high) m_out_high = momentum;

            if (deltaMom <= d_out_low) d_out_low = deltaMom;
            if (deltaMom > d_out_high) d_out_high = deltaMom;

            //if (double.IsNaN(velocity))
            //    throw new Exception();
        }

        /*
         * car left
         * */
        public double ApplyStatic()
        {
            double acc = HighestVar; //divided by 10 for aprox acc
            double m = mind.parms.mass;
            double u = Friction(true, 0.0d, mind.parms.shift);
            double N = m * Constants.GRAVITY;

            double Ffriction = u * N;
            double Fapplied = m * acc; //force, left
            double Fnet = Fapplied - Ffriction;

            if (Fnet <= 0.0d)
                Fnet = Constants.VERY_LOW;

            return Fnet;
        }

        /*
         * car right
         * */
        public double ApplyDynamic()
        {
            UNIT curr_unit_th = mind.curr_unit;

            if (curr_unit_th.IsNull())
                throw new Exception("ApplyDynamic");

            double max = HighestVar; //divided by 10 for aprox acc
            double acc = max - curr_unit_th.Variable; //divided by 10 for aprox acc
            double m = mind.parms.mass;
            double u = Friction(false, curr_unit_th.credits, mind.parms.shift);
            double N = m * Constants.GRAVITY;

            acc = acc == 0.0d ? Constants.VERY_LOW : acc;// jajajaa

            double Ffriction = u * N;
            double Fapplied = m * acc; //force, left
            double Fnet = Fapplied - Ffriction;

            if (Fnet <= 0.0d)
                Fnet = Constants.VERY_LOW;

            return Fnet;
        }

        public double Friction(bool is_static, double credits, double shift)
        {
            /*
             * friction coeficient
             * should friction be calculated from position???
             * */

            if (is_static)
                return Constants.BASE_REDUCTION;

            Calc calc = mind.calc;

            double _c = 10.0d - credits;
            double x = 5.0d - _c + shift;
            double friction = calc.Logistic(x);

            return friction;
        }

        private double velocity = 0.0;
        private double timeStep = 0.0;
        public void CalculateNew(int cycles)
        {
            if(cycles == 1)
                Reset();

            double t = cycles / 10.0d;

            double Fmax = 5000.0d;                                              // Max oscillating force for F2
            double omega = 2 * Math.PI * 0.5;                                   // Frequency (0.5 Hz)
            double eta = 0.2;                                                   // Randomness factor
            double m1 = mind.parms.mass;                                        // 1500;    // Mass of Car 1 in kg
            double m2 = mind.parms.mass;                                        // 1300;    // Mass of Car 2 in kg
            double totalMass = m1 + m2;
                        
            // Friction parameters
            double mu = 0.1;                                                    // Coefficient of kinetic friction
            double g = Constants.GRAVITY;                                       // Gravity in m/s^2
            double frictionForce = mu * totalMass * g;

            double F1 = ApplyStaticNew(Fmax);                                   // Constant force in Newtons (e.g., truck pulling)
            double F2 = ApplyDynamicNew(Fmax, t, omega, eta);
            double friction = frictionForce * Math.Sign(velocity);              // Friction opposes motion
            double Fnet = F1 - F2 - friction;                                   // Net force with F1 constant and F2 dynamic

            // If friction is stronger than applied force and velocity is near zero, stop motion
            if (Math.Abs(Fnet) < frictionForce && Math.Abs(velocity) < 0.01)
            {
                Fnet = 0;
                velocity = 0;
            }

            double acceleration = Fnet / totalMass;
            timeStep = cycles == 1 ? t : timeStep;                              // Time step in seconds
            velocity += acceleration * timeStep;                                // Integrate acceleration to get velocity
            momentum = totalMass * velocity;
            deltaMom = momentum - momentumPrev;                                 // Compute change in momentum
            momentumPrev = momentum;                                            // Store current momentum for next iteration

            if (momentum <= m_out_low) m_out_low = momentum;
            if (momentum > m_out_high) m_out_high = momentum;

            if (deltaMom <= d_out_low) d_out_low = deltaMom;
            if (deltaMom > d_out_high) d_out_high = deltaMom;
        }

        private void Reset()
        {
            timeStep = 0.0d;
            velocity = 0.0d;
            momentum = 0.0d;
            deltaMom = 0.0d;
            momentumPrev = 0.0d;
        }

        private double GetRandomNoise()
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
        public double ApplyStaticNew(double Fmax)
        {
            double Fapplied = Fmax * Constants.BASE_REDUCTION;

            return Fapplied;           
        }

        /*
         * car right
         * */
        public double ApplyDynamicNew(double Fmax, double t, double omega, double eta)
        {
            double Fapplied = Fmax * (Math.Sin(omega * t) + eta * GetRandomNoise());  // Dynamic force
            
            return Fapplied;
        }

        public double FrictionNew(double credits, double shift)
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

        //public void CALC()
        //{
        //    bool reset = velocity >= 0.0d; //maybe 0.666 * max_velocity

        //    //if(reset)
        //    //    velocity = 0.0d;

        //    //if (reset)  //car left
        //    Fsta = ApplyStatic();

        //    //car right
        //    if (mind.goodbye.IsNo())
        //        Fdyn = ApplyDynamic();

        //    Calc calc = new Calc(mind);

        //    //momentum: p = m * v
        //    momentum = (mind.parms.mass * 2) * velocity;
        //    //momentum += 12.0d;// calc.RoundOff((int)out_low);

        //    if (momentum <= out_low) out_low = momentum;
        //    if (momentum > out_high) out_high = momentum;

        //    if (double.IsNaN(velocity))
        //        throw new Exception();
        //}

        /*
         * car left
         * */
        //public double ApplyStatic()
        //{
        //    double acc = mind.common.HighestForce().Variable;
        //    double limit = lim.Limit(true);

        //    double m = mind.parms.mass;
        //    double F = m * acc;                       //force, left
        //    double dt = DeltaT();                   //delta time
        //    double dv = DeltaV(F, m, dt) * limit;   //delta velocity

        //    velocity -= dv;

        //    return dv;
        //}

        /*
         * car right
         * */
        //public double ApplyDynamic()
        //{
        //    UNIT curr_unit_th = mind.curr_unit;

        //    if (curr_unit_th.IsNull())
        //        throw new Exception();

        //    bool first_run = false;
        //    if (mind.cycles_all <= mind.parms.first_run)
        //        first_run = true;

        //    double max = mind.common.HighestForce().Variable;
        //    double acc = max - curr_unit_th.Variable;
        //    double limit = first_run ? 0.5d : lim.Limit(false);

        //    double m = mind.parms.mass;
        //    double F = m * acc;                       //force, right
        //    double dt = DeltaT();                   //delta time
        //    double dv = DeltaV(F, m, dt) * limit;   //delta velocity

        //    //if (goodbye.IsNo())
        //    //if (goodbye.IsNo() && momentum < 0.0d)
        //        velocity += dv;

        //    return dv;
        //}/**/


        //private double DeltaV(double F, double m, double dt)
        //{
        //    //F=m*a
        //    //a=dv/dt
        //    //F=(m*dv)/dt
        //    //F*dt=m*dv
        //    //dv=(F*dt)/m
        //    double dv = (F * dt) / m;
        //    return dv;
        //}

        //private double DeltaT()
        //{
        //    //most of the time this is true

        //    double x = mind.parms.micro_sec;
        //    double dt = x / 1000000.0d;
        //    return dt;
        //}

        //private void Reset(TheMind mind)
        //{
        //    if (mind.cycles_all % 25000 != 0)
        //        return;

        //    out_low *= 0.5d;
        //    out_high *= 0.5d;

        //    posx_low *= 0.5d;
        //    posx_high *= 0.5d;

        //    min *= 0.5d;
        //    max *= 0.5d;
        //}               
    }
}

