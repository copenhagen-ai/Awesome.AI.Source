using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using Awesome.AI.Web.AI.Common;

namespace Awesome.AI.Core.Mechanics
{
    public class _TheHill : IMechanics
    {
        public double velocity { get; set; } = 0.0d;
        public double momentum { get; set; } = 0.0d;
                
        public double Fsta { get; set; } = 0.0d;
        public double Fdyn { get; set; } = 0.0d;
        public double out_high { get; set; } = -1000.0d;
        public double out_low { get; set; } = 1000.0d;
        public double posx_high { get; set; } = -1000.0d;
        public double posx_low { get; set; } = 1000.0d;
        
        //private double a { get { return mind.parms.hill_a; } }//-0.1d;
        //private double b { get { return mind.parms.hill_b; } }//0.0d;
        //private double c { get { return mind.parms.hill_c; } }//10.0d;
        public double res_x_prev { get; set; } = 0;

        public double POS_X { get; set; }
        public Direction dir { get; set; }

        TheMind mind;
        private _TheHill() { }
        public _TheHill(Params parms)
        {
            this.mind = parms.mind;
            dir = new Direction(parms.mind) { d_momentum = 0.0d };
            
            POS_X = parms.pos_x_start;//10;
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
                throw new Exception();

            if (_c.IsIDLE())
                throw new Exception();// return Params.idle_val;

            double earth_gravity = Constants.GRAVITY;
            double mass = mind.parms.mass;
            double percent = mind.calc.NormalizeRange(_c.HighAtZero, 0.0d, mind.parms.max_index, 0.0d, 1.0d);

            double res = (mass * earth_gravity) * percent;

            return res;
        }

        public double Result()
        {
            double res = 10.0d - POS_X;

            return res;
        }

        public void Position()
        {
            //its a hack, yes its cheating..
            double boost = mind.parms.boost;

            if(mind.goodbye.IsNo())
                POS_X = mind.parms.pos_x_start + (boost * momentum);
            else
                POS_X += (boost * momentum);

            //POS_X = 10.0d + (boost * momentum);

            if (POS_X < mind.parms.pos_x_low)
                POS_X = mind.parms.pos_x_low;
            if (POS_X > mind.parms.pos_x_high)
                POS_X = mind.parms.pos_x_high;

            if (POS_X <= posx_low) posx_low = POS_X;
            if (POS_X > posx_high) posx_high = POS_X;

            dir.d_momentum = momentum;
            dir.d_pos_x = POS_X;
            
            dir.Stat();
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

            double res_x_save = mind.parms.pos_x_start + res_x_prev;
            double acc_degree = SlopeInDegrees(res_x_save);

            sta = ApplyStatic(acc_degree);
            
            if (mind.goodbye.IsNo())
                dyn = ApplyDynamic(acc_degree);

            res = calc.Add(sta, dyn);
            res_x_prev = res.xx;
            res = calc.ToPolar(res);

            double acc = res.yy < 0.0d ? res.magnitude : -res.magnitude;
            //double acc = res.theta_in_degrees < 0.0d ? res.magnitude : -res.magnitude;
            //double acc = dyn.magnitude > sta.magnitude ? -res.magnitude : res.magnitude;
            //double acc = sta.magnitude - dyn.magnitude;
            //double acc = res.magnitude;

            double m = mind.parms.mass;
            double dt = DeltaT();
            double dv = DeltaV(acc, dt);

            //momentum: p = m * v
            momentum = m * dv;
            //momentum += m * velocity;
            //momentum += m * dv;
            //velocity += dv;

            if (momentum <= out_low) out_low = momentum;
            if (momentum > out_high) out_high = momentum;
        }

        public Vector2D ApplyStatic(double acc_degree)
        {
            double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
            double angle_sta = -90.0d;
            double angle_com_y_vec = -90.0d - acc_degree_positive;//-135
            double angle_com_y_pyth = 90.0d - acc_degree_positive;//-135

            double force_sta = mind.common.HighestForce().Variable;
            double force_com_y = mind.calc.PythNear(angle_com_y_pyth, force_sta);

            Vector2D calc = new Vector2D();
            Vector2D _static = calc.ToCart(calc.Flip360(new Vector2D(null, null, force_sta, mind.calc.ToRadiansFromDegrees(angle_sta))));
            Vector2D _N = calc.ToCart(new Vector2D(null, null, force_com_y, mind.calc.ToRadiansFromDegrees(angle_com_y_vec + 180.0d)));
            Vector2D _fN = calc.ToPolar((calc.Add(_static, _N)));

            double m = mind.parms.mass;
            double u = mind.calc.FrictionCoefficient(true, 0.0d);
            double N = m * Constants.GRAVITY;

            double Ffriction = u * N;
            double Fapplied = _fN.magnitude;
            double Fnet = Fapplied - Ffriction;

            Vector2D _res = calc.ToCart(calc.Flip360(new Vector2D(null, null, Fnet, _fN.theta_in_radians)));
            
            return _res;
        }

        public Vector2D ApplyDynamic(double acc_degree)
        {
            UNIT curr_unit_th = mind.curr_unit;
                        
            if (curr_unit_th.IsNull())
                throw new Exception();

            double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
            double angle_dyn = 90.0d + acc_degree_positive;

            double max = mind.common.HighestForce().Variable;
            double force_dyn = max - curr_unit_th.Variable;

            Vector2D calc = new Vector2D();
            Vector2D _dynamic = new Vector2D(null, null, force_dyn, mind.calc.ToRadiansFromDegrees(angle_dyn));

            double m = mind.parms.mass;
            double u = mind.calc.FrictionCoefficient(false, curr_unit_th.credits);
            double N = m * Constants.GRAVITY;

            double Ffriction = u * N;
            double Fapplied = _dynamic.magnitude;
            double Fnet = Fapplied - Ffriction;
            
            Vector2D _res = calc.ToCart(new Vector2D(null, null, Fnet, mind.calc.ToRadiansFromDegrees(angle_dyn)));

            return _res;
        }

        private double DeltaV(double a, double dt)
        {
            //F=m*a
            //a=dv/dt
            //dv=a*dt
            double dv = a * dt;
            return dv;
        }

        private double DeltaT()
        {
            //most of the time this is true

            return 0.002d;
        }

        private bool Check(double _a, double _b, double _c)
        {
            double _x1, _x2;
            mind.calc.Roots(null, _a, _b, _c, out _x1, out _x2);

            bool ok = _x1 == -10.0d & _x2 == 10.0d;

            if (!ok)
                throw new Exception();

            return ok;
        }
    }
}

