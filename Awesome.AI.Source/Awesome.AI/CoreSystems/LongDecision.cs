using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;

namespace Awesome.AI.CoreSystems
{
    public class LongDecision
    {
        public Dictionary<string, string> Result = new Dictionary<string, string>();
        public Dictionary<string, int> State = new Dictionary<string, int>();

        List<double> index = new List<double>();
        double thres_h { get => 40.0d; }
        double thres_l { get => 30.0d; }
        enum ACTION { ACTION, DECLINE, NOACTION }


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
            if (mind.z_current != "z_noise")
                return;

            if (!_pro)
                return;

            if (mind.epochs < 2)
                return;

            index.Add(mind.unit_current.Index);

            if (index.Count > 10)
                index.RemoveAt(0);

            if (!mind.unit_current.IsDECISION())
                return;

            if (mind.unit_current.long_deci_type.ToString() != type.ToUpper())
                return;

            if (type == "ask" && mind.chat_asked)
                return;

            if (type == "ask" && State["answer"] > 0)
                return;

            ACTION action = GetAction();
            UNIT current = mind.unit_current;
            string subject = current.HUB?.subject ?? "";
            
            if (subject == "long_decision_should" && State[type] == 0)
            {
                //location
                if (current.data == "A" && action == ACTION.ACTION)
                    SetResult(type, "", 1);

                //if (current.data == "A" && action == ACTION.DECLINE)
                //    SetResult(type, "", 0);

                //if (current.data == "A" && action == ACTION.NOACTION)
                //    SetResult(type, "", 0);

                
                //answer
                if (current.data == "B" && action == ACTION.ACTION)
                    SetResult(type, ":YES", 0);

                if (current.data == "B" && action == ACTION.DECLINE)
                    SetResult(type, "Im busy right now..", 0);

                if (current.data == "B" && action == ACTION.NOACTION)
                    SetResult(type, "", 0);


                //ask
                if (current.data == "C" && action == ACTION.ACTION)
                    SetResult(type, GetSubject(), 0);
            }

            if (subject == "long_decision_what" && State[type] == 1)
            {
                string res = "";
                
                if (mind.down.IsNo)
                    res = current.data.Replace("WHAT", "");

                SetResult(type, res, 0);                
            }
        }

        private void SetResult(string type, string res, int state)
        {
            if(res != "")
                Result[type] = res;

            if(state == 0)
                State[type] = 0;
            
            if (state == 1)
                State[type] = 1;
        }

        private ACTION GetAction() 
        { 
            double avg = index.Average();
            if (avg > thres_h) 
                return ACTION.ACTION;
            if (avg < thres_l) 
                return ACTION.DECLINE;
            else 
                return ACTION.NOACTION;
        }

        private string GetSubject()
        {
            HUB _hub = null;
            List<HUB> list = mind.mem.HUBS_ALL(mind.STATE);
            int count = list.Count;
            int i = 0;
            int[] _r = mind.rand.MyRandomInt(100, count - 1);

            do
            {
                _hub = list[_r[i]];
                i++;
            } while (CONST.DECI_SUBJECTS.Contains(_hub.subject));

            return _hub.subject;
        }

        //public void _Decide(bool _pro, string type)
        //{
        //    /*
        //     * OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
        //     * if OCCU == DecisionHUB
        //     *     if USTAT == "NO"
        //     *         OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
        //     *     if USTAT == "YES"
        //     *         OCCU = [ PosibillitiesHUB ]
        //     * else if OCCU == PosibillitiesHUB
        //     *     move to USTAT.data
        //     *     OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
        //     * else
        //     *     if USTAT == "move location"
        //     *         OCCU = [ DecisionHUB ]
        //     *     else
        //     *         OCCU = OCCU
        //     */

        //    if (mind.z_current != "z_noise")
        //        return;

        //    if (!_pro)
        //        return;

        //    if (mind.epochs < 5)
        //        return;

        //    if (!mind.unit_current.IsDECISION())
        //        return;

        //    if (mind.unit_current.long_deci_type.ToString() != type.ToUpper())
        //        return;

        //    if (type == "ask" && mind.chat_asked)
        //        return;

        //    if (type == "ask" && _State["answer"] > 0)
        //        return;

        //    UNIT current = mind.unit_current;
        //    string subject = current.HUB?.subject ?? "";

        //    List<UNIT> units = mind.mem.UNITS_ALL().Where(x => x.IsDECISION()).ToList();
        //    HUB _1 = mind.mem.HUBS_SUB(mind.STATE, CONST.DECI_SUBJECTS[0]);
        //    HUB _2 = mind.mem.HUBS_SUB(mind.STATE, CONST.DECI_SUBJECTS[1]);

        //    if (CONST.SAMPLE50.RandomSample(mind))
        //    {
        //        mind.mem.Randomize(_1);
        //        mind.mem.Randomize(_2);
        //    }

        //    if (subject == "long_decision_should" && _State[type] == 0)
        //    {
        //        if (current.data == "AYES")
        //            _State[type]++;

        //        if (current.data == "ANO")
        //            _State[type] = 0;

        //        if (current.data == "BYES")
        //            _Result[type] = ":YES";

        //        if (current.data == "BNO")
        //            _Result[type] = "Im busy right now..";
        //        //State[type]++;

        //        if (current.data == "CYES")
        //        {
        //            HUB _hub = null;
        //            List<HUB> list = mind.mem.HUBS_ALL(mind.STATE);
        //            int count = list.Count;
        //            int i = 0;
        //            int[] _r = mind.rand.MyRandomInt(100, count - 1);

        //            do
        //            {
        //                _hub = list[_r[i]];
        //                i++;
        //            } while (CONST.DECI_SUBJECTS.Contains(_hub.subject));

        //            _Result[type] = "" + _hub.subject;
        //            _State[type] = 0;
        //        }
        //    }

        //    if (subject == "long_decision_what" && _State[type] == 1)
        //    {
        //        _Result[type] = mind.down.IsNo ?
        //            current.data.Replace("WHAT", "") :
        //            _Result[type];

        //        _State[type] = 0;
        //    }
        //}

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
