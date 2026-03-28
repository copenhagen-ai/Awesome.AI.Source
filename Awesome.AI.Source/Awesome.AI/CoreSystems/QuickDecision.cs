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
            res.Add("ARCLEARN", false);
            res.Add("ARCSOLVE", false);

            new_res.Add("WHISTLE", false);
            new_res.Add("MATHLEARN", false);
            new_res.Add("MATHSOLVE", false);
            new_res.Add("ARCLEARN", false);
            new_res.Add("ARCSOLVE", false);

            Count.Add("WHISTLE", 0);
            Count.Add("MATHLEARN", 0);
            Count.Add("MATHSOLVE", 0);
            Count.Add("ARCLEARN", 0);
            Count.Add("ARCSOLVE", 0);
        }
        
        private int Period { get; set; }
        private Dictionary<string, int> Count = new Dictionary<string, int>();
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
            
            if (Count[type] > Period)
            {
                new_res[type] = false;
                res[type] = false;
            }

            Count[type]++;
            
            return res[type];            
        }

        public void Run(bool pro, UNIT curr, string type)
        {
            if (mind.z_current != "z_noise")
                return;

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
                Setup(type, 5, 5);
        }

        private void Setup(string type, int count, int period)
        {
            Period = period;
            
            Count[type] = 0;

            List<string> should_decision = new List<string>();

            for (int i = 0; i < count; i++)
                should_decision.Add(CONST.QUICK);// YES

            for (int i = 0; i < count; i++)
                should_decision.Add(CONST.QUICK);// NO

            should_decision.Shuffle();

            mind.access.QDRESETU();

            TONE tone = TONE.RANDOM;

            mind.memory.Decide(STATE.QUICKDECISION, 8, CONST.QSUB_SHOULD, should_decision, UNITTYPE.QDECISION, LONGTYPE.NONE, 0, tone);
            
            mind.STATE = STATE.QUICKDECISION;
        }
    }
}
