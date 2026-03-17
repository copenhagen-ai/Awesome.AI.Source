using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Core.Spaces;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class m_TugOfWar : IMechanics
    {
        public MECHANICS type { get; set; }
        public MechSymbolicOut ms { get; set; }
        public MechParams mp { get; set; }
        public MechHelper mh { get; set; }

        private TheMind mind;
        private m_TugOfWar() { }
        public m_TugOfWar(TheMind mind, MECHANICS type, PROPS props)
        {
            this.mind = mind;

            this.type = type;

            this.ms = new MechSymbolicOut() { };

            this.mh = new MechHelper() { };

            this.mp = new MechParams() { };
            
            mp.props = new Properties(mind, props);

            mp.Fmax = 5000.0d;                                              // Max oscillating force for F2
            mp.omega = 2 * Math.PI * 0.5;                                   // Frequency (0.5 Hz)
            mp.eta = 0.5d;                                                  // Randomness factor
            mp.m1 = 300.0d;                                                 // Mass of Car 1 in kg
            mp.m2 = 300.0d;                                                 // Mass of Car 2 in kg
            mp.totalMass = mp.m1 + mp.m2;
            mp.dt = 0.1d;                                                   // Time step (s)

            // Friction parameters
            mp.mu = 0.1;                                                    // Coefficient of kinetic friction
            mp.g = CONST.GRAVITY;                                           // Gravity in m/s^2
            mp.frictionForce = mp.mu * mp.totalMass * mp.g;

            mp.pos_x = CONST.STARTXY;

            mp.vv_out_high = -1000.0d;
            mp.vv_out_low = 1000.0d;
            mp.dv_out_high = -1000.0d;
            mp.dv_out_low = 1000.0d;
            mp.posx_high = -1000.0d;
            mp.posx_low = 1000.0d;
        }

        public double PosXY()
        {
            throw new NotImplementedException("m_TugOfWar, POS_XY");            
        }

        public void Peek(UNIT curr)
        {
            throw new NotImplementedException("m_TugOfWar, Peek");
        }

        public double Dir()
        {
            double dv = ms.dv_sym_100;

            double dir = dv > 50.0d ? 1.0d : -1.0d;

            return dir;
        }

        private void Calc(PATTERN pattern, int cycles)
        {
            double t = cycles * mp.dt;

            double F1 = ApplyStatic(mp);                                        // Constant force in Newtons (e.g., truck pulling)
            double F2 = ApplyDynamic(mp, t);
            double Ffriction = mp.frictionForce * Math.Sign(mp.velocity);       // Friction opposes motion
            double Fnet = -F1 + F2 - Ffriction;                                 // Net force with F1 constant and F2 dynamic

            // If friction is stronger than applied force and velocity is near zero, stop motion
            if (Math.Abs(Fnet) < mp.frictionForce && Math.Abs(mp.velocity) < 0.01)
            {
                Fnet = 0;
                mp.velocity = 0;
            }

            double acceleration = Fnet / mp.totalMass;

            // Update velocity and position
            mp.velocity += acceleration * mp.dt;                                // Integrate acceleration to get velocity
            //mp.pos_x += mp.velocity * mp.dt;                               // Integrate velocity to get position
            
            mp.vv_prev = mp.vv_curr;                                              // Store current momentum for next iteration
            mp.vv_curr = /*mp.totalMass * */mp.velocity;
            mp.dv_curr = mp.vv_curr - mp.vv_prev;                                  // Compute change in momentum

            if (double.IsNaN(mp.velocity))
                ;// throw new Exception("NAN");
        }

        /*
         * force left
         * */
        public double ApplyStatic(MechParams mp)
        {
            double Fapplied = mp.Fmax * CONST.BASE_SCALE;

            return Fapplied;           
        }

        /*
         * force right
         * */
        public double ApplyDynamic(MechParams mp, double t)
        {
            double Fapplied = mp.Fmax * (mh.Sine(mp.pattern_curr, t, mp.omega, 0.0d, 1.0d, 0.6d, 0.4d, 0.0d, 0.4d) + mh.GetRandomNoise(mind, mp.eta));  // Dynamic force
            
            return Fapplied;
        }

        public void Calculate(PATTERN match, int cycles)
        {
            PATTERN pattern = mind.bot.pattern;

            if (mind.z_current != "z_mech")
                return;

            if (pattern != match)
                return;

            mp.pattern_curr = pattern;

            if (cycles == 1)
                mh.Reset(mp);

            Calc(pattern, cycles);

            mh.Extremes(mp);
            mh.Normalize(mind, mp);
            ms.Convert(mp, MECHANICS.TUGOFWAR_HIGH);
        }
    }
}

