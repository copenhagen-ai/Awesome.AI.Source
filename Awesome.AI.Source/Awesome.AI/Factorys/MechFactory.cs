using Awesome.AI.Core;
using Awesome.AI.Core.Electrical;
using Awesome.AI.Core.Mechanics;
using Awesome.AI.Interfaces;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Factorys
{
    public class MechFactory
    {
        public TheMind mind;
        private MechFactory() { }
        public MechFactory(TheMind mind)
        {
            this.mind = mind;
        }

        public IMechanics GetMech(MECHANICS run)
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
                    return new m_NoiseGenerator(mind, run);
                case MECHANICS.CIRCUIT_1_LOW:
                case MECHANICS.CIRCUIT_2_LOW:
                    return new e_CircuitSimulator(mind, run);
                case MECHANICS.TUGOFWAR_HIGH:
                    return new m_TugOfWar(mind, PROPS.COMMUNICATION);
                case MECHANICS.BALLONHILL_HIGH:
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
    }
}
