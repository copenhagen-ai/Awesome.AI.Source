using Awesome.AI.Core;
using Awesome.AI.Core.Spaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
{
    // A mind resolves one long-decision session at a time. Decision units are
    // created/deleted with normal units; the first eligible current unit claims
    // the session and it remains active until it resolves or cancels.
    public class LongDecision
    {
        public Dictionary<LONGTYPE, string> Result = new Dictionary<LONGTYPE, string>();
        public Dictionary<LONGTYPE, int> State = new Dictionary<LONGTYPE, int>();

        private readonly List<double> index = new List<double>();
        private readonly TheMind mind;
        private bool active;
        private LONGTYPE activeType = LONGTYPE.NONE;
        private int activeState;
        private int lastRecordedEpoch = -1;

        public bool Active => active;
        public LONGTYPE ActiveType => activeType;

        public LongDecision(TheMind mind, Dictionary<LONGTYPE, string> dec)
        {
            this.mind = mind;

            foreach (var kv in dec)
            {
                State[kv.Key] = 0;
                Result[kv.Key] = kv.Value.Replace("WHAT", "");
            }
        }

        public string GetResult(LONGTYPE type)
        {
            string result = Result[type];
            Result[type] = "";
            return result;
        }

        public void Decide(bool process, LONGTYPE type)
        {
            if (!process || mind.epochs < 5)
                return;

            RecordWill();

            UNIT unit = mind.unit_current;
            if (!unit.IsLONGDECISION() || unit.ld_type != type)
                return;

            if (!active)
            {
                if (type == LONGTYPE.ASK && mind.chat_asked)
                    return;

                if (unit.IsLDW())
                    return;

                active = true;
                activeType = type;
                activeState = 0;
                State[type] = 0;
            }

            if (activeType != type)
                return;

            Resolve(unit, type);
        }

        private void Resolve(UNIT unit, LONGTYPE type)
        {
            ACTION action = GetAction();
            string subject = mind.hub.GetSubject(unit) ?? "";

            if (subject == CONST.LSUB_SHOULD && activeState == 0)
            {
                if (unit.Data == CONST.LDAT_LOC_SHOULD && action == ACTION.ACTION)
                {
                    activeState = 1;
                    State[type] = 1;
                    mind.reward = true;
                    return;
                }

                if (unit.Data == CONST.LDAT_ANS_SHOULD)
                {
                    Complete(type, action == ACTION.ACTION ? ":YES" : "Im busy right now..");
                    return;
                }

                if (unit.Data == CONST.LDAT_ASK_SHOULD && action == ACTION.ACTION)
                {
                    Complete(type, mind.hub.GetSubject(mind.unit_actual) ?? "");
                    return;
                }
            }

            if (subject == CONST.LSUB_WHAT && activeState == 1)
            {
                if (mind.down.d_res < 0.0d)
                    End(type);
                else
                    Complete(type, unit.Data.Replace(CONST.lng_what, ""));
            }
        }

        private void Complete(LONGTYPE type, string result)
        {
            if (!string.IsNullOrEmpty(result))
                Result[type] = result;
            End(type);
        }

        private void End(LONGTYPE type)
        {
            State[type] = 0;
            active = false;
            activeType = LONGTYPE.NONE;
            activeState = 0;
        }

        private void RecordWill()
        {
            if (lastRecordedEpoch == mind.epochs)
                return;

            lastRecordedEpoch = mind.epochs;
            index.Add(mind.unit_current.UIget("will"));
            if (index.Count > 10)
                index.RemoveAt(0);
        }

        private ACTION GetAction()
        {
            double average = index.Average();
            double current = mind.unit_current.UIget("will");
            return current >= average ? ACTION.ACTION : ACTION.DECLINE;
        }
    }
}
