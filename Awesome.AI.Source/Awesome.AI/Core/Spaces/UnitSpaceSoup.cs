using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Spaces
{
    public class UnitSpaceSoup
    {
        private TheMind mind;
        private UnitSpaceSoup() { }
        public UnitSpaceSoup(TheMind mind)
        {
            this.mind = mind;

            prev_dir ??= new Dictionary<string, double>();

            foreach (string ax in axis)
                prev_dir[ax] = 0.0d;

        }

        //each axis should run its own mechanics - update function
        public string[] axis = { "will" };
        private Dictionary<string, double> prev_dir { get; set; }

        public string Axis
        {
            get 
            {
                int count = axis.Count();
                int cycles = mind.cycles_all;
                int ax = cycles % count;

                return axis[ax];
            }
        }

        public string axis_tmp { get; set; }
        private double Step
        {
            get
            {
                switch (axis_tmp)
                {
                    case "will": return mind.mech_current.ms.vv_sym_100;
                    default: throw new Exception("UnitSpaceSoup, Step");
                }
            }
        }

        private double Direction
        {
            get
            {
                switch (axis_tmp)
                {
                    case "will": return mind.down.Dir;
                    default: throw new Exception("UnitSpaceSoup, Direction");
                }
            }
        }

        private string Mech
        {
            get
            {
                switch (Axis)
                {
                    case "will": return "z_noise";
                    default: throw new Exception("UnitSpaceSoup, Mech");
                }
            }
        }        

        public void CurrentUnit()
        {
            if (mind.z_current != "z_noise")
                return;

            //dont do this
            //if (Axis != mind.soup.axis[0])
            //    return;

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
                   mind.filters.LowCut(x, Axis)          //comment to turn off
                && mind.filters.Credits(x, Axis)         //comment to turn off
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

            int axis_max = 2;
            int axis_now = 1;

            double[] axis_near = new double[axis_max];

            for (int i = 0; i < axis_max; i++)
            {
                if (i >= axis_now) {
                    axis_near[i] = -1d;
                    continue;
                }
                
                axis_tmp = axis[i];
                axis_near[i] = Near(Step, Direction);
            }

            double near_x = axis_near[0];
            double near_y = 0.0d;
            double min_distance = 10E10d;
            UNIT nearest_unit = null;

            foreach (UNIT unit in units)
            {
                if (unit == mind.unit_current)
                    continue;

                double nearest_x = Map(unit);
                double nearest_y = 10E-10d;

                double distance = mind.calc.Pyth(near_x, nearest_x, near_y, nearest_y);

                if (distance < min_distance)
                {
                    min_distance = distance;
                    nearest_unit = unit;
                }
            }

            GPTVector2D v_near = new GPTVector2D(axis_near[0], axis_near[1], null, null);
            double dist = Math.Abs(min_distance);
            nearest_unit.Update(v_near, dist);

            return nearest_unit;
        }

        public double Map(UNIT x)
        {
            //is this ok?
            //return x.Variable;

            //if (mind.z_current == "z_mech")
            //    return x.Variable;

            string _m = Mech;

            IMechanics mech = mind.mech[_m];

            mech.Peek(x);

            double norm = mech.ms.peek_sym_norm;

            return norm;
        }

        private double Near(double step, double dir)
        {
            //double norm = 100.0d - mind.mech_current.mp.p_100;

            //mind.down.Current = "noise";

            //double norm = 100.0d - mind.down.WillNorm;

            double res = 0.0d;
            bool same = dir == prev_dir[Axis];

            if (same)
                res = step;

            if (!same)
                res = 100.0d - step;

            prev_dir[Axis] = dir;

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
                                   mind.filters.Credits(x, Axis)
                                && mind.filters.LowCut(x, Axis)
                                ).OrderByDescending(x => x.Variable).ToList();

            int rand = mind.rand.MyRandomInt(1, units.Count - 1)[0];
            UNIT _u = units.Any() ? units[rand] : UNIT.CreateIdle(mind);
            return _u;
        }
    }
}
