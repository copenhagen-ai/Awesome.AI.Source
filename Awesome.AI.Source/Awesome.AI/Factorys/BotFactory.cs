using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Factorys
{
    public interface IBot
    {
        public MINDS mindtype { get; set; }
        public MECHANICS mech { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }
    }

    public class Roberta : IBot
    {
        public MINDS mindtype { get; set; }
        public MECHANICS mech { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }

        public Roberta()
        {
            mindtype = MINDS.ROBERTA;
            mech = MECHANICS.BALLONHILL_HIGH;
            lng_dec = CONST.lng_dec_roberta;

            validation = VALIDATION.BOTH;                                       //BOTH or TAGS
            tags = TAGS.ALL;                                                    //used with TAGS and BOTH
            occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
            pattern = PATTERN.MOODGENERAL;
        }
    }

    public class Andrew : IBot
    {
        public MINDS mindtype { get; set; }
        public MECHANICS mech { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }

        public Andrew()
        {
            mindtype = MINDS.ANDREW;
            mech = MECHANICS.TUGOFWAR_HIGH;
            lng_dec = CONST.lng_dec_andrew;

            validation = VALIDATION.BOTH;                                       //BOTH or TAGS
            tags = TAGS.ALL;                                                    //used with TAGS and BOTH
            occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
            pattern = PATTERN.MOODGENERAL;
        }
    }

    public class Basic : IBot
    {
        public MINDS mindtype { get; set; }
        public MECHANICS mech { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }
        public VALIDATION validation { get; set; }
        public TAGS tags { get; set; }                                               //used with WORLD and BOTH
        public OCCUPASION occupasion { get; set; }                                   //used with SELF and BOTH
        public PATTERN pattern { get; set; }

        public Basic()
        {
            mindtype = MINDS.BASIC;
            mech = MECHANICS.BALLONHILL_HIGH;
            lng_dec = CONST.lng_dec_basic;

            validation = VALIDATION.BOTH;                                       //BOTH or TAGS
            tags = TAGS.ALL;                                                    //used with TAGS and BOTH
            occupasion = OCCUPASION.DYNAMIC;                                    //used with OCCU and BOTH
            pattern = PATTERN.MOODGENERAL;
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
