using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
{
    public class LongDecision
    {
        public Dictionary<LONGTYPE, string> Result = new Dictionary<LONGTYPE, string>();
        public Dictionary<LONGTYPE, int> State = new Dictionary<LONGTYPE, int>();

        List<double> index = new List<double>();
        
        private TheMind mind;
        private LongDecision() { }

        public LongDecision(TheMind mind, Dictionary<LONGTYPE, string> dec)
        {
            this.mind = mind;

            foreach (var kv in dec)
            {
                State.Add(kv.Key, 0);
                Result.Add(kv.Key, kv.Value.Replace("WHAT", ""));
            }
        }

        public void Decide(bool _pro, LONGTYPE type)
        {
            /*
             * should this really run on unit_actual
             * */

            if (mind.z_current != "z_noise")
                return;

            if (!_pro)
                return;

            if (mind.epochs < 5)
                return;

            SetAction(mind, type);

            //if (mind.unit_current.IsQUICKDECISION())
            //    return;

            if (!mind.unit_current.IsDECISION())
                return;

            if (mind.unit_current.ld_type != type)
                return;

            if (type == LONGTYPE.ASK && mind.chat_asked)
                return;

            if (type == LONGTYPE.ASK && State[LONGTYPE.ANSWER] > 0)
                return;

            ACTION actionA = GetActionA(mind);
            ACTION actionB = GetActionB(mind);
            UNIT unit = mind.unit_current;
            string subject = unit.HUB?.Subject ?? "";

            if (mind.Roberta())
                ;

            if (mind.Andrew())
                ;

            if (subject == "long_decision_should" && State[type] == 0)
            {
                //location
                if (unit.Data == "A" && actionB == ACTION.ACTION)
                    SetResult(type, "", 1);


                //answer
                if (unit.Data == "B" && actionB == ACTION.ACTION)
                    SetResult(type, ":YES", 0);

                if (unit.Data == "B" && actionB == ACTION.DECLINE)
                    SetResult(type, "Im busy right now..", 0);

                
                //ask
                if (unit.Data == "C" && actionB == ACTION.ACTION)
                    SetResult(type, GetSubject(), 0);
            }

            if (subject == "long_decision_what" && State[type] == 1)
            {
                string _new = unit.Data.Replace("WHAT", "");

                if (mind.down.Dir > 0.0)
                    SetResult(type, "", 0);

                else if (unit.Data != "WHAT" + Result[type])
                    SetResult(type, _new, 0);
            }
        }

        private void SetResult(LONGTYPE type, string res, int state)
        {
            if (res != "")
                Result[type] = res;

            if (state == 0)
                State[type] = 0;

            if (state == 1)
                State[type] = 1;
        }

        double thres { get { switch (mind.mindtype) { case MINDS.ROBERTA: return 23.0d; case MINDS.ANDREW: return 40; default: throw new Exception(); } } }
        
        private void SetAction(TheMind mind, LONGTYPE type)
        {
            if (type != LONGTYPE.LOCATION) //only want the first
                return;

            index.Add(mind.unit_current.UnitIndex);

            if (index.Count > 10)
                index.RemoveAt(0);            
        }

        private ACTION GetActionA(TheMind mind)
        {
            if (mind.Roberta())
                ;

            if (mind.Andrew())
                ;

            double avg = index.Average();

            if (avg > thres)
                return ACTION.ACTION;

            return ACTION.DECLINE;            
        }

        private ACTION GetActionB(TheMind mind)
        {
            if (mind.Roberta())
                ;

            if (mind.Andrew())
                ;

            double avg = index.Average();
            double curr = mind.unit_current.UnitIndex;

            if (curr >= avg)
                return ACTION.ACTION;

            return ACTION.DECLINE;
        }

        private string GetSubject()
        {
            string _hub = null;
            List<string> list = mind._internal.Occu.values;
            int count = list.Count;
            int i = 0;
            int[] _r = mind.rand.MyRandomInt(100, count - 1);

            do
            {
                _hub = list[_r[i]];
                i++;
            } while (CONST.DECI_SUBJECT_CONTAINS(_hub));

            return _hub;
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
