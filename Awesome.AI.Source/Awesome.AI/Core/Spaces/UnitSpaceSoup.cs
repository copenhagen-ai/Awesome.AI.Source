using Awesome.AI.Interfaces;

namespace Awesome.AI.Core.Spaces
{
    public class UnitSpaceSoup
    {
        private TheMind mind;
        private UnitSpaceSoup() { }
        public UnitSpaceSoup(TheMind mind)
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
            List<UNIT> units = mind.access.UNITS_VAL();

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

            double map = Map(res);
            res.Update(near, map);

            return res;
        }

        public double Map(UNIT x)
        {
            //is this ok?
            //return x.Variable;

            //if (mind.z_current == "z_mech")
            //    return x.Variable;

            IMechanics mech = mind.mech["z_noise"];

            mech.Peek(x);

            double norm = mech.ms.peek_sym_norm;

            return norm;
        }

        private double prev_dir { get; set; }
        private double Near()
        {
            //double norm = 100.0d - mind.mech_current.mp.p_100;

            //mind.down.Current = "noise";

            //double norm = 100.0d - mind.down.WillNorm;

            double dir = mind.down.Dir;
            double vel = mind.mech_current.ms.vv_sym_100;
            double res = 0.0d;
            bool same = dir == prev_dir;

            if (same)
                res = vel;

            if (!same)
                res = 100.0d - vel;

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

            List<UNIT> units = mind.access.UNITS_VAL().Where(x =>
                                   //mind.filters.UnitIsValid(x) 
                                   mind.filters.Credits(x)
                                && mind.filters.LowCut(x)
                                ).OrderByDescending(x => x.Variable).ToList();

            int rand = mind.rand.MyRandomInt(1, units.Count - 1)[0];
            UNIT _u = units.Any() ? units[rand] : UNIT.CreateIdle(mind);
            return _u;
        }
    }
}
