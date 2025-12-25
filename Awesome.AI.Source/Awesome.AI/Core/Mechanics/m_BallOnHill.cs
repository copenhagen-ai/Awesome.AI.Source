using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class m_BallOnHill : IMechanics
    {
        public MechParams mp { get; set; }
        public MechHelper mh { get; set; }

        private TheMind mind;
        private m_BallOnHill() { }
        public m_BallOnHill(TheMind mind, PROPS props)
        {
            this.mind = mind;

            this.mh = new MechHelper() { };

            this.mp = new MechParams() { };

            this.mp.props = new Properties(mind, props);

            // Constants
            mp.a = 0.1d;                    // Parabola coefficient (hill steepness)
            mp.g = CONST.GRAVITY;           // Gravity (m/s^2)
            mp.F0 = 5.0d;                   // Wind force amplitude (N)
            mp.omega = Math.PI;             // Wind frequency
            mp.beta = 0.02d;                // Friction coefficient
            mp.dt = 0.1d;                   // Time step (s)
            mp.eta = 0.5d;                  // Random noise amplitude for wind force
            mp.m1 = 0.35d;                  // Ball mass (kg)

            mp.vv_out_high = -1000.0d;
            mp.vv_out_low = 1000.0d;
            mp.dv_out_high = -1000.0d;
            mp.dv_out_low = 1000.0d;
            mp.posx_high = -1000.0d;
            mp.posx_low = 1000.0d;
        }

        public double POS_XY
        {
            get
            {
                double posxy = mh.PosXY(mind, mp);

                return posxy;
            }
        }        

        public void Peek(UNIT _c)
        {
            throw new NotImplementedException();
        }
        
        private void Calc(int cycles)
        {
            double t = cycles * mp.dt;

            // Compute forces
            double Fx = ApplyDynamic(mp, t); // Wind force
            double Fgravity = ApplyStatic(mp);
            double Ffriction = -mp.beta * mp.velocity;
            double Fnet = Fgravity + Fx + Ffriction;

            // Compute acceleration along the tangent
            double a_tangent = Fnet / mp.m1;

            // Update velocity and position
            mp.velocity += a_tangent * mp.dt;
            mp.pos_x += mp.velocity * mp.dt;

            mp.vv_prev = mp.vv_curr;
            mp.vv_curr = /*mp.m1 * */mp.velocity;
            mp.dv_curr = mp.vv_curr - mp.vv_prev;

            if (double.IsNaN(mp.cc_elec_curr) || double.IsNaN(mp.cc_elec_prev) || double.IsNaN(mp.dc_elec_curr) || double.IsNaN(mp.dc_elec_prev))
                throw new Exception("m_BallOnHill, Calc");
        }

        private double ApplyStatic(MechParams mp)
        {
            double slope = 2 * mp.a * mp.pos_x; // Slope dy/dx
            double sinTheta = slope / Math.Sqrt(1 + slope * slope);
            double Fgravity = -(mp.m1 * mp.g) * sinTheta;

            return Fgravity;
        }

        private double ApplyDynamic(MechParams mp, double t)
        {
            if(mind.goodbye)
                return 0.0d;

            double Fx = mp.F0 * mh.Sine(mp.pattern_curr, t, mp.omega, 0.0d, 1.0d, 0.5d, 0.5d, 0.0d, 0.5d) + mh.GetRandomNoise(mind, mp.eta); // Wind force

            if (Fx < 0.0d)
                Fx = 0.0d;

            if (double.IsNaN(Fx))
                throw new Exception("m_BallOnHill, ApplyDynamic");

            return Fx;
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

            Calc(cycles);

            mh.Extremes(mp);
            mh.Normalize(mind, mp);
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

