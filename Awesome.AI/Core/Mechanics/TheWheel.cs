using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using System;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class _TheWheel : IMechanics
    {
        /*
         * >> THE HACK <<
         * -  gaspedal/wheel analogy in docs
         * */
        
        public double velocity { get; set; } = 0.0d;

        public double momentum { get; set; } = -1000.0d;
        
        public double fri_dv { get; set; } = 0.0d;
        public double vel_dv { get; set; } = 0.0d;
        public double out_high { get; set; } = 0.0d;
        public double out_low { get; set; } = 0.0d;
        public double posx_high { get; set; } = -1000.0d;
        public double posx_low { get; set; } = 1000.0d;
        public double res_x { get; set; } = -1.0d;
        
        public double POS_X { get; set; } = 10.0d;
        public Direction dir { get; set; }
        public Limitters lim { get; set; }

        TheMind mind;
        private _TheWheel() { }
        public _TheWheel(Params parms)
        {
            this.mind = parms.mind;
            dir = new Direction(parms.mind) { d_momentum = 0.0d };
            lim = new Limitters(parms.mind);
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
            //POS_X += (boost * velocity);

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
            //force left
            fri_dv = ApplyStatic();
            //force right
            vel_dv = ApplyDynamic();

            //momentum: p = m * v
            momentum = (mind.parms.mass) * velocity;

            if (momentum < 0.0d && momentum <= out_low) out_low = momentum;
            if (momentum > 0.0d && momentum > out_high) out_high = momentum;

            if (double.IsNaN(velocity))
                throw new Exception();
        }

        /*
         * friction
         * */
        public double ApplyStatic()
        {
            double force = mind.common.HighestForce().Variable;
            double limit = lim.Limit(true, () => dir.SayNo());

            double F = force;//force, left
            double m = mind.parms.mass;
            double dt = DeltaT();
            double dv = DeltaV(F, m, dt);

            velocity -= dv * limit;

            //if (velocity < -1.0d)
            //    throw new Exception();

            return dv;
        }

        /*
         * applied force
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
            double force = curr_unit_th.Variable;
            double limit = first_run ? 0.5d : lim.Limit(false, () => dir.SayNo());

            double F = max - force;//force, right
            double m = mind.parms.mass;
            double dt = DeltaT();
            double dv = DeltaV(F, m, dt);

            /*
                * maybe its a "fake it till you make it", situation??
                * maybe this is actually the core of the project. and the task is to make this limit as smooth as possible
                * maybe making it layered somehow, use chance, fuzzy?
                * maybe a gradient of somesort
                * */

            if (goodbye.IsNo())
                velocity += dv * limit;                                 // maybe this does actually make the "whoosh" effect over time/cycles

            //if (velocity > 1.0d)
            //    throw new Exception();

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
    }
}

