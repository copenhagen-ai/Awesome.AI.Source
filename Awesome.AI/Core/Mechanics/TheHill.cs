using Awesome.AI.Common;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using Awesome.AI.Web.AI.Common;

namespace Awesome.AI.Core.Mechanics
{
    public class _TheHill : IMechanics
    {
        //public double velocity { get; set; } = 0.0d;
        public double momentum { get; set; } = 0.0d;
        public double deltaMom { get; set; } = 0.0d;

        public double Fsta { get; set; } = 0.0d;
        public double Fdyn { get; set; } = 0.0d;
        public double m_out_high { get; set; } = -1000.0d;
        public double m_out_low { get; set; } = 1000.0d;
        public double d_out_high { get; set; } = -1000.0d;
        public double d_out_low { get; set; } = 1000.0d;
        public double posx_high { get; set; } = -1000.0d;
        public double posx_low { get; set; } = 1000.0d;
        
        public double res_x { get; set; } = 5.0d;
                
        private TheMind mind;
        private _TheHill() { }
        public _TheHill(TheMind mind, Params parms)
        {
            this.mind = mind;
            
            posxy = Constants.STARTXY;//10;
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
                    posxy = Constants.STARTXY + (boost * deltaMom);
                else
                    posxy += (boost * deltaMom);

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

        //Weight
        public double Variable(UNIT _c)
        {
            /*
             * This is a changeable function.
             * 
             * Weight
             * W = m * g
             * */
            if (_c.IsNull())
                throw new Exception("Variable");

            if (_c.IsIDLE())
                throw new Exception("Variable");

            double earth_gravity = Constants.GRAVITY;
            double mass = mind.parms.mass;
            
            double res = (mass * earth_gravity) * (_c.HighAtZero / 100.0d);

            return res;
        }

        private double SlopeInDegrees(double x)
        {
            double acc_slope, _x, _y;

            Vector2D calc = new Vector2D();
            Vector2D _slope;
            
            acc_slope = mind.calc.SlopeCoefficient(x, Vars.var_a, Vars.var_b);
            _x = 1.0d;
            _y = acc_slope;
            _slope = calc.ToPolar(new Vector2D(_x, _y, null, null));
            double acc_degree = _slope.theta_in_degrees;
                        
            return acc_degree;
        }
        
        public void Calculate()
        {
            Check(Vars.var_a, Vars.var_b, Vars.var_c);

            Vector2D calc = new Vector2D();
            Vector2D res, sta = new Vector2D(), dyn = new Vector2D();

            //double res_x_save = Constants.STARTXY + res_x_prev;
            double acc_degree = SlopeInDegrees(res_x);

            sta = ApplyStatic(acc_degree);
            
            if (mind.goodbye.IsNo())
                dyn = ApplyDynamic(acc_degree);

            res = calc.Add(sta, dyn);
            res_x = res.xx;
            res = calc.ToPolar(res);

            if (res_x < 0.0d) res_x = 0.0d;
            if (res_x > 10.0d) res_x = 10.0d;

            double acc = res.yy < 0.0d ? res.magnitude : -res.magnitude;

            double m = mind.parms.mass;
            //double dt = mind.parms.delta_time;
            double deltaT = mind.parms.delta_time;

            //F=m*a
            //a=dv/dt
            //dv=a*dt
            //double dv = acc * dt;
            double deltaVel = acc * deltaT;
            
            deltaMom = m * deltaVel;
            momentum += deltaMom;

            if (momentum <= m_out_low) m_out_low = momentum;
            if (momentum > m_out_high) m_out_high = momentum;

            if (deltaMom <= d_out_low) d_out_low = deltaMom;
            if (deltaMom > d_out_high) d_out_high = deltaMom;

            //double acc = res.theta_in_degrees < 0.0d ? res.magnitude : -res.magnitude;
            //double acc = dyn.magnitude > sta.magnitude ? -res.magnitude : res.magnitude;
            //double acc = sta.magnitude - dyn.magnitude;
            //double acc = res.magnitude;

            //velocity = dv;
            //momentum: p = m * v
            //momentum += m * velocity;
            //momentum += m * dv;
            //velocity += dv;
        }

        //private double shift = -3.0d;
        public Vector2D ApplyStatic(double acc_degree)
        {
            double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
            double angle_sta = -90.0d;
            double angle_com_y_vec = -90.0d - acc_degree_positive;//-135
            double angle_com_y_pyth = 90.0d - acc_degree_positive;//-135

            double force_sta = mind.common.HighestForce().Variable;
            double force_com_y = mind.calc.PythNear(angle_com_y_pyth, force_sta);

            Vector2D calc = new Vector2D();
            //Vector2D _static = calc.ToCart(calc.Flip360(new Vector2D(null, null, force_sta, mind.calc.ToRadiansFromDegrees(angle_sta))));
            Vector2D _static = calc.ToCart(new Vector2D(null, null, force_sta, calc.ToRadians(angle_sta)));
            Vector2D _N = calc.ToCart(new Vector2D(null, null, force_com_y, calc.ToRadians(angle_com_y_vec + 180.0d)));
            Vector2D _fN = calc.ToPolar((calc.Add(_static, _N)));

            double m = mind.parms.mass;
            double u = mind.core.LimitterFriction(true, 0.0d, mind.parms.shift);
            double N = m * Constants.GRAVITY;

            double Ffriction = u * N;
            double Fapplied = _fN.magnitude;
            double Fnet = Fapplied - Ffriction;

            //Vector2D _res = calc.ToCart(calc.Flip360(new Vector2D(null, null, Fnet, _fN.theta_in_radians)));
            Vector2D _res = calc.ToCart(new Vector2D(null, null, Fnet, _fN.theta_in_radians));

            return _res;
        }

        public Vector2D ApplyDynamic(double acc_degree)
        {
            UNIT curr_unit_th = mind.curr_unit;
                        
            if (curr_unit_th.IsNull())
                throw new Exception("ApplyDynamic");

            double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
            double angle_dyn = 90.0d + acc_degree_positive;

            double max = mind.common.HighestForce().Variable;
            double force_dyn = max - curr_unit_th.Variable;

            Vector2D calc = new Vector2D();
            Vector2D _dynamic = new Vector2D(null, null, force_dyn, calc.ToRadians(angle_dyn));

            double m = mind.parms.mass;
            double u = mind.core.LimitterFriction(false, curr_unit_th.credits, mind.parms.shift);
            double N = m * Constants.GRAVITY;

            double Ffriction = u * N;
            double Fapplied = _dynamic.magnitude;
            double Fnet = Fapplied - Ffriction;
            
            Vector2D _res = calc.ToCart(new Vector2D(null, null, Fnet, calc.ToRadians(angle_dyn)));

            return _res;
        }

        //private double DeltaV(double a, double dt)
        //{
        //    //F=m*a
        //    //a=dv/dt
        //    //dv=a*dt
        //    double dv = a * dt;
        //    return dv;
        //}

        //private double DeltaT()
        //{
        //    //most of the time this is true

        //    return 0.002d;
        //}

        private bool Check(double _a, double _b, double _c)
        {
            double _x1, _x2;
            mind.calc.Roots(null, _a, _b, _c, out _x1, out _x2);

            bool ok = _x1 == -10.0d & _x2 == 10.0d;

            if (!ok)
                throw new Exception("Check");

            return ok;
        }
    }
}

