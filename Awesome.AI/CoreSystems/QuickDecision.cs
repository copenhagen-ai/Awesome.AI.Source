using Awesome.AI.Common;
using Awesome.AI.Core;
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
        }

        private bool Go { get; set; }
        private int Period { get; set; }
        private int Count { get; set; }

        private int epochold = -1;
        private bool res = false;
        private bool new_res = false;

        public bool Result
        {
            get
            {
                if (new_res && mind.epochs > epochold)
                {
                    if (Count > Period)
                    {
                        new_res = false;
                        res = false;
                    }

                    Count++;
                }

                epochold = mind.epochs;

                return res;
            }
        }

        public void Run(UNIT curr)
        {
            if (mind.z_current != "z_noise")
                return;

            if (new_res)
                return;

            if (mind.STATE == STATE.QUICKDECISION && mind.mem.QDCOUNT() > 0)
            {
                if (mind.mem.QDCOUNT() <= 1)
                {
                    res = curr.data == "QYES";
                    new_res = true;
                    mind.STATE = STATE.JUSTRUNNING;
                }

                mind.mem.QDREMOVE(curr);

                return;
            }

            if (!curr.IsQUICKDECISION())
                return;

            Dictionary<string, int[]> dict = mind.mindtype == MINDS.ROBERTA ? CONST.DECISIONS_R : CONST.DECISIONS_A;
            foreach (var kv in dict)
            {
                if (curr.data == kv.Key)
                    Setup(kv.Value[0], 5);
            }
        }

        private void Setup(int count, int period)
        {
            if (!CONST.SAMPLE4500.RandomSample(mind))
                return;
            
            Period = period;
            Count = 0;

            List<string> should_decision = new List<string>();

            for (int i = 0; i < count; i++)
                should_decision.Add(/*YES*/CONST.quick_deci_should_yes);

            for (int i = 0; i < count; i++)
                should_decision.Add(/*NO*/CONST.quick_deci_should_no);

            mind.mem.QDRESETU();
            mind.mem.QDRESETH();

            TONE tone = TONE.RANDOM;
            mind.mem.Decide(STATE.QUICKDECISION, CONST.MAX_UNITS, CONST.DECI_SUBJECTS[2], should_decision, UNITTYPE.QDECISION, LONGTYPE.NONE, 0, tone);
            
            mind.STATE = STATE.QUICKDECISION;
        }        
    }
}
