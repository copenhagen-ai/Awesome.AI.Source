using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.CoreHelpers
{
    public class QuickDecision
    {
        private TheMind mind;
        private QuickDecision() { }

        public QuickDecision(TheMind mind)
        {
            this.mind = mind;
        }

        private bool Go {  get; set; }
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
                    if(Count > Period) {
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

            if(mind.parms.state == STATE.QUICKDECISION && mind.mem.QDCOUNT() > 0)
            {
                if (mind.mem.QDCOUNT() <= 1) {
                    res = curr.data == "DYES";
                    new_res = true;
                    mind.parms.state = STATE.JUSTRUNNING;
                }

                mind.mem.QDREMOVE(curr);

                Go = false;

                return;
            }

            if (!curr.IsQUICKDECISION())
                return;

            if (_pro && curr.data == Constants.whistle_decision_u1)
                Setup(10, 5);

            if(Go)
                Start(_pro);
        }

        private void Setup(int count, int period)
        {
            Go = true;            
            Period = period;
            Count = 0;

            List<string> should_decision = new List<string>();

            for (int i = 0; i < count / 2; i++)
                should_decision.Add(/*YES*/Constants.should_decision_u1);

            for (int i = 0; i < count / 2; i++)
                should_decision.Add(/*NO*/Constants.should_decision_u2);

            mind.mem.QDRESET();

            TONE tone = TONE.RANDOM;
            mind.mem.UnitsDecide(STATE.QUICKDECISION, should_decision, UNITTYPE.QDECISION, 0, tone);
            mind.mem.HubsDecide(STATE.QUICKDECISION, Constants.subject_decision[0], should_decision, UNITTYPE.QDECISION, 0, tone);
        }

        private void Start(bool _pro)
        {
            if (!_pro)
                return;

            mind.parms.state = STATE.QUICKDECISION;
        }
    }
}
