using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Core.Spaces;
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
            string subject = mind.hub.GetSubject(unit) ?? "";

            if (mind.Roberta())
                ;

            if (mind.Andrew())
                ;

            if (subject == CONST.LSUB_SHOULD && State[type] == 0)
            {
                //location
                if (unit.Data == CONST.LDAT_LOC_SHOULD && actionB == ACTION.ACTION)
                    SetResult(type, "", 1);


                //answer
                if (unit.Data == CONST.LDAT_ANS_SHOULD && actionB == ACTION.ACTION)
                    SetResult(type, ":YES", 0);

                if (unit.Data == CONST.LDAT_ANS_SHOULD && actionB == ACTION.DECLINE)
                    SetResult(type, "Im busy right now..", 0);

                
                //ask
                if (unit.Data == CONST.LDAT_ASK_SHOULD && actionB == ACTION.ACTION)
                    SetResult(type, mind.hub.GetSubject(unit), 0);
            }

            if (subject == CONST.LSUB_WHAT && State[type] == 1)
            {
                string _new = unit.Data.Replace(CONST.lng_what, "");

                if (mind.down.Dir > 0.0)
                    SetResult(type, "", 0);

                else if (unit.Data != CONST.lng_what + Result[type])
                    SetResult(type, _new, 0);
            }
        }

        private void SetResult(LONGTYPE type, string res, int state)
        {
            if (state == 1)
                mind.reward = true;

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
    }
}
