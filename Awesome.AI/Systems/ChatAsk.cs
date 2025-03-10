using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;

namespace Awesome.AI.Systems
{
    public class ChatAsk
    {
        public string Subject { get; set; }
        //public string Index { get; set; }
        public int AskState { get; set; }

        private TheMind mind;
        private ChatAsk() { }

        public ChatAsk(TheMind mind, string starting)
        {
            this.mind = mind;
            AskState = 0;
        }

        public void Decide(bool _pro)
        {
            if (!_pro)
                return;

            if (mind.epochs < 5)
                return;

            if (!mind.curr_unit.IsDECISION())
                return;

            if (mind.chat_asked)
                return;

            if (mind.chatans.ChatState > 0)
                return;

            UNIT current = mind.curr_unit;
            HUB hub = current.HUB;

            HUB _1 = mind.mem.HUBS_SUB(mind.parms.state, Constants.subject_decision[4]);

            MyRandom rand = mind.rand;
            int[] _rand = rand.MyRandomInt(1, 5);
            if (_rand[0] == 1)
                mind.mem.Randomize(_1);

            //_rand = rand.MyRandomInt(1, 5 + 1);
            //if (_rand[0] < 5)
            //    return;

            if (hub.subject == "ask_should_decision" && AskState == 0)
            {
                if (current.data == "ASKYES")
                {
                    HUB _hub = null;
                    List<HUB> list = mind.mem.HUBS_ALL(mind.parms.state);
                    int count = list.Count;
                    int i = 0;
                    int[] _r = mind.rand.MyRandomInt(100, count - 1);
                    do
                    {
                        _hub = list[_r[i]];
                        i++;
                    }
                    while (Constants.subject_decision.Contains(_hub.subject));

                    //Subject = mind.parms._mech.dir.Choise.IsNo() ? "" + _hub.subject : ". . . .";
                    Subject = "" + _hub.subject;
                    AskState = 0;
                }
            }
        }
    }
}
