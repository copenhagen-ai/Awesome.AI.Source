using Awesome.AI.Core;
using Awesome.AI.Core.Electrical;
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

        public IMechanics Mechanics(MECHANICS run)
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
                case MECHANICS.TUGOFWAR_LOW:
                case MECHANICS.BALLONHILL_LOW:
                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    pattern = PATTERN.MOODGENERAL;
                    return new m_NoiseGenerator(mind, run);
                case MECHANICS.CIRCUIT_1_LOW:
                case MECHANICS.CIRCUIT_2_LOW:
                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    pattern = PATTERN.MOODGENERAL;
                    return new e_CircuitSimulator(mind, run);
                case MECHANICS.TUGOFWAR_HIGH:
                    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    pattern = PATTERN.MOODGENERAL;
                    return new m_TugOfWar(mind, PROPS.COMMUNICATION);
                case MECHANICS.BALLONHILL_HIGH:
                    validation = VALIDATION.BOTH;                                       //BOTH or TAGS
                    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                    pattern = PATTERN.MOODGENERAL; 
                    return new m_BallOnHill(mind, PROPS.BRAINWAVE);
                //case MECHANICS.GRAVITY:
                //    _mech = new GravityAndRocket(mind, this);
                //    validation = VALIDATION.BOTH;                                       //BOTH or OCCU
                //    tags = TAGS.ALL;                                                    //used with TAGS and BOTH
                //    occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
                //    pattern = PATTERN.MOODGENERAL;
                //    break;
                default: throw new Exception("GetMechanics");
            }
        }        

        /*
         * VARIABLE parameters
         * */

        public VALIDATION validation;
        public TAGS tags;                                               //used with WORLD and BOTH
        public OCCUPASION occupasion;                                   //used with SELF and BOTH
        public PATTERN pattern;
    }
}
