using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class Hill : IMechanics
    {
        public double p_curr { get; set; }
        public double p_prev { get; set; }
        public double p_delta { get; set; }
        public double p_delta_prev {  get; set; }

        public double m_out_high { get; set; }
        public double m_out_low { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        
        private TheMind mind;
        private Hill() { }
        public Hill(TheMind mind, Params parms)
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
                double boost = mind.parms.boost;

                //if (mind.goodbye.IsNo())
                //    posxy = Constants.STARTXY + (boost * momentum);
                //else
                //    posxy += (boost * momentum);

                if (mind.goodbye.IsNo())
                    posxy = Constants.STARTXY + (boost * p_delta);
                else
                    posxy += (boost * p_delta);

                //POS_X = 10.0d + (boost * momentum);

                if (posxy < Constants.LOWXY)
                    posxy = Constants.LOWXY;
                if (posxy > Constants.HIGHXY)
                    posxy = Constants.HIGHXY;

                if (posxy <= posx_low) posx_low = posxy;
                if (posxy > posx_high) posx_high = posxy;

                return 10.0d - posxy;
            }
        }

        public double Variable(UNIT _c)
        {
            if (_c.IsNull())
                throw new Exception("Variable");

            if (_c.IsIDLE())
                throw new Exception("Variable");

            double _var = _c.HighAtZero;

            return _var;
        }

        private double velocity = 0.0;
        private double timeStep = 0.0;
        private double x = 0.0;
        public void CalcPattern1(MECHVERSION version, int cycles)
        {
            if (version != MECHVERSION.MOODGENERAL)
                return;

            if (cycles == 1)
                Reset1();

            // Constants
            double a = 0.1d;                    // Parabola coefficient (hill steepness)
            double g = Constants.GRAVITY;       // Gravity (m/s^2)
            double F0 = 5.0d;                   // Wind force amplitude (N)
            double omega = 2 * Math.PI * 0.5d;  // Wind frequency
            double beta = 0.02d;                // Friction coefficient
            double dt = 0.1d;                   // Time step (s)
            double noiseAmplitude = 0.5d;       // Random noise amplitude for wind force
            double m = 0.5d;                    // Ball mass (kg)
            
            double t = cycles * dt;

            // Compute forces
            double Fx = F0 * (Math.Sin(omega * t) + GetRandomNoise1(noiseAmplitude)); // Wind force
            double slope = 2 * a * x; // Slope dy/dx
            double sinTheta = slope / Math.Sqrt(1 + slope * slope);
            double gravityForce = m * g * sinTheta;
            double frictionForce = -beta * velocity;

            // Compute acceleration along the tangent
            double a_tangent = (gravityForce + Fx + frictionForce) / m;

            // Update velocity and position
            velocity += a_tangent * dt;
            x += velocity * dt;

            p_prev = p_curr;
            p_curr = m * velocity;
            p_delta = p_curr - p_prev;

            if (p_curr <= m_out_low) m_out_low = p_curr;
            if (p_curr > m_out_high) m_out_high = p_curr;

            if (p_delta <= d_out_low) d_out_low = p_delta;
            if (p_delta > d_out_high) d_out_high = p_delta;
        }

        private void Reset1()
        {
            int _rand = mind.rand.MyRandomInt(1, 5)[0];
            bool rand_sample = _rand > 4;
            if (!rand_sample) return;

            x = 2.5;
            timeStep = 0.0d;
            velocity = 0.0d;
            p_curr = 0.0d;
            p_delta = 0.0d;
            p_prev = 0.0d;
        }

        private double GetRandomNoise1(double noiseAmplitude)
        {
            UNIT curr_unit = mind.curr_unit;

            if (curr_unit == null)
                throw new Exception("ApplyDynamic");

            double _var = curr_unit.Variable;

            double rand = mind.calc.NormalizeRange(_var, 0.0d, 100.0d, -1.0d, 1.0d);
            
            return rand * noiseAmplitude;
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

