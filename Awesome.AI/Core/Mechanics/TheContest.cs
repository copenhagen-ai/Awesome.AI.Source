using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Awesome.AI.Helpers.Enums;

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
        public double fri_dv { get; set; } = 0.0d;
        public double vel_dv { get; set; } = 0.0d;
        public double out_high { get; set; } = -1000.0d;
        public double out_low { get; set; } = 1000.0d;
        public double posx_high { get; set; } = -1000.0d;
        public double posx_low { get; set; } = 1000.0d;
        public double res_x { get; set; } = -1.0d;
        private double min { get; set; } = 1000.0d;
        private double max { get; set; } = -1000.0d;

        public double POS_X { get; set; } = 10.0d;
        public Direction dir { get; set; }
        public Limitters lim { get; set; }

        TheMind mind;
        private _TheContest() { }
        public _TheContest(Params parms)
        {
            this.mind = parms.mind;
            dir = new Direction(parms.mind) { d_momentum = 0.0d };
            lim = new Limitters(parms.mind);
            this.mind = parms.mind;
        }

        public double EXIT()
        {
            double res = POS_X;

            return res;
        }

        public void XPOS()
        {
            //its a hack, yes its cheating..
            double boost = mind.parms.boost;

            POS_X = 10.0d + (boost * momentum);//dosnt seem right
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


        public void CALC()
        {
            bool reset = velocity >= 0.0d; //maybe 0.666 * max_velocity

            //if(reset)
            //    velocity = 0.0d;

            //if (reset)  //car left
            fri_dv = ApplyStatic();

            //if(true)    //car right
            vel_dv = ApplyDynamic();

            //momentum: p = m * v
            momentum = (mind.parms.mass * 2) * velocity;

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
            double acc = mind.common.HighestForce().Variable;
            double limit = lim.Limit(true, () => dir.SayNo());

            double m = mind.parms.mass;
            double F = m * acc;                       //force, left
            double dt = DeltaT();                   //delta time
            double dv = DeltaV(F, m, dt) * limit;   //delta velocity

            velocity -= dv;

            return dv;
        }

        /*
         * car right
         * */
        public double ApplyDynamic()
        {
            UNIT curr_unit_th = mind.curr_unit;
            THECHOISE goodbye = mind.goodbye;

            if (curr_unit_th.IsNull())
                throw new Exception();

            bool first_run = false;
            if (mind.cycles_all <= mind.parms.first_run)
                first_run = true;

            double max = mind.common.HighestForce().Variable;
            double acc = max - curr_unit_th.Variable;
            double limit = first_run ? 0.5d : lim.Limit(false, () => dir.SayNo());

            double m = mind.parms.mass;
            double F = m * acc;                       //force, right
            double dt = DeltaT();                   //delta time
            double dv = DeltaV(F, m, dt) * limit;   //delta velocity

            if (goodbye.IsNo())
                //if (goodbye.IsNo() && momentum < 0.0d)
                velocity += dv;

            return dv;
        }/**/

        private double DeltaV(double F, double m, double dt)
        {
            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
            double dv = (F * dt) / m;
            return dv;
        }

        private double DeltaT()
        {
            //most of the time this is true

            double x = mind.parms.micro_sec;
            double dt = x / 1000000.0d;
            return dt;
        }

        private void Reset(TheMind mind)
        {
            if (mind.cycles_all % 25000 != 0)
                return;

            out_low *= 0.5d;
            out_high *= 0.5d;

            posx_low *= 0.5d;
            posx_high *= 0.5d;

            min *= 0.5d;
            max *= 0.5d;
        }
    }
}

