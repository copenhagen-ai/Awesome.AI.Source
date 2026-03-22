using Awesome.AI.Common;
using Awesome.AI.Core.Spaces;
using Awesome.AI.CoreInternals;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    public class Core
    {
        private MyStats stats { get; set; }

        private List<UNIT> history { get; set; }
        private List<int> remember { get; set; }
        private Dictionary<int, int> hits { get; set; }
        private Dictionary<int, int> units { get; set; }


        private TheMind mind;
        private Core() { }
        public Core(TheMind mind, int rand1, int rand2, int rand3)
        {
            this.mind = mind;

            stats = new MyStats();
            history = new List<UNIT>();
            remember = new List<int>();
            hits = new Dictionary<int, int>();
            units = new Dictionary<int, int>();

            history.Add(mind.access.UNITS_ALL()[rand1]);
            history.Add(mind.access.UNITS_ALL()[rand2]);
            history.Add(mind.access.UNITS_ALL()[rand3]);
            
            mind.unit_actual = history[1];

            for (int i = 1; i <= 10; i++)
                hits.Add(i * 10, 0);

            for (int i = 1; i <= 10; i++)
                units.Add(i * 10, 0);
        }

        public bool OK(out double pain_truth_something)
        {
            /*
             * this is the Go/NoGo class
             * actually not part of the algorithm
             * */

            pain_truth_something = 0.0d;

            if (mind.z_current != "z_noise")
                return true;

            bool ok;
            switch (mind.bot.mech_low)
            {
                case MECHANICS.TUGOFWAR_LOW: 
                    ok = ReciprocalOK(mind.mech_current.PosXY(), out pain_truth_something);
                    return ok;
                case MECHANICS.BALLONHILL_LOW:
                    ok = ReciprocalOK(mind.mech_current.PosXY(), out pain_truth_something);
                    return ok;
                case MECHANICS.MECH_OTHER_LOW:
                    ok = EventHorizonOK(mind.mech_current.PosXY(), out pain_truth_something);
                    return ok;
                default: 
                    throw new Exception("OK");
            }
        }

        public bool ReciprocalOK(double pos, out double pain_truth_something)
        {
            try
            {
                double epsilon = 0;
                if (!mind.goodbye)
                    epsilon = CONST.EPSILON2;

                double _e = pos + epsilon;

                pain_truth_something = mind.calc.Reciprocal(_e);

                if (pain_truth_something > CONST.MAX_PAIN_TRUTH_SOMETHING)
                    throw new Exception("ReciprocalOK");

                return true;
            }
            catch (Exception e)//thats it
            {
                pain_truth_something = CONST.MAX_PAIN_TRUTH_SOMETHING;
                return false;
            }
        }

        public bool EventHorizonOK(double pos, out double time)
        {
            try
            {
                double _e = pos;

                time = mind.calc.EventHorizon(_e);

                if (time <= 0.0)
                    throw new Exception("EventHorizonOK");

                return true;
            }
            catch (Exception e)//thats it
            {
                time = 0.0d;
                return false;
            }
        }

        public void StopCondition()
        {
            /*
             * ..or if all goals are fulfilled?
             * ..or if can make consious choise
             * should there be some procedure for this (unlocking)?
             * */

            if (mind.z_current != "z_noise")
                return;

            if ((mind.epochs) >= (60 * mind.bot.RUNTIME))
                mind.theanswer.Data = "It does not";
                        
            string answer = mind.theanswer.Data;
            
            mind.goodbye = answer == "It does not" ? true : false;
        }

        public void UpdateCredit()
        {
            if (mind.z_current != "z_noise")
                return;

            if (!Filters.FilterUnit(mind, FILTERUNIT.CURRENT, FILTERTYPE.TWO))
                return;

            List<UNIT> list = mind.access.UNITS_ALL();

            //this could be a problem with many hubs
            foreach (UNIT _u in list)
            {
                if (!Filters.FilterUnit(mind, FILTERUNIT.NONE, FILTERTYPE.TWO, _u))
                    continue;

                if (_u.Root == mind.unit_current.Root)
                    continue;

                double cred = CONST.UPD_CREDIT;
                _u.credits += cred;

                if (_u.credits > CONST.MAX_CREDIT)
                    _u.credits = CONST.MAX_CREDIT;
            }

            mind.unit_current.credits -= 1.0d;
            if (mind.unit_current.credits < 0.0d)
                mind.unit_current.credits = 0.0d;
        }

        public void History()
        {
            if (mind.z_current != "z_noise")
                return;

            if (mind.unit_current.IsIDLE())
                return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            if (mind.unit_current.IsQDECISION())
                return;

            //if (mind.curr_unit.IsDECISION())
            //    return;


            history.Insert(0, mind.unit_current);
            if (history.Count > CONST.HIST_TOTAL)
                history.RemoveAt(history.Count - 1);
        }

        public void ActualUnit(bool _pro)
        {
            if (!_pro)
                return;

            if (mind.z_current != "z_noise")
                return;

            //if (mind.unit_current.IsQUICKDECISION())
            //    return;

            //if (mind.STATE == STATE.QUICKDECISION)
            //    return;

            switch (CONST.select_a) 
            {
                case SELECTACTUAL.DOMINANT:
                    UNIT unit = history
                        .GroupBy(x => x)
                        .OrderByDescending(x => x.Count())
                        .Select(x => x.Key)
                        .First();

                    mind.unit_actual = unit;
                    break;
                case SELECTACTUAL.OTHER:
                    throw new NotImplementedException("Core, ActualUnit 1");
                default: 
                    throw new NotImplementedException("Core, ActualUnit 2");
            }
        }

        public void Stats(bool _pro)
        {
            if (mind.z_current != "z_noise")
                return;

            if (!_pro)
                return;

            if (!Filters.FilterUnit(mind, FILTERUNIT.ACTUAL, FILTERTYPE.TWO))
                return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            try
            {
                Hits();
                Units();
            }
            catch (Exception _e)
            { 
                return; 
            }
        }

        private void Hits()
        {
            int idx = mind.unit_actual.Index();

            hits[idx] += 1;

            remember.Insert(0, idx);

            if (remember.Count > CONST.REMEMBER)
            {
                int res_idx = remember.Last();

                hits[res_idx] -= 1;

                remember.RemoveAt(remember.Count);
            }

            mind.stats.hits = hits;
        }

        private void Units()
        {
            units = new Dictionary<int, int>();

            for (int i = 1; i <= 10; i++)
                units.Add(i * 10, 0);

            List<UNIT> units2 = mind.access.UNITS_ALL();

            foreach (UNIT unit in units2)
            {
                int idx = unit.Index();
                units[idx] += 1;
            }

            mind.stats.units = units;
        }        
    }
}
