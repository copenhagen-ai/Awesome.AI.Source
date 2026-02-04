using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Source.Awesome.AI.Common
{
    public interface IBot
    {
        public MINDS mindtype { get; set; }
        public MECHANICS mech { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }        
    }

    public class Roberta : IBot
    {
        public MINDS mindtype { get; set; }
        public MECHANICS mech { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }

        public Roberta()
        {
            mindtype = MINDS.ROBERTA;
            mech = MECHANICS.BALLONHILL_HIGH;
            lng_dec = CONST.lng_dec_roberta;
        }
    }

    public class Andrew : IBot
    {
        public MINDS mindtype { get; set; }
        public MECHANICS mech { get; set; }
        public Dictionary<LONGTYPE, string> lng_dec { get; set; }

        public Andrew()
        {
            mindtype = MINDS.ANDREW;
            mech = MECHANICS.TUGOFWAR_HIGH;
            lng_dec = CONST.lng_dec_andrew;
        }
    }

    internal class Bots
    {
        public IBot GetBot(MINDS mind)
        {
            IBot bot = null;

            switch (mind)
            {
                case MINDS.ROBERTA: bot = new Roberta(); break;
                case MINDS.ANDREW: bot = new Andrew(); break;
                default: throw new Exception("Bots, GetBot");
            }            

            return bot;
        }
    }
}
