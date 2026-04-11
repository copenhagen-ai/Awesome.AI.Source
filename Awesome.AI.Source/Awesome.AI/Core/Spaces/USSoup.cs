using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using System.Numerics;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Spaces
{
    public class GPT
    {
        /// <summary>
        /// Returns all UNITs within a rectangular corridor connecting unitA and unitB, ordered along the corridor.
        /// </summary>
        /// <param name="mind">Reference to TheMind for accessing all UNITs.</param>
        /// <param name="unitA">Start UNIT of the corridor.</param>
        /// <param name="unitB">End UNIT of the corridor.</param>
        /// <param name="axisX">Primary axis name (like axis[0]).</param>
        /// <param name="axisY">Secondary axis name (like axis[1]).</param>
        /// <param name="width">Width of the corridor rectangle.</param>
        /// <returns>List of UNITs inside the corridor, ordered along the corridor line.</returns>
        public List<UNIT> Corridor(List<UNIT> allUnits, UNIT unitA, UNIT unitB, string axisX, string axisY, double width = 10.0)
        {
            //List<UNIT> allUnits = mind.access.UNITS_ALL();

            // 1. Convert UNITs to 2D points using provided axes
            Vector2 p1 = new Vector2((float)unitA.UIget(axisX), (float)unitA.UIget(axisY));
            Vector2 p2 = new Vector2((float)unitB.UIget(axisX), (float)unitB.UIget(axisY));

            Vector2 dir = p2 - p1;
            float length = dir.Length();
            if (length == 0) return new List<UNIT>();

            dir /= length; // normalize
            Vector2 perp = new Vector2(-dir.Y, dir.X); // perpendicular

            // 2. Filter UNITs inside corridor
            var corridorUnits = allUnits
                .Where(u =>
                {
                    Vector2 pu = new Vector2((float)u.UIget(axisX), (float)u.UIget(axisY));
                    Vector2 rel = pu - p1;

                    float along = Vector2.Dot(rel, dir);
                    float across = Vector2.Dot(rel, perp);

                    return along >= 0 && along <= length && Math.Abs(across) <= width / 2;
                })
                .Select(u => new
                {
                    Unit = u,
                    DistanceAlong = Vector2.Dot(new Vector2((float)u.UIget(axisX), (float)u.UIget(axisY)) - p1, dir)
                })
                .OrderByDescending(x => x.DistanceAlong) // closest to unitB first
                .Select(x => x.Unit)
                .ToList();

            Fix(corridorUnits, unitA, unitB);

            return corridorUnits;
        }

        public void Fix(List<UNIT> corridorUnits, UNIT unitA, UNIT unitB)
        {
            corridorUnits.Remove(unitA);
            corridorUnits.Remove(unitB);

            corridorUnits.Insert(0, unitB);
            corridorUnits.Add(unitA);
        }

        public static GPT Create() 
        { 
            return new GPT(); 
        }
    }

    public class USSoup
    {
        private TheMind mind;
        private USSoup() { }
        public USSoup(TheMind mind)
        {
            this.mind = mind;            
        }

        //each axis should run its own mechanics - update function
        // comm, andrew { "will", "opinion", "temporality", "abstraction" };
        // brain, roberta { "will", "attention", "readiness" };
        //public string[] axis { get; set; }
        
        private double Step(string axis_tmp)
        {
            switch(axis_tmp)
            {
                case "will": return mind.mech.mp.eprops.Will().Norm100(mind);
                case "attention": return mind.mech.mp.eprops.Attention().Norm100(mind);
                case "commitment": return mind.mech.mp.eprops.Commitment().Norm100(mind);
                case "adaptation": return mind.mech.mp.eprops.Adaptation().Norm100(mind);
                case "activation": return mind.mech.mp.eprops.Activation().Norm100(mind);
                case "influence": return mind.mech.mp.eprops.Influence().Norm100(mind);
                default: throw new Exception("UnitSpaceSoup, Step");
            }
        }

        private double Direction(string axis_tmp, bool set_prev)
        {
            switch (axis_tmp)
            {
                case "will": return mind.down.Dir;
                case "attention":
                case "commitment":
                case "adaptation":
                case "activation":
                case "influence":
                    return mind.mech.mp.eprops.Dir(axis_tmp, set_prev);
                default: throw new Exception("UnitSpaceSoup, Direction");
            }
        }

        private bool Quick(bool _pro)
        {
            /*
             * make logic for initiating quick decision here
             * */

            if (!_pro)
                return false;

            if (mind.STATE == STATE.QUICKDECISION)
                return false;

            if (!CONST.SAMPLE20.RandomSample(mind))
                return false;

            UNIT[] list = { mind.q_u_whistle, mind.q_u_mathlearn, mind.q_u_mathsolve, mind.q_u_arclearn/*, mind.q_u_arclearn*/, mind.q_u_arcsolve/*, mind.q_u_arcsolve*/ };

            int rand = mind.rand.MyRandomInt(1, 49)[0];
            
            mind.unit_current = list[rand % 5];

            return true;
        }
        
        public void CurrentUnit(bool _pro)
        {
            if (Quick(_pro))
                return;

            UNIT _u = mind.unit_current;
            UNIT[] res = null;

            if (_u.IsIDLE())
                res = Buffer();
            else
                res = Unit();

            mind.unit_current = res[0];
            mind.unit_corridor = res;
        }

        /*
         * priority 1
         * */
        private UNIT[] Unit()
        {
            List<UNIT> units = mind.access.UNITS_ALL();

            units = units.Where(x =>
                   mind.filters.Valid(x)                    //comment to turn off
                && mind.filters.LowCut(x, "will")           //comment to turn off
                && mind.filters.Credits(x, "will")          //comment to turn off
                ).ToList();

            if (units is null)
                throw new Exception("TheSoup, Unit");

            var near = Near();
            UNIT[] res = null;

            if (CONST.select_c == SELECTCURRENT.PYTH)
                res = SelectByPyth(units, near);

            if (CONST.select_c == SELECTCURRENT.OTHER)
                res = SelectByOther(units, near);

            if (res == null)
                throw new Exception("UnitSpaceSoup, Unit");
            
            if (res[0].IsIDLE())
                return res;

            GPTVector2D v_near = new GPTVector2D(near[0], near[1], null, null);
            
            res[0].Update(v_near);

            return res;
        }

        private double[] Near()
        {
            double[] near = new double[CONST.AXIS_MAX];

            for (int i = 0; i < CONST.AXIS_MAX; i++)
               near[i] = JumpTo(CONST.AXES[i], Step(CONST.AXES[i]), Direction(CONST.AXES[i], true));
            
            return near;
        }

        private UNIT[] SelectByPyth(List<UNIT> units, double[] near)
        {
            double near_x = near[0];
            double near_y = near[1];
            double min_distance = 10E20d;
            UNIT nearest = null;

            foreach (UNIT unit in units)
            {
                if (unit == mind.unit_current)
                    continue;

                double nearest_x = JumpTo("will", Map(unit), Direction("will", false));
                double nearest_y = JumpTo(CONST.AXES[1], Map(unit), Direction(CONST.AXES[1], false));

                double distance = mind.calc.Pyth(near_x, nearest_x, near_y, nearest_y);

                if (distance < min_distance)
                {
                    min_distance = distance;
                    nearest = unit;
                }
            }


            List<UNIT> res = new List<UNIT>() { mind.unit_current };

            if (nearest == null)
                res.Insert(0, UNIT.CreateIdle(mind));
            else
                res = GPT.Create().Corridor(units, mind.unit_current, nearest, CONST.AXES[0], CONST.AXES[1], 5.0d);

            return res.ToArray();
        }

        private UNIT[] SelectByOther(List<UNIT> units, double[] near)
        {
            throw new NotImplementedException("UnitSpaceSoup, SelectOther");
        }

        private double JumpTo(string ax, double step, double dir)
        {
            double prev_dir = mind.mech.mp.eprops.DirPrev(ax);
            double res = 0.0d;
            bool same = dir == prev_dir;

            if (same)
                res = step;

            if (!same)
                res = 100.0d - step;

            return res;
        }

        public double Map(UNIT x)
        {
            IMechanics mech = mind.mech;

            mech.Peek(x);

            double norm = mech.ms.peek_sym_norm;

            return norm;
        }

        /*
         * priority 2
         * */
        private UNIT[] Buffer()
        {
            /*
             * with more HUBS and UNITS added, this buffer wil be used less often
             * */

            List<UNIT> units = mind.access.UNITS_ALL();
            units = units.Where(x =>
                       mind.filters.Valid(x)
                    && mind.filters.Credits(x, "will")
                    && mind.filters.LowCut(x, "will")
                    ).OrderByDescending(x => x.Variable).ToList();

            int rand = mind.rand.MyRandomInt(1, units.Count - 1)[0];

            List<UNIT> _u = units.Any() ? 
                new List<UNIT>{ units[rand], mind.unit_current } : 
                new List<UNIT>{ UNIT.CreateIdle(mind), mind.unit_current };

            return _u.ToArray();
        }
    }
}
