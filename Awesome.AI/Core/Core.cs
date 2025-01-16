using Awesome.AI.Common;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Core
{
    public class TheCurve
    {
        /*
         * this is the Go/NoGo class
         * actually not part of the algorithm
         * */
        TheMind mind;
        private TheCurve() { }
        public TheCurve(TheMind mind)
        {
            this.mind = mind;
        }

        public bool OK(out double pain)
        {
            try
            {
                double _e = mind.parms._mech.Result();
                pain = mind.calc.Reciprocal(_e);

                if (pain > 1000.0)
                    throw new Exception();
                    
                return pain < mind.parms.max_pain;
            }
            catch (Exception e)//thats it
            {
                pain = mind.parms.max_pain;
                return false;
            }
        }
    }

    public class Core
    {
        TheMind mind;
        private Core() { }
        public Core(TheMind mind)
        {
            this.mind = mind;
        }

        public void UpdateCredit()
        {
            //List<UNIT> list = mind.mem.UNITS_VAL();
            List<UNIT> list = mind.mem.UNITS_ALL();

            //this could be a problem with many hubs
            foreach (UNIT _u in list)
            {
                if (_u.root == mind.curr_unit.root)
                    continue;

                double nrg = mind.parms.update_nrg;
                _u.credits += nrg;

                if (_u.credits > _u.max_nrg)
                    _u.credits = _u.max_nrg;
            }

            mind.curr_unit.credits -= 1.0d;
            if (mind.curr_unit.credits < 0.0d)
                mind.curr_unit.credits = 0.0d;
        }
        
        public double FrictionCoefficient(bool is_static, double credits)
        {
            //should friction be calculated from position???

            if (is_static)
                return mind.parms.base_friction;
            
            Calc calc = new Calc(mind);
            double x = credits;
            double friction = 0.0d;

            switch(mind.mech)
            {
                //this could be better, use sigmoid/logistic
                case MECHANICS.HILL: friction = calc.Linear(x, -0.5d, 7d) / 10; break;
                case MECHANICS.CONTEST: friction = calc.Linear(x, -0.25d, 2.5d) / 10; break;
                default: throw new Exception();
            }

            if (friction < 0.0d)
                friction = 0.0d;

            if (friction > 10.0d)
                friction = 10.0d;

            return friction;
        }

        public void AnswerQuestion()
        {
            /*
             * ..or if all goals are fulfilled?
             * ..or if can make consious choise
             * should there be some procedure for this(unlocking)?
             * */

            if ((mind.epochs - 0) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 1) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 2) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 3) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 4) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 5) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 6) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 7) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 8) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 9) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            if ((mind.epochs - 10) == (60 * mind.parms.runtime))
                mind.theanswer.root = "It does not";

            string answer = mind.theanswer.root;
            if (answer == null)
                throw new ArgumentNullException();

            mind.goodbye = answer == "It does not" ? THECHOISE.YES : THECHOISE.NO;
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
