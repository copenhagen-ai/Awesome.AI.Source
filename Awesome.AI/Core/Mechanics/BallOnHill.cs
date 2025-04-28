using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class BallOnHill : IMechanics
    {
        public double peek_momentum { get; set; }
        public double p_norm { get; set; }
        public double d_norm { get; set; }
        public double p_curr { get; set; }
        public double p_prev { get; set; }
        public double d_curr { get; set; }
        public double d_prev {  get; set; }

        public double m_out_high_c { get; set; }
        public double m_out_low_c { get; set; }
        public double m_out_high_n { get; set; }
        public double m_out_low_n { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        
        private TheMind mind;
        private BallOnHill() { }
        public BallOnHill(TheMind mind, Params parms)
        {
            this.mind = mind;

            m_out_high_c = -1000.0d;
            m_out_low_c = 1000.0d;
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

        public double POS_XY
        {
            get
            {
                double posxy = position_x;

                if (posxy <= 0.1d && mind.goodbye.IsNo())
                    posxy = CONST.VERY_LOW;
                
                if (posxy < CONST.LOWXY) posxy = CONST.LOWXY;
                if (posxy > CONST.HIGHXY) posxy = CONST.HIGHXY;

                if (posxy <= posx_low) posx_low = posxy;
                if (posxy > posx_high) posx_high = posxy;

                return posxy;
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

            p_norm = mind.calc.Normalize(p_av, p_low, p_high, 10.0d, 90.0d);
            d_norm = mind.calc.Normalize(d_av, d_low, d_high, 10.0d, 90.0d);
        }

        public void Peek(UNIT _c)
        {
            throw new NotImplementedException();
        }

        private double velocity = 0.0;
        private double position_x = CONST.STARTXY;
        private void Calc(PATTERN pattern, int cycles)
        {
            if (cycles == 1)
                Reset();

            // Constants
            double a = 0.1d;                    // Parabola coefficient (hill steepness)
            double g = CONST.GRAVITY;       // Gravity (m/s^2)
            double F0 = 5.0d;                   // Wind force amplitude (N)
            double omega = Math.PI;             // Wind frequency
            double beta = 0.02d;                // Friction coefficient
            double dt = 0.1d;                   // Time step (s)
            double eta = 0.5d;                  // Random noise amplitude for wind force
            double m = 0.35d;                    // Ball mass (kg)

            double t = cycles * dt;

            // Compute forces
            double Fx = ApplyDynamic(pattern, omega, t, F0, eta); // Wind force
            double Fgravity = ApplyStatic(m, g, a, position_x);
            double Ffriction = -beta * velocity;

            // Compute acceleration along the tangent
            double a_tangent = (Fgravity + Fx + Ffriction) / m;

            // Update velocity and position
            velocity += a_tangent * dt;
            position_x += velocity * dt;

            p_prev = p_curr;
            p_curr = m * velocity;
            d_curr = p_curr - p_prev;

            if (p_curr <= m_out_low_c) m_out_low_c = p_curr;
            if (p_curr > m_out_high_c) m_out_high_c = p_curr;

            if (d_curr <= d_out_low) d_out_low = d_curr;
            if (d_curr > d_out_high) d_out_high = d_curr;
        }

        public void CalcPattern1(PATTERN pattern, int cycles)
        {
            if (mind.current != "mech")
                return;

            if (pattern != PATTERN.MOODGENERAL)
                return;

            pattern_curr = pattern;
            Calc(pattern, cycles);
            Normalize();
        }

        public void CalcPattern2(PATTERN pattern, int cycles)
        {
            if (mind.current != "mech")
                return;

            if (pattern != PATTERN.MOODGOOD)
                return;

            pattern_curr = pattern;
            Calc(pattern, cycles);
            Normalize();
        }

        public void CalcPattern3(PATTERN pattern, int cycles)
        {
            if (mind.current != "mech")
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

            position_x = CONST.STARTXY;
            velocity = 0.0d;
            p_curr = 0.0d;
            d_curr = 0.0d;
            p_prev = 0.0d;

            //m_out_high = -1000.0d;
            //m_out_low = 1000.0d;
            //d_out_high = -1000.0d;
            //d_out_low = 1000.0d;
            posx_high = -1000.0d;
            posx_low = 1000.0d;
        }

        private double GetRandomNoise(double noiseAmplitude)
        {
            UNIT curr_unit = mind.unit_noise;

            if (curr_unit == null)
                throw new Exception("ApplyDynamic");

            double _var = curr_unit.Variable;

            double rand = mind.calc.Normalize(_var, 0.0d, 100.0d, -1.0d, 1.0d);
            
            return rand * noiseAmplitude;// Random value in range [-amplitude, amplitude]
        }

        private double Sine(PATTERN pattern, double t, double omega)
        {
            switch (pattern)
            {
                case PATTERN.MOODGENERAL: return (Math.Sin(omega * t) + 1.0d) / 2.0d;
                case PATTERN.MOODGOOD: return 0.5d + (Math.Sin(omega * t) + 1.0d) / 2.0d * 0.5d;
                case PATTERN.MOODBAD: return (Math.Sin(omega * t) + 1.0d) / 2.0d * 0.5d;
                default: throw new Exception("BallOnHill, Sine");
            }
        }

        private double ApplyStatic(double m, double g, double a, double x)
        {
            double slope = 2 * a * x; // Slope dy/dx
            double sinTheta = slope / Math.Sqrt(1 + slope * slope);
            double Fgravity = -(m * g) * sinTheta;

            return Fgravity;
        }

        private double ApplyDynamic(PATTERN pattern, double omega, double t, double F0, double eta)
        {
            if(mind.goodbye.IsYes())
                return 0.0d;

            double Fx = F0 * Sine(pattern, t, omega) + GetRandomNoise(eta); // Wind force

            if (Fx < 0.0d)
                Fx = 0.0d;
            
            return Fx;
        }


        //Weight
        //public double Variable(UNIT _c)
        //{
        //    /*
        //     * This is a changeable function.
        //     * 
        //     * Weight
        //     * W = m * g
        //     * */
        //    if (_c.IsNull())
        //        throw new Exception("Variable");

        //    if (_c.IsIDLE())
        //        throw new Exception("Variable");

        //    double earth_gravity = Constants.GRAVITY;
        //    double mass = 0.5d;
            
        //    double res = (mass * earth_gravity) * (_c.HighAtZero / 100.0d);

        //    return res;
        //}

        //double res_x = 5.0d;
        //public void CalcPatternOld(MECHVERSION version)
        //{
        //    if (version != MECHVERSION.OLD)
        //        return;

        //    double var_a = -0.1d;
        //    double var_b = 0.0d;
        //    double var_c = 10.0d;

        //    MyVector2D calc = new MyVector2D();
        //    MyVector2D res, sta = new MyVector2D(), dyn = new MyVector2D();

        //    double acc_degree = SlopeInDegreesOld(x, var_a, var_b);

        //    sta = ApplyStaticOld(acc_degree);
            
        //    if (mind.goodbye.IsNo())
        //        dyn = ApplyDynamicOld(acc_degree);

        //    res = calc.Add(sta, dyn);
        //    res_x = res.xx;
        //    res = calc.ToPolar(res);

        //    if (res_x < 0.0d) res_x = 0.0d;
        //    if (res_x > 10.0d) res_x = 10.0d;

        //    double acc = res.yy < 0.0d ? res.magnitude : -res.magnitude;

        //    double m = 0.5d;
        //    //double dt = mind.parms.delta_time;
        //    double deltaT = 0.5d;

        //    //F=m*a
        //    //a=dv/dt
        //    //dv=a*dt
        //    //double dv = acc * dt;
        //    double deltaVel = acc * deltaT;

        //    p_delta_prev = p_delta;
        //    p_delta = m * deltaVel;
        //    p_curr += p_delta;

        //    if (p_curr <= m_out_low) m_out_low = p_curr;
        //    if (p_curr > m_out_high) m_out_high = p_curr;

        //    if (p_delta <= d_out_low) d_out_low = p_delta;
        //    if (p_delta > d_out_high) d_out_high = p_delta;
        //}

        //private double SlopeInDegreesOld(double x, double var_a, double var_b)
        //{
        //    double acc_slope, _x, _y;

        //    MyVector2D calc = new MyVector2D();
        //    MyVector2D _slope;
            
        //    acc_slope = mind.calc.SlopeCoefficient(x, var_a, var_b);
        //    _x = 1.0d;
        //    _y = acc_slope;
        //    _slope = calc.ToPolar(new MyVector2D(_x, _y, null, null));
        //    double acc_degree = _slope.theta_in_degrees;
            
        //    return acc_degree;
        //}

        //public MyVector2D ApplyStaticOld(double acc_degree)
        //{
        //    double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
        //    double angle_sta = -90.0d;
        //    double angle_com_y_vec = -90.0d - acc_degree_positive;//-135
        //    double angle_com_y_pyth = 90.0d - acc_degree_positive;//-135

        //    double force_sta = HighestVar;
        //    double force_com_y = mind.calc.PythNear(angle_com_y_pyth, force_sta);

        //    MyVector2D calc = new MyVector2D();
        //    MyVector2D _static = calc.ToCart(new MyVector2D(null, null, force_sta, calc.ToRadians(angle_sta)));
        //    MyVector2D _N = calc.ToCart(new MyVector2D(null, null, force_com_y, calc.ToRadians(angle_com_y_vec + 180.0d)));
        //    MyVector2D _fN = calc.ToPolar((calc.Add(_static, _N)));

        //    double m = 0.5d;
        //    double u = FrictionOld(true, 0.0d, mind.parms.shift);
        //    double N = m * Constants.GRAVITY;

        //    double Ffriction = u * N;
        //    double Fapplied = _fN.magnitude;
        //    double Fnet = Fapplied - Ffriction;

        //    if (Fnet <= Constants.VERY_LOW)
        //        Fnet = Constants.VERY_LOW;

        //    MyVector2D _res = calc.ToCart(new MyVector2D(null, null, Fnet, _fN.theta_in_radians));

        //    return _res;
        //}

        //public MyVector2D ApplyDynamicOld(double acc_degree)
        //{
        //    UNIT curr_unit = mind.curr_unit;
            
        //    if (curr_unit.IsNull())
        //        throw new Exception("ApplyDynamic");

        //    double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
        //    double angle_dyn = 90.0d + acc_degree_positive;

        //    double max = HighestVar;
        //    double force_dyn = max - curr_unit.Variable;

        //    MyVector2D calc = new MyVector2D();
        //    MyVector2D dynamic = new MyVector2D(null, null, force_dyn, calc.ToRadians(angle_dyn));

        //    double m = 0.5d;
        //    double u = FrictionOld(false, curr_unit.credits, mind.parms.shift);
        //    double N = m * Constants.GRAVITY;

        //    double Ffriction = u * N;
        //    double Fapplied = dynamic.magnitude;
        //    double Fnet = Fapplied - Ffriction;

        //    if (Fnet <= Constants.VERY_LOW)
        //        Fnet = Constants.VERY_LOW;
            
        //    MyVector2D _res = calc.ToCart(new MyVector2D(null, null, Fnet, calc.ToRadians(angle_dyn)));

        //    return _res;
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

        //    return friction / 2.0d;
        //}
    }
}

