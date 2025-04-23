using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;

namespace Awesome.AI.CoreSystems
{
    public class LongDecision
    {
        public Dictionary<string, string> Result = new Dictionary<string, string>();
        public Dictionary<string, int> State = new Dictionary<string, int>();

        private TheMind mind;
        private LongDecision() { }

        public LongDecision(TheMind mind, Dictionary<string, string> dec)
        {
            this.mind = mind;

            foreach (var kv in dec)
            {
                State.Add(kv.Key, 0);
                Result.Add(kv.Key, kv.Value.Replace("WHAT", ""));
            }
        }

        public void Decide(bool _pro, string type)
        {
            /*
             * OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
             * if OCCU == DecisionHUB
             *     if USTAT == "NO"
             *         OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
             *     if USTAT == "YES"
             *         OCCU = [ PosibillitiesHUB ]
             * else if OCCU == PosibillitiesHUB
             *     move to USTAT.data
             *     OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
             * else
             *     if USTAT == "move location"
             *         OCCU = [ DecisionHUB ]
             *     else
             *         OCCU = OCCU
             */

            if (!_pro)
                return;

            if (mind.epochs < 5)
                return;

            if (!mind.unit_current.IsDECISION())
                return;

            if (mind.unit_current.long_deci_type.ToString() != type.ToUpper())
                return;

            if (type == "ask" && mind.chat_asked)
                return;

            if (type == "ask" && State["answer"] > 0)
                return;

            UNIT current = mind.unit_current;
            HUB hub = current.HUB;

            List<UNIT> units = mind.mem.UNITS_ALL().Where(x => x.IsDECISION()).ToList();
            HUB _1 = mind.mem.HUBS_SUB(mind.State, CONST.deci_subject[0]);
            HUB _2 = mind.mem.HUBS_SUB(mind.State, CONST.deci_subject[1]);

            MyRandom rand = mind.rand;
            int[] _rand = rand.MyRandomInt(1, 5);
            if (_rand[0] == 1)
            {
                mind.mem.Randomize(_1);
                mind.mem.Randomize(_2);
            }

            if (hub.subject == "long_decision_should" && State[type] == 0)
            {
                if (current.data == "AYES")
                    State[type]++;

                if (current.data == "ANO")
                    State[type] = 0;

                if (current.data == "BYES")
                    Result[type] = ":YES";

                if (current.data == "BNO")
                    Result[type] = "Im busy right now..";
                    //State[type]++;

                if (current.data == "CYES")
                {
                    HUB _hub = null;
                    List<HUB> list = mind.mem.HUBS_ALL(mind.State);
                    int count = list.Count;
                    int i = 0;
                    int[] _r = mind.rand.MyRandomInt(100, count - 1);
                    
                    do {
                        _hub = list[_r[i]];
                        i++;
                    } while (CONST.deci_subject.Contains(_hub.subject));

                    Result[type] = "" + _hub.subject;
                    State[type] = 0;
                }
            }

            if (hub.subject == "long_decision_what" && State[type] == 1)
            {
                Result[type] = mind.dir.DownHard.IsNo() ?
                    current.data.Replace("WHAT", "") :
                    Result[type];

                State[type] = 0;
            }

            //if (hub.subject == "should_decision" && State == 0)
            //{
            //    if (current.data == "SHOULDYES")
            //        State++;

            //    if (current.data == "SHOULDNO")
            //        State = 0;
            //}

            //if (hub.subject == "what_decision" && State == 1)
            //{
            //    LocationTmp = current.data;
            //    State++;
            //    return "make_decision";
            //}

            //if (hub.subject == "make_decision" && State == 2)
            //{
            //    if (current.data == "MAKEYES")
            //        LocationFinal = LocationTmp.Replace("WHAT", "");

            //    if (current.data == "MAKENO")
            //        LocationTmp = "";

            //    mind._internal.epoch_stop = -1;
            //    mind._internal.epoch_count = 0;
            //    State = 0;

            //    return "";
            //}
        }

        //private UNIT Random(string subject)
        //{
        //    HUB hub = mind.mem.HUBS_SUB(subject);

        //    List<UNIT> units = hub.units.Where(x =>
        //                        mind.filters.Credits(x)
        //                        && !mind.filters.LowCut(x)
        //                        ).OrderByDescending(x => x.Variable).ToList();

        //    int rand = mind.calc.MyRandom(units.Count - 1);

        //    if (!units.Any())
        //        throw new Exception();

        //    UNIT __u = units[rand];
        //    return __u;
        //}
    }
}
