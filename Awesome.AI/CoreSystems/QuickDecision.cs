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

        public void Run(bool _pro, UNIT curr)
        {
            if (new_res)
                return;

            if (mind.parms[mind.current].state == STATE.QUICKDECISION && mind.mem.QDCOUNT() > 0)
            {
                if (mind.mem.QDCOUNT() <= 1)
                {
                    res = curr.data == "QYES";
                    new_res = true;
                    mind.parms[mind.current].state = STATE.JUSTRUNNING;
                }

                mind.mem.QDREMOVE(curr);

                Go = false;

                return;
            }

            if (!curr.IsQUICKDECISION())
                return;

            Dictionary<string, int[]> dict = mind.mindtype == MINDS.ROBERTA ? Constants.DECISIONS_R : Constants.DECISIONS_A;
            foreach (var kv in dict)
            {
                if (_pro && curr.data == kv.Key)
                    Setup(kv.Value[0], 5);

                if (Go)
                    Start(_pro);
            }
        }

        private void Setup(int count, int period)
        {
            Go = true;
            Period = period;
            Count = 0;

            List<string> should_decision = new List<string>();

            for (int i = 0; i < count; i++)
                should_decision.Add(/*YES*/Constants.quick_deci_should_yes);

            for (int i = 0; i < count; i++)
                should_decision.Add(/*NO*/Constants.quick_deci_should_no);

            mind.mem.QDRESETU();
            mind.mem.QDRESETH();

            TONE tone = TONE.RANDOM;
            mind.mem.UnitsDecide(STATE.QUICKDECISION, should_decision, UNITTYPE.QDECISION, LONGTYPE.NONE, 0, tone);
            mind.mem.HubsDecide(STATE.QUICKDECISION, Constants.deci_subject[2], should_decision, UNITTYPE.QDECISION, 0, tone);
        }

        private void Start(bool _pro)
        {
            if (!_pro)
                return;

            mind.parms[mind.current].state = STATE.QUICKDECISION;
        }
    }
}
