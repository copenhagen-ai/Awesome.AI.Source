using Awesome.AI.Core;
using Awesome.AI.Core.Mechanics;
using Awesome.AI.Interfaces;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Variables
{
    public class Params
    {
        public TheMind mind;
        private Params() { }
        public Params(TheMind mind)
        {
            this.mind = mind;
        }

        public IMechanics GetMechanics(MECHANICS run = MECHANICS.NONE)
        {
            IMechanics _mech = null;
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
                //case MECHANICS.WHEEL:
                //    _mech = new _TheWheel(this.mind, this);

                //    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                //    case_tags = TAGS.ALL;                                               //used with TAGS and BOTH
                //    case_occupasion = OCCUPASION.FIXED;                                 //used with OCCU and BOTH
                //    hack = HACKMODES.HACK;
                //    mech = MECHANICS.WHEEL;
                //    matrix_type = MATRIX.GPT;

                //    earth mass:      5.972×10 ^ 24 kg
                //    sun mass: 1.989×10 ^ 30 kg
                //    earth gravity: 9.807 m / s²
                //    distance sun:    148.010.000 km
                //    earth radius: 6,371 km
                //    Vars.zero_mass = 10.0d * 1.989E30d;
                //    mass = 5.972E24d;
                //    update_cred = 0.050d;

                //    /*
                //     * boost is life span
                //     * as it seem momentum seem to go towards below 0.0
                //     * boost should be as close 1.0, without dying to fast
                //     * */

                //    boost = 1E9d;

                //    break;
                case MECHANICS.NOISE:
                    _mech = new NoiseGenerator(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    version = PATTERN.MOODGENERAL;

                    high_at_zero = true;
                    update_cred = 0.030d;

                    //schedule_low = -1d;
                    //schedule_mid = -1d;
                    //schedule_high = -1d;

                    //hack = HACKMODES.NOHACK;                                          //not used any more
                    //shift = -2.0d;

                    /*
                     * boost is life span
                     * as it seem momentum seem to go towards below 0.0
                     * boost should be as close 1.0, without dying to fast
                     * */

                    boost = 1E-1d;

                    break;
                case MECHANICS.GRAVITY:
                    _mech = new Gravity(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    version = PATTERN.NONE;
                    //hack = HACKMODES.NOHACK;                                          //not used any more

                    high_at_zero = false;
                    update_cred = 0.05d;
                    
                    schedule_low = 2.0d;
                    schedule_mid = 6.0d;
                    schedule_high = 9.0d;

                    //shift = 0.0d;

                    /*
                        * boost is life span
                        * as it seem momentum seem to go towards below 0.0
                        * boost should be as close 1.0, without dying to fast
                        * */

                    boost = 1E-1d;

                    break;
                case MECHANICS.TUGOFWAR:
                    _mech = new TugOfWar(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    version = PATTERN.MOODGENERAL;
                    //hack = HACKMODES.NOHACK;                                          //not used any more

                    high_at_zero = true;
                    update_cred = 0.030d;
                    
                    schedule_low = 2.0d;
                    schedule_mid = 5.0d;
                    schedule_high = 8.0d;

                    //shift = -2.0d;

                    /*
                     * boost is life span
                     * as it seem momentum seem to go towards below 0.0
                     * boost should be as close 1.0, without dying to fast
                     * */

                    boost = 1E-2d;

                    break;
                case MECHANICS.HILL:
                    _mech = new BallOnHill(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or TAGS
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    version = PATTERN.MOODGENERAL;

                    high_at_zero = true;
                    update_cred = 0.050d;

                    //hack = HACKMODES.NOHACK;                                          //obsolete
                    //shift = 0.0d;                    

                    /*
                        * boost is life span?
                        * I dont know if boost acts the same for this system
                        * boost should be as close 1.0, without dying to fast
                        * */

                    boost = 1.0E0d;

                    break;
                default: throw new Exception("GetMechanics");
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
        public TAGS tags;                                               //used with WORLD and BOTH
        public OCCUPASION occupasion;                                   //used with SELF and BOTH
        public STATE state;
        public PATTERN version;
        //public HACKMODES hack;

        // should it be 200, 1000 or more???
        public double low_cut;
        public double update_cred;
        public double boost;//dimm the fluctuations

        private bool high_at_zero { get; set; }

        public double schedule_low { get; set; }
        public double schedule_mid { get; set; }
        public double schedule_high { get; set; }

        //public double shift;
        //public double delta_time;
        //public double mass;
        //public double max_index;
        //public double pos_x_high;
        //public double pos_x_low;
        //public double pos_x_start;
        //public double scale;
        //public double dir_learningrate;
        //public double lim_bias = 0.0d;                                            // approx start, then it auto adjusts
        //public double lim_learningrate;                                           // I call it learningrate, but really it is just an adjustment
        //public bool goto_school = false;
        //public bool debug = true;
        //public bool is_accord = true;
        //public int learning_epochs = 100;
    }
}
