using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using Awesome.AI.Web.AI.Common;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class _TheHill : IMechanics
    {
        public double velocity { get; set; } = 0.0d;
        public double momentum { get; set; } = 0.0d;
                
        public double fri_dv { get; set; } = 0.0d;
        public double vel_dv { get; set; } = 0.0d;
        public double out_high { get; set; } = -1000.0d;
        public double out_low { get; set; } = 1000.0d;
        public double posx_high { get; set; } = -1000.0d;
        public double posx_low { get; set; } = 1000.0d;
        
        private double a { get { return mind.parms.val_a; } }//-0.1d;
        private double b { get { return mind.parms.val_b; } }//0.0d;
        private double c { get { return mind.parms.val_c; } }//10.0d;
        public double res_x { get; set; } = 0;
        public double res_x_prev { get; set; } = 0;

        public double POS_X { get; set; }
        public Direction dir { get; set; }
        public Limitters lim { get; set; }

        TheMind mind;
        private _TheHill() { }
        public _TheHill(Params parms)
        {
            this.mind = parms.mind;
            dir = new Direction(parms.mind) { d_momentum = 0.0d };
            lim = new Limitters(parms.mind);

            POS_X = parms.pos_x_start;//10;
        }
        
        private int count { get; set; } = 0;
        public double EXIT()
        {
            double _x = POS_X;
            double res = POS_X;

            //this is weird!!!
            double _div = mind.parms.pos_x_start;// * Params.boost;
            count = _x >= _div ? ++count : 0;
            res = count >= 1000 ? 0.0d : res;

            return res;
        }

        public void XPOS()
        {
            //its a hack, yes its cheating..
            double boost = mind.parms.boost;

            POS_X = mind.parms.pos_x_start + (boost * momentum);
            //POS_X += (boost * momentum);
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

        private double SlopeInDegrees(double x, out bool is_right)
        {
            double acc_slope, _x, _y;

            Vector2D calc = new Vector2D();
            Vector2D _slope;
            
            acc_slope = mind.calc.SlopeCoefficient(x, a, b);
            is_right = acc_slope <= 0.0d;
            _x = is_right ? 1.0d : -1.0d;
            _y = acc_slope;
            _slope = calc.ToPolar(new Vector2D(_x, _y, null, null));
            double acc_degree = _slope.theta_in_degrees;

            return acc_degree;
        }
        
        public void CALC()
        {
            Check(a, b, c);

            Vector2D calc = new Vector2D();
            Vector2D res, sta, dyn = calc.ToCart(new Vector2D(null, null, 0.0d, 0.0d));

            res_x_prev = mind.goodbye.IsNo() ? mind.parms.pos_x_start + res_x : res_x_prev;
            double acc_degree = SlopeInDegrees(res_x_prev, out bool is_right);
            sta = ApplyStatic(acc_degree, is_right);
            if(mind.goodbye.IsNo())
                dyn = ApplyDynamic(acc_degree, is_right);

            res = calc.Add(sta, dyn);
            res_x = res.xx;
            res_x_prev = res_x_prev + res_x;
            res = calc.ToPolar(res);

            double acceleration = res.yy < 0.0d ? res.magnitude : -res.magnitude;
            //double acceleration = res.theta_in_degrees < 0.0d ? res.magnitude : -res.magnitude;
            //double acceleration = dyn.magnitude > sta.magnitude ? -res.magnitude : res.magnitude;
            //double acceleration = sta.magnitude - dyn.magnitude;
            //double acceleration = res.magnitude;

            double m = mind.parms.mass;
            double dt = DeltaT();
            double dv = DeltaV(acceleration, dt);

            //momentum: p = m * v
            momentum = m * dv;
            //momentum += m * velocity;
            //momentum += m * dv;
            //velocity += dv;

            if (momentum <= out_low) out_low = momentum;
            if (momentum > out_high) out_high = momentum;
        }

        public Vector2D ApplyStatic(double acc_degree, bool is_right)
        {
            double limit = lim.Limit(true, () => dir.SayNo());

            double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
            double angle_sta = -90.0d;
            double angle_com_y_vec = is_right ? -90.0d - acc_degree_positive : -90.0d + acc_degree_positive;//-135
            double angle_com_y_pyth = 90.0d - acc_degree_positive;//-135

            double force_sta = mind.common.HighestForce().Variable;
            double force_com_y = mind.calc.PythNear(angle_com_y_pyth, force_sta);

            Vector2D calc = new Vector2D();
            Vector2D _static = calc.ToCart(calc.Flip360(new Vector2D(null, null, force_sta, mind.calc.ToRadiansFromDegrees(angle_sta))));
            Vector2D _N = calc.ToCart(new Vector2D(null, null, force_com_y, mind.calc.ToRadiansFromDegrees(angle_com_y_vec + 180.0d)));
            Vector2D _fN = calc.ToPolar((calc.Add(_static, _N)));

            double force = _fN.magnitude * limit;

            Vector2D _res = calc.ToCart(calc.Flip360(new Vector2D(null, null, force, _fN.theta_in_radians)));
            
            return _res;
        }

        public Vector2D ApplyDynamic(double acc_degree, bool is_right)
        {
            UNIT curr_unit_th = mind.curr_unit;
            THECHOISE goodbye = mind.goodbye;
            
            if (curr_unit_th.IsNull())
                throw new Exception();

            bool first_run = mind.cycles_all <= mind.parms.first_run;
            double limit = first_run ? 0.1d : lim.Limit(false, () => dir.SayNo());

            double acc_degree_positive = acc_degree < 0.0d ? -acc_degree : acc_degree;
            double angle_dyn = is_right ? 90.0d + acc_degree_positive : 90.0d - acc_degree_positive;

            double max = mind.common.HighestForce().Variable;
            double force_dyn = max - curr_unit_th.Variable;

            Vector2D calc = new Vector2D();
            Vector2D _dynamic = new Vector2D(null, null, force_dyn, mind.calc.ToRadiansFromDegrees(angle_dyn));

            double force = goodbye.IsNo() ? _dynamic.magnitude * limit : 0.0d;
            
            Vector2D _res = calc.ToCart(new Vector2D(null, null, force, mind.calc.ToRadiansFromDegrees(angle_dyn)));

            return _res;
        }/**/

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

            double x = mind.parms.micro_sec;
            double dt = x / 1000000.0d;
            return dt;
        }

        //private void Reset(TheMind mind)
        //{
        //    if (mind.cycles_all % 50000 == 0) { out_low = 1000.0d; out_high = -1000.0d; }
        //    if (mind.cycles_all % 50000 == 0) { posx_low = 1000.0d; posx_high = -1000.0d; }
        //    if (mind.cycles_all % 50000 == 0) { min = 1000.0d; max = -1000.0d; }
        //}

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

