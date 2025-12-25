using Awesome.AI.Interfaces;

namespace Awesome.AI.Core
{
    public class TheSoup
    {
        /*
         * as it is now, it is very simple just up or down
         * 
         * expansion 1:
         * - still using up or down approach
         * - implement a theme system, and say GiveMeYourTheme and follow(follow by mood, interest, environment etc?)
         * - topic: most common HUB in related
         * 
         * expansion 2:
         * - still using up or down approach
         * - the closer to zero Calc.PythDist(UNIT.CreateMIN(), unit) is, the further up we want to jump
         * - maybe a limit or fuzzy: if Calc.PythDist(UNIT.CreateMIN(), unit) < 20.0 -> its critical -> jump far (similar to Meters.SayNO())
         * 
         * */

        private TheMind mind;
        private TheSoup() { }
        public TheSoup(TheMind mind)
        {
            this.mind = mind;
        }

        public void CurrentUnit()
        {
            if (mind.z_current != "z_noise")
                return;

            UNIT _u = mind.unit_current;

            /*
             * with more HUBS and UNITS added, this buffer wil be used less often
             * */
            
            UNIT res;

            if (_u.IsIDLE())
                res = Buffer();
            else
                res = Unit();

            mind.unit_current = res;        
        }

        /*
         * priority 1
         * */
        private UNIT Unit()
        {
            List<UNIT> units = mind.mem.UNITS_VAL();

            units = units.Where(x =>
                //   mind.filters.Direction(x) //comment to turn off
                   mind.filters.LowCut(x)          //comment to turn off
                && mind.filters.Credits(x)         //comment to turn off
                //   mind.filters.UnitIsValid(x)   //comment to turn off
                //&& mind.filters.Theme(x)         //comment to turn off
                //&& Filters.Elastic2(dir)         //comment to turn off
                //&& Filters.Ideal(x)              //comment to turn off
                //&& Filters.Neighbor(x)
                ).ToList();

            if (units is null)
                throw new Exception("TheSoup, Unit");

            UNIT _u = Jump(units);
            
            return _u;
        }

        private UNIT Jump(List<UNIT> units)
        {
            if (units is null)
                throw new ArgumentNullException();

            double near = Near();

            units = units.OrderByDescending(x => Map(x)).ToList();

            UNIT above = units.Where(x => Map(x) < near).FirstOrDefault();
            UNIT below = units.Where(x => Map(x) >= near).LastOrDefault();
            UNIT res = null;
         
            if (above is null && below is null)
                return null;

            if (above is null)
                res = below;

            if (below is null)
                res = above;

            if (res is null)
                res = near - Map(below) < Map(above) - near ? below : above;//switcharoonie

            //if (res is null)
            //    res = mind.cycles % 2 == 0 ? below : above;//switcharoonie

            UpdateUnit(near, res);

            return res;
        }

        private double Map(UNIT x)
        {
            //is this ok?
            //return x.Variable;

            //if (mind.z_current == "z_mech")
            //    return x.Variable;

            IMechanics mech = mind.mech["z_noise"];

            mech.Peek(x);

            double norm = mech.mp.peek_norm;
            
            return norm;
        }

        private void UpdateUnit(double near, UNIT res)
        {
            //return;

            //i think it makes sense only noise can update unit
            if (mind.z_current != "z_noise")
                return;

            //mind.down.Current = "noise";
            //double dir = mind.down.IsYes ? -1.0d : 1.0d;
            double dir = mind.down.Dir;
            double dist = DistAbsolute(res, near);

            res.Update(dir, near, dist);
        }

        private double DistAbsolute(UNIT unit, double near)
        {
            double _var = Map(unit);
            double res = Math.Abs(_var -  near);

            return res;
        }
        
        private double prev_dir { get; set; }
        private double Near()
        {
            //double norm = 100.0d - mind.mech_current.mp.p_100;

            //mind.down.Current = "noise";

            //double norm = 100.0d - mind.down.WillNorm;

            double dir = mind.down.Dir;
            double mom = mind.mech_current.mp.vv_100;
            double res = 0.0d;
            bool same = dir == prev_dir;

            if (same)
                res = mom;
            if (!same)
                res = 100.0d - mom;
            
            prev_dir = dir;

            return res;
        }

        /*
         * priority 2
         * */
        private UNIT Buffer()
        {
            /*
             * not sure if the buffer should be here(because of the random in topic an here)
             * but maybe it wont have such a big part later on, when more HUBS and UNITS are added
             * */
                        
            List<UNIT> units = mind.mem.UNITS_VAL().Where(x => 
                                //mind.filters.UnitIsValid(x) 
                                   mind.filters.Credits(x) 
                                && mind.filters.LowCut(x)
                                ).OrderByDescending(x => x.Variable).ToList();
            
            int rand = mind.rand.MyRandomInt(1,units.Count - 1)[0];
            UNIT _u = units.Any() ? units[rand] : UNIT.IDLE_UNIT(mind);
            return _u;
        }
    }
}
