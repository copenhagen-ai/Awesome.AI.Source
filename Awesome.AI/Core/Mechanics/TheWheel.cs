//using Awesome.AI.Common;
//using Awesome.AI.Helpers;
//using Awesome.AI.Interfaces;

//namespace Awesome.AI.Core.Mechanics
//{
//    public class _TheWheel : IMechanics
//    {
//        /*
//         * >> THE HACK <<
//         * -  gaspedal/wheel analogy in docs
//         * */
        
//        public double velocity { get; set; } = 0.0d;

//        public double momentum { get; set; } = -1000.0d;
        
//        public double Fsta { get; set; } = 0.0d;
//        public double Fdyn { get; set; } = 0.0d;
//        public double out_high { get; set; } = 0.0d;
//        public double out_low { get; set; } = 0.0d;
//        public double posx_high { get; set; } = -1000.0d;
//        public double posx_low { get; set; } = 1000.0d;
//        public double res_x { get; set; } = -1.0d;
        
//        private TheMind mind;
//        private _TheWheel() { }
//        public _TheWheel(TheMind mind, Params parms)
//        {
//            this.mind = mind;

//            posxy = Constants.STARTXY;//10;
//        }


//        private double posxy { get; set; }
//        public double POS_XY
//        {
//            get
//            {
//                //its a hack, yes its cheating..
//                double boost = mind.parms.boost;

//                posxy = 10.0d + (boost * momentum);//dosnt seem right
//                //POS_X += (boost * velocity);

//                if (posxy < Constants.LOWXY)
//                    posxy = Constants.LOWXY;
//                if (posxy > Constants.HIGHXY)
//                    posxy = Constants.HIGHXY;

//                if (posxy <= posx_low) posx_low = posxy;
//                if (posxy > posx_high) posx_high = posxy;

//                return posxy;
//            }
//        }

//        //NewtonForce
//        public double Variable(UNIT curr)
//        {
//            /*
//             * I guess this is a changeable function, for now it is just the one I know to calculate force
//             * */

//            if (curr.IsNull())
//                throw new Exception();

//            if (curr.IsIDLE())
//                throw new Exception();

//            double acc = curr.HighAtZero;
//            acc = acc == 0.0d ? 1.0E-50 : acc;// jajajaa

//            return acc;
//        }

//        public void Calculate()
//        {
//            //force left
//            Fsta = ApplyStatic();
//            //force right
//            Fdyn = ApplyDynamic();

//            //momentum: p = m * v
//            momentum = (mind.parms.mass) * velocity;

//            if (momentum < 0.0d && momentum <= out_low) out_low = momentum;
//            if (momentum > 0.0d && momentum > out_high) out_high = momentum;

//            if (double.IsNaN(velocity))
//                throw new Exception();
//        }

//        /*
//         * friction
//         * */
//        public double ApplyStatic()
//        {
//            double force = mind.common.HighestForce().Variable;
//            //double limit = lim.Limit(true);

//            double F = force;//force, left
//            double m = mind.parms.mass;
//            double dt = DeltaT();
//            double dv = DeltaV(F, m, dt);

//            velocity -= dv * 0.5d;// limit;

//            //if (velocity < -1.0d)
//            //    throw new Exception();

//            return dv;
//        }

//        /*
//         * applied force
//         * */
//        public double ApplyDynamic()
//        {
//            UNIT curr_unit_th = mind.curr_unit;
            
//            if (curr_unit_th.IsNull())
//                throw new Exception();

//            bool first_run = false;
//            if (mind.cycles_all <= mind.parms.first_run)
//                first_run = true;

//            double max = mind.common.HighestForce().Variable;
//            double force = curr_unit_th.Variable;
//            //double limit = first_run ? 0.5d : lim.Limit(false);

//            double F = max - force;//force, right
//            double m = mind.parms.mass;
//            double dt = DeltaT();
//            double dv = DeltaV(F, m, dt);

//            /*
//                * maybe its a "fake it till you make it", situation??
//                * maybe this is actually the core of the project. and the task is to make this limit as smooth as possible
//                * maybe making it layered somehow, use chance, fuzzy?
//                * maybe a gradient of somesort
//                * */

//            //if (goodbye.IsNo())
//            velocity += dv * 0.5d;// limit;                                 // maybe this does actually make the "whoosh" effect over time/cycles

//            //if (velocity > 1.0d)
//            //    throw new Exception();

//            return dv;
//        }/**/
                
//        private double DeltaV(double F, double m, double dt)
//        {
//            //F=m*a
//            //a=dv/dt
//            //F=(m*dv)/dt
//            //F*dt=m*dv
//            //dv=(F*dt)/m
//            double dv = (F * dt) / m;
//            return dv;
//        }

//        private double DeltaT()
//        {
//            //most of the time this is true

//            double x = mind.parms.micro_sec;
//            double dt = x / 1000000.0d;
//            return dt;
//        }        
//    }
//}

