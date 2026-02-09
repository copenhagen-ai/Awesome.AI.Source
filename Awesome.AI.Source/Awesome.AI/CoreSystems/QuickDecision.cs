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

        public void Run(bool pro, UNIT curr)
        {
            if (mind.z_current != "z_noise")
                return;

            if (new_res)
                return;

            if (!curr.IsQUICKDECISION())
                return;

            if (mind.Roberta())
                ;

            if (mind.Andrew())
                ;

            if (mind.STATE == STATE.QUICKDECISION && mind.access.QDCOUNT() > 0)
            {
                if (mind.access.QDCOUNT() == 1)
                {
                    res = curr.Data == "QYES";
                    new_res = true;
                    mind.STATE = STATE.JUSTRUNNING;

                    mind.access.QDRESETU();
                    
                    return;
                }

                mind.access.QDREMOVE(curr);

                return;
            }

            if (!pro)
                return;

            if (curr.Data == "WHISTLE")
                Setup(5, 5);
        }

        int sample_count = 0;
        private void Setup(int count, int period)
        {
            sample_count++;

            if (sample_count < 20) return;

            sample_count = 0;
            
            Period = period;
            
            Count = 0;

            List<string> should_decision = new List<string>();

            for (int i = 0; i < count; i++)
                should_decision.Add(/*YES*/CONST.quick_deci_should_yes);

            for (int i = 0; i < count; i++)
                should_decision.Add(/*NO*/CONST.quick_deci_should_no);

            mind.access.QDRESETU();
            //mind.mem.QDRESETH();

            TONE tone = TONE.RANDOM;
            mind.memory.Decide(STATE.QUICKDECISION, CONST.DECI_SUBJECT_C, should_decision, UNITTYPE.QDECISION, LONGTYPE.NONE, 0, tone);
            
            mind.STATE = STATE.QUICKDECISION;
        }
    }
}
