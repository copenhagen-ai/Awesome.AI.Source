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

            count.Add("WHISTLE", 0);
            count.Add("MATHLEARN", 0);
            count.Add("MATHSOLVE", 0);
            count.Add("ARCLEARN", 0);
            count.Add("ARCSOLVE", 0);
        }
        
        private int period { get; set; }
        private Dictionary<string, int> count = new Dictionary<string, int>();
        private Dictionary<string, bool> res = new Dictionary<string, bool>();

        public bool Result(string type)
        {
            if (!res[type])
                return false;
            
            count[type]++;

            if (count[type] > period)
            {
                res[type] = false;
                count[type] = 0;
            }
                        
            return res[type];            
        }

        public void Decide(UNIT curr, string type)
        {
            if (!curr.IsQDECISION())
                return;

            bool deciding = type == "QYES" || type == "QNO";

            if (!deciding)
                Setup(curr, type, 5, 5);

            if (deciding)
                Run(curr);
        }

        public void Run(UNIT curr)
        {
            //if (count[type] > 0)
            //    return;

            if (mind.STATE == STATE.QUICKDECISION && mind.access.QDCOUNT() > 0)
                mind.access.QDREMOVE(curr);

            if (mind.STATE == STATE.QUICKDECISION && mind.access.QDCOUNT() == 0)
                res[current] = curr.Data == "QYES";

            if (mind.STATE == STATE.QUICKDECISION && mind.access.QDCOUNT() == 0)
                mind.STATE = STATE.JUSTRUNNING;
        }

        private string current = "";
        private void Setup(UNIT curr, string type, int _c, int per)
        {
            if (curr.Data == type && mind.STATE == STATE.JUSTRUNNING)
            {
                period = per;
            
                count[type] = 0;

                List<string> should_decision = new List<string>();

                for (int i = 0; i < _c; i++)
                    should_decision.Add(CONST.QUICK);// YES

                for (int i = 0; i < _c; i++)
                    should_decision.Add(CONST.QUICK);// NO

                should_decision.Shuffle();

                mind.access.QDRESETU();

                TONE tone = TONE.RANDOM;

                mind.memory.Decide(STATE.QUICKDECISION, 100, CONST.QSUB_SHOULD, should_decision, UNITTYPE.QDECISION, LONGTYPE.NONE, 0, tone);
            
                mind.STATE = STATE.QUICKDECISION;

                current = type;
            }
        }
    }
}
