//using Awesome.AI.Awesome.AI.Core;
//using Awesome.AI.Core.Spaces;
//using Awesome.AI.Interfaces;
//using Awesome.AI.Variables;
//using static Awesome.AI.Variables.Enums;

//namespace Awesome.AI.Core.Mechanics
//{
//    public class _m_BallOnHillOld : IMechanics
//    {
//        public MECHANICS type { get; set; }
//        public MechSymbolicOut ms { get; set; }
//        public MechParams mp { get; set; }
//        public MechHelper mh { get; set; }

//        private TheMind mind;
//        private _m_BallOnHillOld() { }
//        public _m_BallOnHillOld(TheMind mind, MECHANICS type, PROPS props)
//        {
//            this.mind = mind;

//            this.type = type;

//            this.ms = new MechSymbolicOut() { };

//            this.mh = new MechHelper() { };

//            this.mp = new MechParams() { };

//            this.mp.props = new ModProperties(mind, props);

//            // Constants
//            mp.a = 0.1d;                    // Parabola coefficient (hill steepness)
//            mp.g = CONST.GRAVITY;           // Gravity (m/s^2)
//            mp.F0 = 5.0d;                   // Wind force amplitude (N)
//            mp.omega = Math.PI;             // Wind frequency
//            mp.beta = 0.02d;                // Friction coefficient
//            mp.dt = 0.1d;                   // Time step (s)
//            mp.eta = 0.5d;                  // Random noise amplitude for wind force
//            mp.m1 = 0.35d;                  // Ball mass (kg)

//            mp.vv_out_high = -1000.0d;
//            mp.vv_out_low = 1000.0d;
//            mp.dv_out_high = -1000.0d;
//            mp.dv_out_low = 1000.0d;
//            mp.posx_high = -1000.0d;
//            mp.posx_low = 1000.0d;
//        }

//        public double PosXY()
//        {
//            throw new NotImplementedException("m_BallOnHill, POS_XY");            
//        }        

//        public void Peek(UNIT _c)
//        {
//            throw new NotImplementedException("m_BallOnHill, Peek");
//        }

//        public double Dir()
//        {
//            double vv = ms.vv_sym_100;

//            double dir = vv > 50.0d ? 1.0d : -1.0d;

//            return dir;
//        }

//        private void Calc(int cycles)
//        {
//            double t = cycles * mp.dt;

//            // Compute forces
//            double Fx = ApplyDynamic(mp, t); // Wind force
//            double Fgravity = ApplyStatic(mp);
//            double Ffriction = -mp.beta * mp.velocity;
//            double Fnet = Fgravity + Fx + Ffriction;

//            // Compute acceleration along the tangent
//            double a_tangent = Fnet / mp.m1;

//            // Update velocity and position
//            mp.velocity += a_tangent * mp.dt;
//            //mp.pos_x += mp.velocity * mp.dt;

//            mp.vv_prev = mp.vv_curr;
//            mp.vv_curr = /*mp.m1 * */mp.velocity;
//            mp.dv_curr = mp.vv_curr - mp.vv_prev;

//            if (double.IsNaN(mp.cc_elec_curr) || double.IsNaN(mp.cc_elec_prev) || double.IsNaN(mp.dc_elec_curr) || double.IsNaN(mp.dc_elec_prev))
//                throw new Exception("m_BallOnHill, Calc");
//        }

//        private double ApplyStatic(MechParams mp)
//        {
//            double slope = 2 * mp.a * mp.pos_x; // Slope dy/dx
//            double sinTheta = slope / Math.Sqrt(1 + slope * slope);
//            double Fgravity = -(mp.m1 * mp.g) * sinTheta;

//            return Fgravity;
//        }

//        private double ApplyDynamic(MechParams mp, double t)
//        {
//            double Fx = mp.F0 * mh.Sine(mp.pattern_curr, t, mp.omega, 0.0d, 1.0d, 0.5d, 0.5d, 0.0d, 0.5d) + mh.GetRandomNoise(mind, mp.eta); // Wind force

//            if (Fx < 0.0d)
//                Fx = 0.0d;

//            if (double.IsNaN(Fx))
//                throw new Exception("m_BallOnHill, ApplyDynamic");

//            return Fx;
//        }

//        public void Calculate(PATTERN match, int cycles)
//        {
//            PATTERN pattern = mind.bot.pattern;

//            if (mind.z_current != "z_mech")
//                return;

//            if (pattern != match)
//                return;

//            mp.pattern_curr = pattern;

//            if (cycles == 1)
//                mh.Reset(mp);

//            Calc(cycles);

//            mh.Extremes(mp);
//            mh.Normalize(mind, mp);
//            ms.Convert(mp, MECHANICS.BALLONHILL_HIGH);
//        }
//    }
//}

