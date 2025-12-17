using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public  class MechParams
    {
        public double pp_elec_max { get; set; }
        public double pp_elec_min { get; set; }
        public double dp_elec_max { get; set; }
        public double dp_elec_min { get; set; }
        public double pp_elec_100 { get; set; }
        public double dp_elec_100 { get; set; }
        public double pp_elec_90 { get; set; }
        public double dp_elec_90 { get; set; }
        public double pp_elec_prev { get; set; }
        public double pp_elec_curr { get; set; }
        public double dp_elec_prev { get; set; }
        public double dp_elec_curr { get; set; }
        public double peek_pp_elec { get; set; }
        public double peek_max { get; set; }
        public double peek_min { get; set; }



        public double peek_velocity { get; set; }
        public double peek_norm { get; set; }
        public double vv_100 { get; set; }
        public double dv_100 { get; set; }
        public double vv_90 { get; set; }
        public double dv_90 { get; set; }
        public double vv_curr { get; set; }
        public double vv_prev { get; set; }
        public double dv_curr { get; set; }
        public double dv_prev { get; set; }
        public double vv_out_high_peek { get; set; }
        public double vv_out_low_peek { get; set; }
        public double vv_out_high { get; set; }
        public double vv_out_low { get; set; }
        public double dv_out_high { get; set; }
        public double dv_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        public double posxy { get; set; }
        
        public double velocity = 0.0;
        public double pos_x = CONST.STARTXY;

        public PATTERN pattern_curr = PATTERN.NONE;
        public PATTERN pattern_prev = PATTERN.NONE;

        public Properties props {  get; set; }

        //Shared
        public double dt { get; set; }              // Time step (s)
        public double m1 { get; set; }              // Ball mass (kg)
        public double m2 { get; set; }              // Mass in kg
        public double omega { get; set; }           // Frequency (0.5 Hz)
        public double eta { get; set; }             // Random noise amplitude for wind force
        public double g { get; set; }               // Gravity in m/s^2

        //Friction parameters
        public double mu { get; set; }              // Coefficient of kinetic friction
        public double frictionForce { get; set; }

        //Curcuit
        //public double voltageMax { get; set; }
        public double batteryVoltage { get; set; }
        public double variableResistance { get; set; }
        public double inductance { get; set; }
        public double dampingFactor { get; set; }
        public double currentCurrent { get; set; }
        public double deltaCurrent { get; set; }
        public double previousCurrent { get; set; }
        //public double cumulativeCharge { get; set; }
        //public double previousFluxLinkage { get; set; }
        //public double currentFluxLinkage { get; set; }
        

        //Noise 
        public double f_sta { get; set; }
        public double f_dyn { get; set; }
        public double f_friction { get; set; }
        public double acc_max {  get; set; }
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
