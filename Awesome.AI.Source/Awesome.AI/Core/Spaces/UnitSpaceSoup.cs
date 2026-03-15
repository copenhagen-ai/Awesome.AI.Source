using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
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

            PROPS props = mind.mech_high.type == MECHANICS.TUGOFWAR_HIGH ?
                PROPS.COMMUNICATION : PROPS.BRAINWAVE;

            axis = props == PROPS.COMMUNICATION ?
                [ "will", CONST.axis_2_comm] :
                [ "will", CONST.axis_2_brain];

            prev_dir ??= new Dictionary<string, double>();

            foreach (string ax in axis)
                prev_dir[ax] = 0.0d;
        }

        //each axis should run its own mechanics - update function
        // comm, andrew { "will", "opinion", "temporality", "abstraction" };
        // brain, roberta { "will", "attention", "readiness" };
        public string[] axis { get; set; }
        public int Counter {  get; set; }
        private Dictionary<string, double> prev_dir { get; set; }

        public string Axis
        {
            get 
            {
                int count = axis.Count();
                int cycles = Counter;
                int ax = cycles % count;

                return axis[ax];
            }
        }

        private double Step(string axis_tmp)
        {
            switch(axis_tmp)
            {
                case "will": return mind.mech_noise.ms.vv_sym_100;
                case CONST.axis_2_brain: return mind.mech_high.ms.vv_sym_100;
                case CONST.axis_2_comm: return mind.mech_high.ms.vv_sym_100;
                default: throw new Exception("UnitSpaceSoup, Step");
            }
        }

        private double Direction(string axis_tmp, bool _pro)
        {
            switch (axis_tmp)
            {
                case "will": return mind.down.Dir;
                case CONST.axis_2_brain: return mind.mech_high.Dir(_pro);
                case CONST.axis_2_comm: return mind.mech_high.Dir(_pro);
                default: throw new Exception("UnitSpaceSoup, Direction");
            }
        }
        
        public void CurrentUnit(bool _pro)
        {
            if (mind.z_current != "z_noise")
                return;

            //dont do this
            //if (Axis != mind.soup.axis[0])
            //    return;

            UNIT _u = mind.unit_current;
            UNIT res;

            if (_u.IsIDLE())
                res = Buffer();
            else
                res = Unit(_pro);

            mind.unit_current = res;
        }

        /*
         * priority 1
         * */
        private UNIT Unit(bool _pro)
        {
            List<UNIT> units = mind.access.UNITS_VAL();

            units = units.Where(x =>
                   mind.filters.Quick(x)                    //comment to turn off
                && mind.filters.LowCut(x, "will")           //comment to turn off
                && mind.filters.Credits(x, "will")          //comment to turn off
                ).ToList();

            if (units is null)
                throw new Exception("TheSoup, Unit");

            UNIT _u = Jump(units, _pro);

            return _u;
        }

        private UNIT Jump(List<UNIT> units, bool _pro)
        {
            if (units is null)
                throw new ArgumentNullException();

            int axis_max = 2;
            int axis_now = 2;

            double[] near = new double[axis_max];

            for (int i = 0; i < axis_max; i++)
            {
                if (i >= axis_now) {
                    near[i] = -1d;
                    continue;
                }
                
                near[i] = Near(axis[i], Step(axis[i]), Direction(axis[i], _pro));
            }

            double near_x = near[0];
            double near_y = near[1];
            double min_distance = 10E20d;
            UNIT nearest_unit = null;

            foreach (UNIT unit in units)
            {
                if (unit == mind.unit_current)
                    continue;

                double nearest_x = Near("will", Map(unit), Direction("will", _pro));
                double nearest_y = Near(axis[1], Map(unit), Direction(axis[1], _pro));

                double distance = mind.calc.Pyth(near_x, nearest_x, near_y, nearest_y);

                if (distance < min_distance)
                {
                    min_distance = distance;
                    nearest_unit = unit;
                }
            }

            if (nearest_unit == null)
                return UNIT.CreateIdle(mind);

            GPTVector2D v_near = new GPTVector2D(near[0], near[1], null, null);
            double dist = Math.Abs(min_distance);

            nearest_unit.Update(v_near, _pro);

            return nearest_unit;
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

        private double Near(string ax, double step, double dir)
        {
            //double norm = 100.0d - mind.mech_current.mp.p_100;

            //mind.down.Current = "noise";

            //double norm = 100.0d - mind.down.WillNorm;

            double res = 0.0d;
            bool same = dir == prev_dir[ax];

            if (same)
                res = step;

            if (!same)
                res = 100.0d - step;

            prev_dir[ax] = dir;

            return res;
        }

        /*
         * priority 2
         * */
        private UNIT Buffer()
        {
            /*
             * with more HUBS and UNITS added, this buffer wil be used less often
             * */

            List<UNIT> units = mind.access.UNITS_ALL();
            units = units.Where(x =>
                    //mind.filters.UnitIsValid(x) 
                       mind.filters.Quick(x)
                    && mind.filters.Credits(x, "will")
                    && mind.filters.LowCut(x, "will")
                    ).OrderByDescending(x => x.Variable).ToList();

            int rand = mind.rand.MyRandomInt(1, units.Count - 1)[0];
            UNIT _u = units.Any() ? units[rand] : UNIT.CreateIdle(mind);
            return _u;
        }
    }
}
