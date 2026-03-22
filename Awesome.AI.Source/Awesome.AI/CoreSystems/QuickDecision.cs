using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Core.Spaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
{
    public class QuickDecision
    {
        private TheMind mind;
        private QuickDecision() { }

        public QuickDecision(TheMind mind)
        {
            this.mind = mind;

            res.Add("WHISTLE", false);
            res.Add("MATHLEARN", false);
            res.Add("MATHSOLVE", false);

            new_res.Add("WHISTLE", false);
            new_res.Add("MATHLEARN", false);
            new_res.Add("MATHSOLVE", false);

        }
        
        private int Period { get; set; }
        private int Count { get; set; }

        private Dictionary<string, bool> res = new Dictionary<string, bool>();
        private Dictionary<string, bool> new_res = new Dictionary<string, bool>();

        private void Stats()
        {
            List<UNIT> list = mind.access.UNITS_ALL();
            list = list.Where(x=>x.IsQDECISION()).ToList();

            int count = list.Count();

            Console.WriteLine("count" + count);
        }

        public bool Result(string type)
        {
            if (!new_res[type])
                return false;
            
            if (Count > Period)
            {
                new_res[type] = false;
                res[type] = false;
            }

            Count++;           
            
            return res[type];            
        }

        public void Run(bool pro, UNIT curr, string type)
        {
            if (new_res[type])
                return;

            if (!curr.IsQDECISION())
                return;

            if (mind.STATE == STATE.QUICKDECISION && mind.access.QDCOUNT() > 0)
            {
                if (mind.access.QDCOUNT() == 1)
                {
                    res[type] = curr.Data == "QYES";

                    new_res[type] = true;

                    mind.STATE = STATE.JUSTRUNNING;                    
                }

                mind.access.QDREMOVE(curr);

                return;
            }


            if (curr.Data == type)
                Setup(5, 5);
        }

        private void Setup(int count, int period)
        {
            Period = period;
            
            Count = 0;

            List<string> should_decision = new List<string>();

            for (int i = 0; i < count; i++)
                should_decision.Add(CONST.QUICK);// YES

            for (int i = 0; i < count; i++)
                should_decision.Add(CONST.QUICK);// NO

            should_decision.Shuffle();

            mind.access.QDRESETU();

            TONE tone = TONE.RANDOM;

            mind.memory.Decide(STATE.QUICKDECISION, CONST.QSUB_SHOULD, should_decision, UNITTYPE.QDECISION, LONGTYPE.NONE, 0, tone);
            
            mind.STATE = STATE.QUICKDECISION;
        }
    }
}
