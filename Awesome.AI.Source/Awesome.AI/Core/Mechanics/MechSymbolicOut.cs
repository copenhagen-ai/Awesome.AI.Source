using Awesome.AI.Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class MechSymbolicOut
    {
        public double vv_sym_peek { get; set; }
        public double vv_sym_low_peek { get; set; }
        public double vv_sym_high_peek { get; set; }

        public double vv_sym_curr { get; set; }
        public double dv_sym_curr { get; set; }

        public double vv_sym_low { get; set; }
        public double vv_sym_high { get; set; }
        public double dv_sym_low { get; set; }
        public double dv_sym_high { get; set; }

        public double vv_sym_100 { get; set; }
        public double dv_sym_100 { get; set; }

        public double vv_sym_90 { get; set; }
        public double dv_sym_90 { get; set; }

        public double peek_sym_norm { get; set; }
        //public double inertia_sym { get; set; }

        public void Convert(MechParams mp, MECHANICS type)
        {
            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                case MECHANICS.BALLONHILL_LOW:
                    vv_sym_peek = mp.peek_velocity;
                    vv_sym_low_peek = mp.vv_out_low_peek;
                    vv_sym_high_peek = mp.vv_out_high_peek;

                    vv_sym_curr = mp.vv_curr;
                    dv_sym_curr = mp.dv_curr;

                    vv_sym_low = mp.vv_out_low;
                    vv_sym_high = mp.vv_out_high;
                    dv_sym_low = mp.dv_out_low;
                    dv_sym_high = mp.dv_out_high;

                    vv_sym_100 = mp.vv_100;
                    dv_sym_100 = mp.dv_100;

                    vv_sym_90 = mp.vv_90;
                    dv_sym_90 = mp.dv_90;

                    peek_sym_norm = mp.peek_norm;
                    //inertia_sym = mp.inertia_lim;
                    break;
                case MECHANICS.CIRCUIT_1_LOW:
                case MECHANICS.CIRCUIT_2_LOW:
                    vv_sym_peek = mp.peek_cc_elec;
                    vv_sym_low_peek = mp.peek_min;
                    vv_sym_high_peek = mp.peek_max;

                    vv_sym_curr = mp.cc_elec_curr;
                    dv_sym_curr = mp.dc_elec_curr;

                    vv_sym_low = mp.cc_elec_min;
                    vv_sym_high = mp.cc_elec_max;
                    dv_sym_low = mp.dc_elec_min;
                    dv_sym_high = mp.dc_elec_max;

                    vv_sym_100 = mp.cc_elec_100;
                    dv_sym_100 = mp.dc_elec_100;

                    vv_sym_90 = mp.cc_elec_90;
                    dv_sym_90 = mp.dc_elec_90;

                    peek_sym_norm = mp.peek_norm;
                    //inertia_sym = mp.inertia_lim;
                    break;
                case MECHANICS.TUGOFWAR_HIGH:
                case MECHANICS.BALLONHILL_HIGH:
                    vv_sym_peek = mp.peek_velocity;
                    vv_sym_low_peek = mp.vv_out_low_peek;
                    vv_sym_high_peek = mp.vv_out_high_peek;

                    vv_sym_curr = mp.vv_curr;
                    dv_sym_curr = mp.dv_curr;

                    vv_sym_low = mp.vv_out_low;
                    vv_sym_high = mp.vv_out_high;
                    dv_sym_low = mp.dv_out_low;
                    dv_sym_high = mp.dv_out_high;

                    vv_sym_100 = mp.vv_100;
                    dv_sym_100 = mp.dv_100;

                    vv_sym_90 = mp.vv_90;
                    dv_sym_90 = mp.dv_90;

                    peek_sym_norm = mp.peek_norm;
                    //inertia_sym = mp.inertia_lim;
                    break;
                    break;
            }
        }
    }
}
