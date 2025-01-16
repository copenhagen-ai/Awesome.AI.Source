using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;

namespace Awesome.AI.Core.Mechanics
{
    public class _TheContest : IMechanics
    {
        /*
         * >> THE HACK <<
         * -  gaspedal/wheel analogy in docs
         * */

        public double velocity { get; set; } = 0.0d;
        public double momentum { get; set; } = 0.0d;
        public double limit_result { get; set; } = 0.0d;
        public double learn_result { get; set; } = 0.0d;
        public double Fsta { get; set; } = 0.0d;
        public double Fdyn { get; set; } = 0.0d;
        public double out_high { get; set; } = -1000.0d;
        public double out_low { get; set; } = 1000.0d;
        public double posx_high { get; set; } = -1000.0d;
        public double posx_low { get; set; } = 1000.0d;
        public double res_x { get; set; } = -1.0d;
        private double min { get; set; } = 1000.0d;
        private double max { get; set; } = -1000.0d;

        public double POS_X { get; set; } = 10.0d;
        public Direction dir { get; set; }
        
        TheMind mind;
        private _TheContest() { }
        public _TheContest(Params parms)
        {
            this.mind = parms.mind;
            this.dir = new Direction(parms.mind) { d_momentum = 0.0d };
            this.mind = parms.mind;
        }

        //NewtonForce
        public double Variable(UNIT curr)
        {
            /*
             * I guess this is a changeable function, for now it is just the one I know to calculate force
             * */

            if (curr.IsNull())
                throw new Exception();

            if (curr.IsIDLE())
                throw new Exception();

            double acc = curr.HighAtZero;
            acc = acc == 0.0d ? Constants.VERY_LOW : acc;// jajajaa

            return acc;
        }        

        public double Result()
        {
            double res = POS_X;

            return res;
        }

        public void Position()
        {
            //its a hack, yes its cheating..
            double boost = mind.goodbye.IsNo() ? mind.parms.boost : 1.0d;

            POS_X = 10.0d + (boost * momentum);//dosnt seem right
            //POS_X += (boost * momentum);
            //POS_X = POS_X + (boost * velocity);
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

        public void Calculate()
        {
            //car left
            Fsta = ApplyStatic();

            //car right
            Fdyn = ApplyDynamic();

            double Fnet = mind.goodbye.IsNo() ? -Fsta + Fdyn : -Fsta;
            double dt = 0.002d; //delta time, 1sec/500cyc
            double m = mind.parms.mass;

            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
            double dv = (Fnet * dt) / m;

            velocity += dv;

            //momentum: p = m * v
            momentum = (m * 2) * velocity;

            if (momentum <= out_low) out_low = momentum;
            if (momentum > out_high) out_high = momentum;

            if (double.IsNaN(velocity))
                throw new Exception();
        }

        /*
         * car left
         * */
        public double ApplyStatic()
        {
            double acc = mind.common.HighestForce().Variable / 10; //divided by 10 for aprox acc
            double m = mind.parms.mass;
            double u = mind.core.FrictionCoefficient(true, 0.0d);
            double N = m * Constants.GRAVITY;

            double Ffriction = u * N;
            double Fapplied = m * acc; //force, left
            double Fnet = Fapplied - Ffriction;

            if (Fnet <= 0.0d)
                Fnet = 0.0d;

            return Fnet;
        }

        /*
         * car right
         * */
        public double ApplyDynamic()
        {
            UNIT curr_unit_th = mind.curr_unit;

            if (curr_unit_th.IsNull())
                throw new Exception();

            double max = mind.common.HighestForce().Variable / 10; //divided by 10 for aprox acc
            double acc = max - curr_unit_th.Variable / 10; //divided by 10 for aprox acc
            double m = mind.parms.mass;
            double u = mind.core.FrictionCoefficient(false, curr_unit_th.credits);
            double N = m * Constants.GRAVITY;

            double Ffriction = u * N;
            double Fapplied = m * acc; //force, left
            double Fnet = Fapplied - Ffriction;

            if (Fnet <= 0.0d)
                Fnet = 0.0d;

            return Fnet;
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

