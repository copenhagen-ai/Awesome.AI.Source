using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public  class MechParams
    {
        public double peek_momentum { get; set; }
        public double peek_norm { get; set; }
        public double p_100 { get; set; }
        public double d_100 { get; set; }
        public double p_90 { get; set; }
        public double d_90 { get; set; }
        public double p_curr { get; set; }
        public double p_prev { get; set; }
        public double d_curr { get; set; }
        public double d_prev { get; set; }
        public double m_out_high_p { get; set; }
        public double m_out_low_p { get; set; }
        public double m_out_high { get; set; }
        public double m_out_low { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        public double posxy { get; set; }
        
        public double velocity = 0.0;
        public double position_x = CONST.STARTXY;

        public PATTERN pattern_curr = PATTERN.NONE;
        public PATTERN pattern_prev = PATTERN.NONE;

        //Shared
        public double dt { get; set; }              // Time step (s)
        public double m1 { get; set; }              // Ball mass (kg)
        public double m2 { get; set; }              // Mass in kg
        public double omega { get; set; }           // Frequency (0.5 Hz)
        public double eta { get; set; }             // Random noise amplitude for wind force
        public double g { get; set; }               // Gravity in m/s^2

        // Friction parameters
        public double mu { get; set; }              // Coefficient of kinetic friction
        public double frictionForce { get; set; }

        //Noise
        public double N { get; set; }
        public double a_max {  get; set; }
        public double damp { get; set; }

        //Tug Of War
        public double Fmax { get; set; }            // Max oscillating force for F2
        public double totalMass { get; set; }
        
        //Ball On Hill
        public double a { get; set; }              // Parabola coefficient (hill steepness)
        public double F0 { get; set; }             // Wind force amplitude (N)
        public double beta { get; set; }           // Friction coefficient
    }
}
