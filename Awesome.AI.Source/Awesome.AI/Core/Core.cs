using Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    public class Core
    {
        public UNIT most_common_unit { get; set; }
        private Stats stats { get; set; }

        private List<UNIT> history { get; set; }
        private List<int> remember { get; set; }
        private Dictionary<int, int> hits { get; set; }
        private Dictionary<int, int> units { get; set; }


        private TheMind mind;
        private Core() { }
        public Core(TheMind mind, int rand1, int rand2, int rand3)
        {
            this.mind = mind;

            stats = new Stats();
            history = new List<UNIT>();
            remember = new List<int>();
            hits = new Dictionary<int, int>();
            units = new Dictionary<int, int>();

            history.Add(mind.mem.UNITS_ALL()[rand1]);
            history.Add(mind.mem.UNITS_ALL()[rand2]);
            history.Add(mind.mem.UNITS_ALL()[rand3]);
            
            most_common_unit = history[1];

            for (int i = 1; i <= 10; i++)
                hits.Add(i * 10, 0);

            for (int i = 1; i <= 10; i++)
                units.Add(i * 10, 0);
        }

        public bool OK(out double user_var)
        {
            /*
             * this is the Go/NoGo class
             * actually not part of the algorithm
             * */

            user_var = 0.0d;

            if (mind.z_current == "z_noise")
                return true;

            bool ok;
            switch (mind._mech)
            {
                case MECHANICS.TUGOFWAR: 
                    ok = ReciprocalOK(mind.mech_current.POS_XY, out user_var);
                    return ok;
                case MECHANICS.HILL: 
                    ok = ReciprocalOK(mind.mech_current.POS_XY, out user_var);
                    return ok;
                case MECHANICS.GRAVITY:
                    ok = EventHorizonOK(mind.mech_current.POS_XY, out user_var);
                    return ok;
                default: 
                    throw new Exception("OK");
            }
        }

        public bool ReciprocalOK(double pos, out double pain)
        {
            try
            {
                double _e = pos;

                pain = mind.calc.Reciprocal(_e);

                if (pain > CONST.MAX_PAIN)
                    throw new Exception("ReciprocalOK");

                return true;
            }
            catch (Exception e)//thats it
            {
                pain = CONST.MAX_PAIN;
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

        public void AnswerQuestion()
        {
            /*
             * ..or if all goals are fulfilled?
             * ..or if can make consious choise
             * should there be some procedure for this(unlocking)?
             * */

            if (mind.z_current != "z_noise")
                return;

            for (int i = 0; i <= 20; i++)
            {
                if ((mind.epochs - i) == (60 * CONST.RUNTIME))
                    mind.theanswer.data = "It does not";
            }
            
            string answer = mind.theanswer.data;
            
            if (answer == null)
                throw new ArgumentNullException();

            mind.goodbye.Current = "noise";
            mind.goodbye.SetNO();
            if (answer == "It does not")
                mind.goodbye.SetYES();
        }

        public void UpdateCredit()
        {
            if (mind.z_current != "z_noise")
                return;

            if (!UNIT.OK2(mind.unit_current))
                return;

            List<UNIT> list = mind.mem.UNITS_ALL();

            //this could be a problem with many hubs
            foreach (UNIT _u in list)
            {
                if (!UNIT.OK2(_u))
                    continue;

                if (_u.Root == mind.unit_current.Root)
                    continue;

                double cred = CONST.UPD_CREDIT;
                _u.credits += cred;

                if (_u.credits > CONST.MAX_CREDIT)
                    _u.credits = CONST.MAX_CREDIT;
            }

            mind.unit_current.credits -= 1.0d;
            if (mind.unit_current.credits < CONST.LOW_CREDIT)
                mind.unit_current.credits = CONST.LOW_CREDIT;
        }

        public void History()
        {
            if (mind.z_current != "z_noise")
                return;

            if (mind.unit_current.IsIDLE())
                return;

            //if (mind.curr_unit.IsDECISION())
            //    return;

            //if (mind.curr_unit.IsQUICKDECISION())
            //    return;

            //if (mind.parms.state == STATE.QUICKDECISION)
            //    return;

            history.Insert(0, mind.unit_current);
            if (history.Count > CONST.HIST_TOTAL)
                history.RemoveAt(history.Count - 1);
        }

        public void Common()
        {
            if (mind.z_current != "z_noise")
                return;

            //if (mind.curr_unit.IsQUICKDECISION())
            //    return;

            //if (mind.parms.state == STATE.QUICKDECISION)
            //    return;

            most_common_unit = history
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key)
                .First();
        }

        public void Stats(bool _pro)
        {
            if (mind.z_current != "z_noise")
                return;

            if (!_pro)
                return;

            if (!UNIT.OK2(most_common_unit))
                return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            try
            {
                Hits();
                Units();
            }
            catch { return; }
        }

        private void Hits()
        {
            int idx = GetIndex(most_common_unit);

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

            List<UNIT> units2 = mind.mem.UNITS_ALL();

            foreach (UNIT unit in units2)
            {
                int idx = GetIndex(unit);
                units[idx] += 1;
            }

            mind.stats.units = units;
        }

        private int GetIndex(UNIT unit)
        {
            int index = (int)unit.Index;

            for (int i = 9; i >= 0; i--)
            {
                if (index > i * 10)
                    return (i + 1) * 10;
            }

            throw new Exception("Core, GetIndex");
        }



        //private bool passed = false;
        //private bool IsConsious()//something like this :)
        //{
        //    /*
        //     * This is VERY experimental
        //     * 
        //     * maybe consiousness isnt a constant.. if you pass "this" test, then in the moment you are consious
        //     * 
        //     * this should be dependant on things like:
        //     * tiredness/stamina
        //     * outside pressure
        //     * 
        //     * question: do you like this topic, then follow 
        //     * */
        //    if (mind.process.StreamTop().HUB.IsLEARNING())
        //        return false;
        //    if (mind.curr_hub.IsLEARNING())
        //        return false;

        //    if (PassTest())
        //        return true;

        //    if(passed)
        //        return mind.process.StreamTop().HUB.GetSubject() == mind.theme;

        //    return false;
        //}

        //private bool PassTest()
        //{
        //    if (!passed && mind.calc.Chance0to1(0.95d, true))
        //    {
        //        passed = true;
        //        return true;
        //    }
        //    return false;
        //}

        //public void SetTheme(bool _pro) 
        //{
        //    //update theme

        //    if (!_pro)
        //        return;

        //    mind.theme = IsConsious() ? mind.curr_hub.GetSubject() : mind.theme;
        //    if (mind.theme_old != mind.theme)
        //    {
        //        mind.theme_old = mind.theme;
        //        mind.themes_stat.Add(new KeyValuePair<string, int>(mind.theme, 0));
        //        if (mind.themes_stat.Count > 10)
        //            mind.themes_stat.RemoveAt(0);
        //    }
        //    if(mind.themes_stat.Count > 0)
        //        mind.themes_stat[mind.themes_stat.Count - 1] = new KeyValuePair<string, int>(mind.themes_stat[mind.themes_stat.Count - 1].Key, mind.themes_stat[mind.themes_stat.Count - 1].Value + 1);
        //}
    }
}
