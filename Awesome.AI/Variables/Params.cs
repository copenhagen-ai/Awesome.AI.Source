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
                case MECHANICS.NOISE:
                    _mech = new NoiseGenerator(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    pattern = PATTERN.MOODGENERAL;

                    high_at_zero = true;                    
                    update_cred = 0.030d;

                    break;
                case MECHANICS.GRAVITY:
                    _mech = new GravityAndRocket(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    pattern = PATTERN.NONE;

                    high_at_zero = false;
                    update_cred = 0.05d;
                    
                    schedule_low = 2.0d;
                    schedule_mid = 6.0d;
                    schedule_high = 9.0d;

                    break;
                case MECHANICS.TUGOFWAR:
                    _mech = new TugOfWar(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    pattern = PATTERN.MOODGENERAL;

                    high_at_zero = true;
                    update_cred = 0.030d;

                    break;
                case MECHANICS.HILL:
                    _mech = new BallOnHill(mind, this);

                    validation = VALIDATION.BOTH;                                       //BOTH or TAGS
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    state = STATE.JUSTRUNNING;
                    pattern = PATTERN.MOODGENERAL;

                    high_at_zero = false;
                    update_cred = 0.050d;                   

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
        public PATTERN pattern;

        public bool high_at_zero { get; set; }        
        public double low_cut { get; set; }
        public double update_cred { get; set; }

        public double schedule_low { get; set; }
        public double schedule_mid { get; set; }
        public double schedule_high { get; set; }
    }
}
