using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class m_TugOfWar : IMechanics
    {
        public MechParams mp { get; set; }
        public MechHelper mh { get; set; }

        private TheMind mind;
        private m_TugOfWar() { }
        public m_TugOfWar(TheMind mind, PROPS props)
        {
            this.mind = mind;

            this.mh = new MechHelper() { };

            this.mp = new MechParams() { };
            
            mp.props = new Properties(mind, props);

            mp.Fmax = 5000.0d;                                              // Max oscillating force for F2
            mp.omega = 2 * Math.PI * 0.5;                                   // Frequency (0.5 Hz)
            mp.eta = 0.5d;                                                  // Randomness factor
            mp.m1 = 300.0d;                                                 // Mass of Car 1 in kg
            mp.m2 = 300.0d;                                                 // Mass of Car 2 in kg
            mp.totalMass = mp.m1 + mp.m2;
            mp.dt = 0.1d;                                                   // Time step (s)

            // Friction parameters
            mp.mu = 0.1;                                                    // Coefficient of kinetic friction
            mp.g = CONST.GRAVITY;                                           // Gravity in m/s^2
            mp.frictionForce = mp.mu * mp.totalMass * mp.g;

            mp.pos_x = CONST.STARTXY;

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
                double meter = mh.PosXY(mind, mp);

                return meter;
            }
        }

        public void Peek(UNIT curr)
        {
            throw new NotImplementedException();
        }
                
        private void Calc(PATTERN pattern, int cycles)
        {
            double t = cycles * mp.dt;

            double F1 = ApplyStatic(mp);                                        // Constant force in Newtons (e.g., truck pulling)
            double F2 = ApplyDynamic(mp, t);
            double Ffriction = mp.frictionForce * Math.Sign(mp.velocity);       // Friction opposes motion
            double Fnet = -F1 + F2 - Ffriction;                                 // Net force with F1 constant and F2 dynamic

            // If friction is stronger than applied force and velocity is near zero, stop motion
            if (Math.Abs(Fnet) < mp.frictionForce && Math.Abs(mp.velocity) < 0.01)
            {
                Fnet = 0;
                mp.velocity = 0;
            }

            double acceleration = Fnet / mp.totalMass;

            // Update velocity and position
            mp.velocity += acceleration * mp.dt;                                // Integrate acceleration to get velocity
            mp.pos_x += mp.velocity * mp.dt;                               // Integrate velocity to get position
            
            mp.p_prev = mp.p_curr;                                              // Store current momentum for next iteration
            mp.p_curr = mp.totalMass * mp.velocity;
            mp.d_curr = mp.p_curr - mp.p_prev;                                  // Compute change in momentum

            if (double.IsNaN(mp.velocity))
                ;// throw new Exception("NAN");
        }

        /*
         * force left
         * */
        public double ApplyStatic(MechParams mp)
        {
            double Fapplied = mp.Fmax * CONST.BASE_REDUCTION;

            return Fapplied;           
        }

        /*
         * force right
         * */
        public double ApplyDynamic(MechParams mp, double t)
        {
            if (mind.goodbye)
                return 0.0d;

            double Fapplied = mp.Fmax * (mh.Sine(mp.pattern_curr, t, mp.omega, 0.0d, 1.0d, 0.6d, 0.4d, 0.0d, 0.4d) + mh.GetRandomNoise(mind, mp.eta));  // Dynamic force
            
            return Fapplied;
        }

        public void Calculate(PATTERN match, int cycles)
        {
            PATTERN pattern = mind.parms_current.pattern;

            if (mind.z_current != "z_mech")
                return;

            if (pattern != match)
                return;

            mp.pattern_curr = pattern;

            if (cycles == 1)
                mh.Reset(mp);

            Calc(pattern, cycles);

            mh.Update(mp);
            mh.Normalize(mind, mp);
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

