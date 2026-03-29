using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Factorys
{
    public interface IBot
    {
        public MINDS mindtype { get; set; }
        public LOGICTYPE logic { get; set; }
        public MECHANICS mech_low { get; set; }
        public MECHANICS mech_high { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }
        public ARC arc { get; set; }
        public int RUNTIME { get; set; }                                               //minutes / 2

    }

    public class Roberta : IBot
    {
        public MINDS mindtype { get; set; }
        public LOGICTYPE logic { get; set; }
        public MECHANICS mech_low { get; set; }
        public MECHANICS mech_high { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }
        public ARC arc { get; set; }
        public int RUNTIME { get; set; }

        public Roberta()
        {
            mindtype = MINDS.ROBERTA;
            logic = LOGICTYPE.PROBABILITY;
            mech_low = MECHANICS.TUGOFWAR_LOW;
            mech_high = MECHANICS.BALLONHILL_HIGH;
            lng_dec = CONST.lng_dec_roberta;

            validation = VALIDATION.BOTH;                                       //BOTH or TAGS
            tags = TAGS.ALL;                                                    //used with TAGS and BOTH
            occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
            pattern = PATTERN.MOODGENERAL;
            arc = ARC.DONTUSE;
            RUNTIME = (6) / 2;                                                  //minutes / 2
    }
    }

    public class Andrew : IBot
    {
        public MINDS mindtype { get; set; }
        public LOGICTYPE logic { get; set; }
        public MECHANICS mech_low { get; set; }
        public MECHANICS mech_high { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }
        public ARC arc { get; set; }
        public int RUNTIME { get; set; }

        public Andrew()
        {
            mindtype = MINDS.ANDREW;
            logic = LOGICTYPE.PROBABILITY;
            mech_low = MECHANICS.TUGOFWAR_LOW;
            mech_high = MECHANICS.TUGOFWAR_HIGH;
            lng_dec = CONST.lng_dec_andrew;

            validation = VALIDATION.BOTH;                                       //BOTH or TAGS
            tags = TAGS.ALL;                                                    //used with TAGS and BOTH
            occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
            pattern = PATTERN.MOODGENERAL;
            arc = ARC.DONTUSE;
            RUNTIME = (6) / 2;
        }
    }

    public class Basic : IBot
    {
        public MINDS mindtype { get; set; }
        public LOGICTYPE logic { get; set; }
        public MECHANICS mech_low { get; set; }
        public MECHANICS mech_high { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }
        public ARC arc { get; set; }
        public int RUNTIME { get; set; }

        public Basic()
        {
            mindtype = MINDS.BASIC;
            logic = LOGICTYPE.PROBABILITY;
            mech_low = MECHANICS.TUGOFWAR_LOW;
            mech_high = MECHANICS.BALLONHILL_HIGH;
            lng_dec = CONST.lng_dec_basic;

            validation = VALIDATION.BOTH;                                       //BOTH or TAGS
            tags = TAGS.ALL;                                                    //used with TAGS and BOTH
            occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
            pattern = PATTERN.MOODGENERAL;
            arc = ARC.USE;
            RUNTIME = (120) / 2;                                                  //minutes / 2
        }
    }

    public class BotFactory
    {
        private MINDS mindtype { get; set; }

        public TheMind mind;
        private BotFactory() { }
        public BotFactory(TheMind mind)
        {
            this.mind = mind;
            this.mindtype = mind.mindtype;
        }

        public IBot GetBot()
        {
            IBot bot = null;

            switch (mindtype)
            {
                case MINDS.ROBERTA: bot = new Roberta(); break;
                case MINDS.ANDREW: bot = new Andrew(); break;
                case MINDS.BASIC: bot = new Basic(); break;
                default: throw new Exception("Bots, GetBot");
            }            

            return bot;
        }
    }
}
