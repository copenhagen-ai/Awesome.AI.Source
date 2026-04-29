using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class MechSymbolicOut
    {
        public double vv_sym_peek { get; set; }
        public double vv_sym_low_peek { get; set; }
        public double vv_sym_high_peek { get; set; }

        public double dv_sym_prev { get; set; }
        public double vv_sym_prev { get; set; }
        public double fnet_sym_prev { get; set; }
        public double mom_sym_prev { get; set; }
        public double acc_sym_prev { get; set; }
        public double ke_sym_prev { get; set; }

        public double dv_sym_curr { get; set; }
        public double vv_sym_curr { get; set; }
        public double fnet_sym_curr { get; set; }
        public double mom_sym_curr { get; set; }
        public double acc_sym_curr { get; set; }
        public double ke_sym_curr { get; set; }
        
        public double m1_sym { get; set; }
        public double m2_sym { get; set; }

        public double fsta_sym { get; set; }
        public double fdyn_sym { get; set; }

        public double vv_sym_low { get; set; }
        public double vv_sym_high { get; set; }
        public double dv_sym_low { get; set; }
        public double dv_sym_high { get; set; }
        public double fnet_sym_low { get; set; }
        public double fnet_sym_high { get; set; }
        public double mom_sym_low { get; set; }
        public double mom_sym_high { get; set; }
        public double acc_sym_low { get; set; }
        public double acc_sym_high { get; set; }
        public double ke_sym_low { get; set; }
        public double ke_sym_high { get; set; }

        public double vv_sym_100 { get; set; }
        public double dv_sym_100 { get; set; }
        public double fnet_sym_100 { get; set; }
        public double mom_sym_100 { get; set; }
        public double acc_sym_100 { get; set; }
        public double ke_sym_100 { get; set; }

        public double vv_sym_90 { get; set; }
        public double dv_sym_90 { get; set; }
        public double fnet_sym_90 { get; set; }
        public double mom_sym_90 { get; set; }
        public double acc_sym_90 { get; set; }
        public double ke_sym_90 { get; set; }

        public double peek_sym_norm { get; set; }
        //public double inertia_sym { get; set; }

        public void Convert(MechParams mp, MECHANICS type)
        {
            switch (type)
            {
                case MECHANICS.TUGOFWAR_LOW:
                case MECHANICS.BALLONHILL_LOW:
                    //vv_sym_peek = mp.peek_vv_curr;
                    //vv_sym_low_peek = mp.peek_vv_out_low;
                    //vv_sym_high_peek = mp.peek_vv_out_high;

                    dv_sym_prev = mp.dv_prev;
                    vv_sym_prev = mp.vv_prev;
                    fnet_sym_prev = mp.fnet_prev;
                    mom_sym_prev = mp.mom_prev;
                    acc_sym_prev = mp.acc_prev;
                    ke_sym_prev = mp.ke_prev;

                    vv_sym_curr = mp.vv_curr;
                    dv_sym_curr = mp.dv_curr;
                    mom_sym_curr = mp.dv_curr;
                    acc_sym_curr = mp.dv_curr;
                    ke_sym_curr = mp.dv_curr;
                    fnet_sym_curr = mp.dv_curr;
                    //mo_sym_curr = mp.mo_curr;

                    vv_sym_low = mp.vv_out_low;
                    vv_sym_high = mp.vv_out_high;
                    dv_sym_low = mp.dv_out_low;
                    dv_sym_high = mp.dv_out_high;
                    fnet_sym_low = mp.fnet_out_low;
                    fnet_sym_high = mp.fnet_out_high;
                    mom_sym_low = mp.mom_out_low;
                    mom_sym_high = mp.mom_out_high;
                    acc_sym_low = mp.acc_out_low;
                    acc_sym_high = mp.acc_out_high;
                    ke_sym_low = mp.ke_out_low;
                    ke_sym_high = mp.ke_out_high;

                    vv_sym_100 = mp.vv_100;
                    dv_sym_100 = mp.dv_100;
                    fnet_sym_100 = mp.fnet_100;
                    mom_sym_100 = mp.mom_100;
                    acc_sym_100 = mp.acc_100;
                    ke_sym_100 = mp.ke_100;

                    vv_sym_90 = mp.vv_90;
                    dv_sym_90 = mp.dv_90;
                    fnet_sym_90 = mp.fnet_90;
                    mom_sym_90 = mp.mom_90;
                    acc_sym_90 = mp.acc_90;
                    ke_sym_90 = mp.ke_90;

                    //peek_sym_norm = mp.peek_vv_norm;

                    m1_sym = mp.m1;
                    m2_sym = mp.m2;

                    fsta_sym = mp.f_sta;
                    fdyn_sym = mp.f_dyn;
                    //inertia_sym = mp.inertia_lim;
                    break;
                case MECHANICS.CIRCUIT_1_LOW:
                case MECHANICS.CIRCUIT_2_LOW:
                    //vv_sym_peek = mp.peek_cc_elec;
                    //vv_sym_low_peek = mp.peek_vv_out_low;
                    //vv_sym_high_peek = mp.peek_vv_out_high;

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

                    //peek_sym_norm = mp.peek_vv_norm;
                    //inertia_sym = mp.inertia_lim;
                    break;
            }
        }
    }
}
