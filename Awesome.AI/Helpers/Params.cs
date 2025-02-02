using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Core.Mechanics;
using Awesome.AI.Interfaces;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Helpers
{
    public class Params
    {
        public TheMind mind;
        private Params() { }
        public Params(TheMind mind, MECHANICS run) 
        {
            this.mind = mind;
            GetMechanics(run);
        }

        //public int selector;
        public IMechanics _mech = null;
        private bool high_at_zero {  get; set; }
        
        public IMechanics GetMechanics(MECHANICS run = MECHANICS.NONE)
        {
            if (_mech == null)
            {
                /*
                 * INFO (not used)
                 * earth mass:      5.972 × 10^24 kg
                 * sun mass:        1.989 × 10^30 kg
                 * earth radius:            6,371 km
                 * distance moon:         384,400 km
                 * distance sun:      148.010.000 km
                 * car mass:                  500 kg
                 * */

                switch (run)
                {
                    case MECHANICS.WHEEL: 
                        _mech = new _TheWheel(this);

                        validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                        case_tags = TAGS.ALL;                                               //used with TAGS and BOTH
                        case_occupasion = OCCUPASION.FIXED;                                 //used with OCCU and BOTH
                        hack1 = HACKMODES.HACK;
                        hack2 = HACKMODES.NOHACK;
                        mech = MECHANICS.WHEEL;
                        matrix_type = MATRIX.GPT;

                        //earth mass:      5.972×10^24 kg
                        //sun mass:        1.989×10^30 kg
                        //earth gravity:         9.807 m/s²
                        //distance sun:    148.010.000 km
                        //earth radius:          6,371 km
                        Vars.zero_mass = 10.0d * 1.989E30d;
                                                
                        mass = 5.972E24d;
                        max_index = 100.0d;
                        pos_x_high = 10.0d;
                        pos_x_low = 0.0d;
                        pos_x_start = 5.0d;
                        update_cred = 0.050d;

                        //low_cut = 1.0E-9d;
                        //scale = 2.0d;
                        //dir_learningrate = -1d;

                        /*
                         * boost is life span
                         * as it seem momentum seem to go towards below 0.0
                         * boost should be as close 1.0, without dying to fast
                         * */

                        boost = 1E9d;

                        //selector = 0;

                        break;
                    case MECHANICS.GRAVITY: 
                        _mech = new _TheGravity(this);

                        validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                        case_tags = TAGS.ALL;                                               //used with TAGS and BOTH
                        case_occupasion = OCCUPASION.FIXED;                                 //used with OCCU and BOTH
                        hack1 = HACKMODES.HACK;
                        hack2 = HACKMODES.NOHACK;
                        mech = MECHANICS.GRAVITY;
                        matrix_type = MATRIX.GPT;

                        //earth mass:      5.972×10^24 kg
                        //sun mass:        1.989×10^30 kg
                        //earth gravity:         9.807 m/s²
                        //distance sun:    148.010.000 km
                        //earth radius:          6,371 km
                        Vars.zero_mass = 5.972E24d;
                                                
                        mass = 0.05d;
                        max_index = 7000.0d;
                        pos_x_high = 10.0d;
                        pos_x_low = 0.0d;
                        pos_x_start = 5.0d;
                        update_cred = 0.050d;

                        //low_cut = 5.610E7d;
                        //scale = -1d;
                        //dir_learningrate = -1d;

                        /*
                         * boost is life span
                         * as it seem momentum seem to go towards below 0.0
                         * boost should be as close 1.0, without dying to fast
                         * */

                        boost = 1E-10d;

                        //selector = 1;

                        break;
                    case MECHANICS.CONTEST: 
                        _mech = new _TheContest(this);

                        validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                        case_tags = TAGS.ALL;                                               //used with TAGS and BOTH
                        case_occupasion = OCCUPASION.DYNAMIC;                                 //used with OCCU and BOTH
                        hack1 = HACKMODES.NOHACK;
                        hack2 = HACKMODES.NOHACK;
                        mech = MECHANICS.CONTEST;
                        matrix_type = MATRIX.GPT;

                        //earth mass:      5.972×10^24 kg
                        //sun mass:        1.989×10^30 kg
                        //earth gravity:         9.807 m/s²
                        //distance sun:    148.010.000 km
                        //earth radius:          6,371 km
                                                                        
                        high_at_zero = true;

                        mass = 500.0d;
                        max_index = 100.0d;
                        pos_x_high = 10.0d;
                        pos_x_low = 0.0d;
                        pos_x_start = 5.0d;
                        update_cred = 0.030d;
                        shift = -2.0d;

                        //low_cut = 5.610E7d;
                        //scale = 80.0d;
                        //dir_learningrate = -1d;

                        /*
                         * boost is life span
                         * as it seem momentum seem to go towards below 0.0
                         * boost should be as close 1.0, without dying to fast
                         * */

                        boost = 1E-2d;

                        //selector = 1;

                        break;
                    case MECHANICS.HILL: 
                        _mech = new _TheHill(this);

                        validation = VALIDATION.BOTH;                                       //BOTH or TAGS
                        case_tags = TAGS.ALL;                                               //used with TAGS and BOTH
                        case_occupasion = OCCUPASION.DYNAMIC;                               //used with OCCU and BOTH
                        hack1 = HACKMODES.HACK;
                        hack2 = HACKMODES.HACK;
                        mech = MECHANICS.HILL;
                        matrix_type = MATRIX.GPT;

                        //earth mass:    5.972 × 10^24kg
                        //sun mass:      1.989 × 10^30kg
                        //earth gravity: 9.807m/s²
                        Vars.var_a = -0.1d;
                        Vars.var_b = 0.0d;
                        Vars.var_c = 10.0d;

                        //Vars.var_a = -0.01d;
                        //Vars.var_b = 0.0d;
                        //Vars.var_c = 1.0d;

                        //Vars.var_a = -0.001d;
                        //Vars.var_b = 0.0d;
                        //Vars.var_c = 0.1d;

                        high_at_zero = true;

                        mass = 0.5d;
                        max_index = 100.0d;
                        pos_x_high = 10.0d;
                        pos_x_low = 0.0d;
                        pos_x_start = 5.0d;
                        update_cred = 0.050d;
                        shift = -3.0d;

                        //high_pass = 4.48d;
                        //scale = -1d;
                        //dir_learningrate = 0.001d;

                        /*
                         * boost is life span?
                         * I dont know if boost acts the same for this system
                         * boost should be as close 1.0, without dying to fast
                         * */

                        boost = 1.0E1d;

                        //selector = 2;

                        break;
                    default: throw new Exception();
                }
            }
            
            return _mech;
        }

        public void UpdateLowCut()
        {
            List<UNIT> units = mind.mem.UNITS_ALL();

            units = high_at_zero ?
                units = units.OrderBy(x => x.Index).ToList() :
                units = units.OrderByDescending(x => x.Index).ToList();

            UNIT _u = units[3];
                        
            low_cut = _u.Variable + 0.1d;
        }
        
        /*
         * VARIABLE parameters
         * */

        public VALIDATION validation;      
        public TAGS case_tags;                                               //used with WORLD and BOTH
        public OCCUPASION case_occupasion;                                   //used with SELF and BOTH
        public HACKMODES hack1;
        public HACKMODES hack2;
        public MECHANICS mech;
        public MATRIX matrix_type;


        //BOTH      WORLD       SELF
        //public double[,] update_nrg_vals = {
        //    { 0.040d,   0.000d,     0.000d },//TheWheel
        //    { 0.030d,   0.000d,     0.000d },//TheContest
        //    { 0.050d,   0.000d,     0.000d } //TheHill
        //};

        //must be between 1.0 and 2.0
        //BOTH      WORLD       SELF
        //public double[,] lim_correction_vals = {
        //    { 1.20d,    0.00d,      0.00d },//TheWheel
        //    { 1.20d,    0.00d,      0.00d },//TheContest
        //    { 1.80d,    0.00d,      0.00d } //TheHill
        //};

        //public double update_nrg
        //{                                               // curve adjustment, spike the "U" form, lower -> flatter "U"
        //    get
        //    {
        //        return
        //            validation == VALIDATION.BOTH ? update_nrg_vals[selector, 0] :
        //            validation == VALIDATION.EXTERNAL ? update_nrg_vals[selector, 1] :
        //            validation == VALIDATION.INTERNAL ? update_nrg_vals[selector, 2] : -1d;
        //    }
        //}

        //public double lim_correction {                                           // yesno ratio, curve adjustment, tip the "U" form, MAX: 2.0 MIN: 0.1, lower -> more yes
        //    get
        //    {
        //        return
        //            validation == VALIDATION.BOTH ? lim_correction_vals[selector, 0] :
        //            validation == VALIDATION.EXTERNAL ? lim_correction_vals[selector, 1] :
        //            validation == VALIDATION.INTERNAL ? lim_correction_vals[selector, 2] : -1d;
        //    }
        //}

        /*
         * FIXED parameters
         * 
         * 0.888 = 8 / 9
         * 0.777 = 7 / 9
         * 0.666 = 6 / 9, hehe
         * 0.555 = 5 / 9
         * */

        public double base_friction = 6d / 9d;                                      // COMMENT NEEDS UPDATE: needs to be this otherwise position keeps going down
        public double lapses_total = 99d;                                           // yesno ratio : reaction time in cycles
        public double ratio = 50d;
        public double slope = 0.666d;

        // should it be 200, 1000 or more???
        public double mass;
        public double max_index;
        public double low_cut;
        public double pos_x_high;
        public double pos_x_low;
        public double pos_x_start;
        public double update_cred;
        public double boost;                                              //dimm the fluctuations
        public double shift;
        public double max_pain = 999.99d;
                
        public int first_run = 5;
        public int number_of_units = 10;
        public int runtime = 2; //minutes
        
        //public double scale;
        //public double hill_a;
        //public double hill_b;
        //public double hill_c;
        //public double dir_learningrate;
        //public double lim_bias = 0.0d;                                            // approx start, then it auto adjusts
        //public double lim_learningrate;                                           // I call it learningrate, but really it is just an adjustment
        //public bool goto_school = false;
        //public bool debug = true;
        //public bool is_accord = true;
        //public int learning_epochs = 100;
        
        /*
         * these seem to be related
         * hist_total
         * - my guess is: unlike normal AI algorithms, here we want to hit local minimas, so not to much memory
         * - does hist_total increase with the number of HUBS/UNITS?
         * - hist_total seems to have impact on "thought patterns"
         * - I rarely change these variables anymore
         * */
        public int micro_sec = 4000;                                       //call micro timer every 1000µs (1ms)
        public int hist_total = 100;                                       //the number of UNITS???
        public int remember = 200;
    }
}
