using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Spaces
{
    public class UnitSpaceAccess
    {
        private TheMind mind;
        private UnitSpaceAccess() { }

        public UnitSpaceAccess(TheMind mind)
        {
            this.mind = mind;            
        }

        public List<UNIT> UNITS_ALL(ORDER order = ORDER.NONE)
        {
            if (mind.STATE == STATE.JUSTRUNNING && !mind.memory.units_running.Any())
                throw new Exception("Memory, UNITS_VAL 1");

            if (mind.STATE == STATE.QUICKDECISION && !mind.memory.units_decision.Any())
                throw new Exception("Memory, UNITS_VAL 2");

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING:
                    if (order == ORDER.BYINDEX)
                        return mind.memory.units_running.OrderBy(x=>x.UnitIndex).ToList();
                    if (order == ORDER.BYVARIABLE)
                        return mind.memory.units_running.OrderBy(x=>x.Variable).ToList();
                    return mind.memory.units_running;
                case STATE.QUICKDECISION: return mind.memory.units_decision;
                default: throw new NotImplementedException();
            }
        }

        public UNIT UNIT_GUID(string guid)
        {
            UNIT res;

            if (mind.STATE == STATE.JUSTRUNNING && !mind.memory.units_running.Any())
                throw new Exception("Memory, UNITS_VAL 1");

            if (mind.STATE == STATE.QUICKDECISION && !mind.memory.units_decision.Any())
                throw new Exception("Memory, UNITS_VAL 2");

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING: res = mind.memory.units_running.Where(x => x.guid == guid).First(); break;
                default: throw new NotImplementedException();
            }

            return res;
        }

        public List<UNIT> UNITS_VAL()
        {
            List<UNIT> res;

            if (mind.STATE == STATE.JUSTRUNNING && !mind.memory.units_running.Any())
                throw new Exception("Memory, UNITS_VAL 1");

            if (mind.STATE == STATE.QUICKDECISION && !mind.memory.units_decision.Any())
                throw new Exception("Memory, UNITS_VAL 2");

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING: res = mind.memory.units_running.Where(x => x.IsValid).ToList(); break;
                case STATE.QUICKDECISION: res = mind.memory.units_decision.ToList(); break;//all are valid
                default: throw new NotImplementedException();
            }

            return res;
        }

        public UNIT UNITS_RND(int index)
        {
            int[] rand;
            UNIT _u;

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING:
                    rand = mind.rand.MyRandomInt(index, mind.memory.units_running.Count() - 1);
                    _u = mind.memory.units_running[rand[index - 1]];
                    break;
                case STATE.QUICKDECISION:
                    rand = mind.rand.MyRandomInt(index, mind.memory.units_decision.Count() - 1);
                    _u = mind.memory.units_decision[rand[index - 1]];
                    break;
                default: throw new NotImplementedException();
            }

            return _u;
        }

        public void UNITS_ADD(UNIT unit, double low, double high)
        {
            double idx = mind.rand.MyRandomDouble(1)[0];
            idx = mind.calc.Normalize(idx, 0.0d, 1.0d, low, high);

            List<string> list = mind.memory.Tags(mind.mindtype);
            int rand = mind.rand.MyRandomInt(1, list.Count)[0] + 1;

            string ticket = "" + unit.HUB.Subject + rand;
            string guid = "" + unit.guid;

            UNIT _u = UNIT.Create(mind, guid, idx, "DATA", ticket, UNITTYPE.JUSTAUNIT, LONGTYPE.NONE);

            mind.memory.units_running.Add(_u);
        }

        public void UNITS_REM(UNIT unit, double low, double high)
        {
            List<UNIT> list = UNITS_ALL().Where(x => x.Variable > low && x.Variable < high).ToList();
            list = list.Where(x => x.created < unit.created).ToList();

            foreach (UNIT _u in list)
                mind.memory.units_running.Remove(_u);            
        }
        
        public void QDRESETU() => mind.memory.units_decision = new List<UNIT>();
        public void QDREMOVE(UNIT curr) => mind.memory.units_decision.Remove(curr);
        public int QDCOUNT() => mind.memory.units_decision.Count();        
    }
}
