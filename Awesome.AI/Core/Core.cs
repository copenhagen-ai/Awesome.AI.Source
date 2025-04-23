using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    public class Core
    {
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

            if (mind.current == "noise")
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

            if (mind.current == "noise")
                return;

            for (int i = 0; i <= 10; i++)
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
            //if (mind.UnitCurrent.IsIDLE())
            //    return;

            if (mind.unit_current.IsQUICKDECISION())
                return;

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
