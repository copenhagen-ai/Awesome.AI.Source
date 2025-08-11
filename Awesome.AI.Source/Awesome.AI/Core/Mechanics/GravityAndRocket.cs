using Awesome.AI.Common;
using Awesome.AI.CoreInternals;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class GravityAndRocket : IMechanics
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
        public double d_prev {  get; set; }
        public double Fsta { get; set; }
        public double Fdyn { get; set; }
        public double m_out_high_c { get; set; }
        public double m_out_low_c { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        private TheMind mind;
        private GravityAndRocket() { }
        public GravityAndRocket(TheMind mind, Params parms)
        {
            this.mind = mind;

            posxy = CONST.STARTXY;

            //m_out_high = -1.0E20d;
            //m_out_low = 1.0E20d;
            //d_out_high = -1.0E20d;
            //d_out_low = 1.0E20d;
            posx_high = -1.0E20d;
            posx_low = 1.0E20d;
        }

        public FUZZYDOWN FuzzyMom
        {
            get { return d_curr.ToFuzzy(mind); }
        }

        public HARDDOWN HardMom
        {
            //return p_curr.ToDownPrev(p_prev, mind);
            //return p_curr.ToDownZero(mind);

            //return p_delta.ToDownPrev(p_delta_prev, mind);
            get { return d_curr.GoDownZero(mind); }            
        }

        private double posxy { get; set; }
        public double POS_XY
        {
            get 
            {
                if (position_x <= posx_low) posx_low = position_x;
                if (position_x > posx_high) posx_high = position_x;

                double x_meter = mind.calc.Normalize(position_x, posx_low, posx_high, 0.0d, 10.0d);

                if (x_meter <= CONST.RS && mind.goodbye.IsNo())
                    x_meter = CONST.RS + CONST.VERY_LOW;

                if (x_meter < CONST.LOWXY) x_meter = CONST.LOWXY;
                if (x_meter > CONST.HIGHXY) x_meter = CONST.HIGHXY;

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

            const double G = CONST.GRAV_CONST;      // gravitational constant
            const double M = 5E24;                  // black hole mass (in kg)
            const double rocketMass = 1000;         // in kg
            const double thrustAmplitude = 5000;    // thrust amplitude in Newtons
            const double eta = 0.5;
            const double omega = 0.1;               // frequency of sine wave
            const double dt = 0.1;                  // time step in seconds
            
            double t = cycles * dt;
            double r = 1E8;                         // initial distance from black hole (10,000 km)
            
            double gravityForce = -G * M * rocketMass / (r * r);
            double thrustForce = thrustAmplitude * (Sine(pattern, t, omega) + eta * GetRandomNoise());
            double netForce = gravityForce + (thrustForce < 0.0d ? 0.0d : thrustForce);

            double acceleration = netForce / rocketMass;
            velocity += acceleration * dt;                                      // Integrate acceleration to get velocity
            position_x += velocity * dt;                                        // Integrate velocity to get position
            r += velocity * dt;
            t += dt;

            p_prev = p_curr;                                                    // Store current momentum for next iteration
            p_curr = rocketMass * velocity;                                     // Calculate mommentum
            d_curr = p_curr - p_prev;                                           // Compute change in momentum

            if (p_curr <= m_out_low_c) m_out_low_c = p_curr;
            if (p_curr > m_out_high_c) m_out_high_c = p_curr;

            if (d_curr <= d_out_low) d_out_low = d_curr;
            if (d_curr > d_out_high) d_out_high = d_curr;

            // Delta momentum (Impulse) = F * dt
            //double deltaMomentum = netForce * dt;

            // Current momentum = mv
            //double momentum = rocketMass * v;

            if (r <= 0.0d)
                r = 0.0d;        
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
                throw new Exception("ApplyDynamic");

            double _var = curr_unit.Variable;

            return mind.calc.Normalize(_var, 0.0d, 100.0d, -1.0d, 1.0d);
            //return mind.rand.RandomDouble(-1d, 1d)); // Random value between -1 and 1
        }

        private double Sine(PATTERN pattern, double t, double omega)
        {
            switch (pattern)
            {
                case PATTERN.MOODGENERAL: return (Math.Sin(omega * t)) / 2.0d;
                case PATTERN.MOODGOOD: return (Math.Sin(omega * t) + 1.0d) / 2.0d;
                case PATTERN.MOODBAD: return (Math.Sin(omega * t) - 1.0d) / 2.0d;
                default: throw new Exception("TugOfWar, Sine");
            }
        }

        //public double VariableOld(UNIT curr)
        //{
        //    /*
        //     * I guess this is a changeable function, for now it is just the one I know to calculate force
        //     * 
        //     * earth mass:               5.972×10^24 kg
        //     * sun mass:                 1.989×10^30 kg
        //     * earth gravity:                  9.807 m/s²
        //     * distance sun:             148.010.000 km
        //     * distance moon:             3.844×10^5 km 3.844e5;
        //     * distance mercury(avg):    ~58 million km (~0.39 AU)
        //     * earth radius:                   6,371 km
        //     * millinium falken:            40000.0d kg
        //     * */

        //    if (curr.IsNull())
        //        throw new Exception("Variable");

        //    if (curr.IsIDLE())
        //        throw new Exception("Variable");

        //    double dist = mind.calc.NormalizeRange(curr.LowAtZero, 0.0d, 100.0d, 0.0d, 50000000000.0d);
        //    double mass_M = 5.972E24d;
        //    double mass_m = 40000.0d;

        //    //Gravitational Constant (G)
        //    double G = Constants.GRAV_CONST;

        //    // FORMEL: ~F = (G * (m * M) / r^2) * ~r
        //    double grav = G * ((mass_m * mass_M) / (dist * dist));

        //    return grav;
        //}

        //public void CalcPatternOld(MECHVERSION version)
        //{
        //    if (version != MECHVERSION.OLD)
        //        return;
        //    /*
        //     * still experimental..
        //     * I know its not using a black hole, but it should be the same principle outside the event horizon???
        //     * */

        //    double mod = ModifierOld(mind.curr_unit.credits, mind.parms.shift);
        //    double m = 40000.0d;
        //    double dt = 100.0d;    //delta time
        //    double Fnet = mind.curr_unit.Variable * mod;

        //    //F=m*a
        //    //a=dv/dt
        //    //F=(m*dv)/dt
        //    //F*dt=m*dv
        //    //dv=(F*dt)/m

        //    double deltaVel = (Fnet * dt) / m;            //delta velocity

        //    //momentum: p = m * v
        //    p_delta_prev = p_delta;
        //    p_delta = (m) * deltaVel;
        //    p_curr += p_delta;

        //    if (p_curr <= m_out_low) m_out_low = p_curr;
        //    if (p_curr > m_out_high) m_out_high = p_curr;

        //    if (p_delta <= d_out_low) d_out_low = p_delta;
        //    if (p_delta > d_out_high) d_out_high = p_delta;

        //    if (double.IsNaN(deltaVel))
        //        throw new Exception("Calculate");
        //}

        //public double ModifierOld(double credits, double shift)
        //{
        //    Calc calc = mind.calc;

        //    double _c = 10.0d - credits;
        //    double x = 5.0d - _c + shift;
        //    double mod = calc.Logistic(x);

        //    return mod < 0.5 ? -1d : 1d;
        //}        
    }
}

