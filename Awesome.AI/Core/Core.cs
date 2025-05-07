using Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    public class Core
    {
        public UNIT most_common_unit;
        private List<UNIT> u_history = new List<UNIT>();
        private List<string> remember = new List<string>();
        private Dictionary<string, int> hits = new Dictionary<string, int>();

        private TheMind mind;
        private Core() { }
        public Core(TheMind mind)
        {
            this.mind = mind;
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
                    ok = EventHorizonOK(mind.pos.Pos, out user_var);
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

            mind.goodbye = answer == "It does not" ? HARDDOWN.YES : HARDDOWN.NO;
        }

        public void UpdateCredit()
        {
            if (mind.z_current != "z_noise")
                return;

            if (mind.unit_current.IsQUICKDECISION())
                return;

            //if (mind.UnitCurrent.IsIDLE())
            //    return;

            List<UNIT> list = mind.mem.UNITS_ALL();

            //this could be a problem with many hubs
            foreach (UNIT _u in list)
            {
                //if (_u.IsIDLE())
                //    continue;

                if (_u.IsQUICKDECISION())
                    continue;

                if (_u.Root == mind.unit_current.Root)
                    continue;

                double cred = mind.parms_current.update_cred;
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

            if (!u_history.Any())
            {
                u_history.Add(mind.mem.UNITS_RND(1));
                u_history.Add(mind.mem.UNITS_RND(2));
                u_history.Add(mind.mem.UNITS_RND(3));
            }

            u_history.Insert(0, mind.unit_current);
            if (u_history.Count > CONST.HIST_TOTAL)
                u_history.RemoveAt(u_history.Count - 1);
        }

        public void Common()
        {
            if (mind.z_current != "z_noise")
                return;

            //if (mind.curr_unit.IsQUICKDECISION())
            //    return;

            //if (mind.parms.state == STATE.QUICKDECISION)
            //    return;

            most_common_unit = u_history
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

            if (most_common_unit.IsNull())
                return;

            if (mind.unit_current.IsQUICKDECISION())
                return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            Stats stats = new Stats();

            List<UNIT> units = mind.mem.UNITS_ALL();

            foreach (UNIT u in units)
                stats.list.Add(new Stat() { _name = u.Root, _var = u.Variable, _index = u.Index });

            string nam = most_common_unit.Root;

            try
            {
                Stat _s_curr = stats.list.Where(x => x._name == nam).First();

                if (!hits.ContainsKey(nam))
                    hits.Add(nam, 0);

                hits[nam] += 1;

                stats.curr_name = nam;

                _s_curr.hits = hits[nam];

                remember.Insert(0, nam);
                if (remember.Count > CONST.REMEMBER)
                {
                    string name = remember.Last();

                    Stat _s_reset = stats.list.Where(x => x._name == name).First();

                    hits[name] -= 1;

                    stats.reset_name = name;

                    _s_reset.hits = hits[name];

                    remember.RemoveAt(remember.Count - 1);
                }

                mind.stats = stats;
            }
            catch { return; }
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
