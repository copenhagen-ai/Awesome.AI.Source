using Awesome.AI.Common;
using Awesome.AI.Source.Awesome.AI.Core.Internals;
using Awesome.AI.Variables;
using System.Numerics;
using System.Xml.Schema;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Spaces
{
    public class GPT
    {
        public List<UNIT> Corridor2D(List<UNIT> allUnits, UNIT unitA, UNIT unitB)
        {
            string axisX = CONST.AXES[0];
            string axisY = CONST.AXES[1];
            double width = 5.0;

            GPTVector2D func = new GPTVector2D();

            // 1. Convert UNITs to 2D points using provided axes
            GPTVector2D p1 = new GPTVector2D((float)unitA.UIget(axisX), (float)unitA.UIget(axisY), null, null);
            GPTVector2D p2 = new GPTVector2D((float)unitB.UIget(axisX), (float)unitB.UIget(axisY), null, null);

            GPTVector2D dir = func.Sub(p2, p1);
            double length = dir.magnitude;
            if (length == 0) return new List<UNIT>();

            dir = func.Div(dir, length); // normalize
            GPTVector2D perp = new GPTVector2D(-dir.yy, dir.xx, null, null); // perpendicular

            // 2. Filter UNITs inside corridor
            var corridorUnits = allUnits
                .Where(u =>
                {
                    GPTVector2D pu = new GPTVector2D((float)u.UIget(axisX), (float)u.UIget(axisY), null, null);
                    GPTVector2D rel = func.Sub(pu, p1);

                    double along = func.Dot(rel, dir);
                    double across = func.Dot(rel, perp);

                    return along >= 0 && along <= length && Math.Abs(across) <= width / 2;
                })
                .Select(u => new
                {
                    Unit = u,
                    DistanceAlong = func.Dot(func.Sub(new GPTVector2D((float)u.UIget(axisX), (float)u.UIget(axisY), null, null), p1), dir)
                })
                .OrderByDescending(x => x.DistanceAlong) // closest to unitB first
                .Select(x => x.Unit)
                .ToList();

            Fix2D(corridorUnits, unitA, unitB);

            if (corridorUnits.Count == 0)
                throw new Exception("USSoup, Corridor");

            return corridorUnits;
        }

        public List<UNIT> Corridor6D(List<UNIT> allUnits, UNIT unitA, UNIT unitB)
        {
            string axisX = CONST.AXES[0];
            string axisY = CONST.AXES[1];
            string axisZ = CONST.AXES[2];
            string axisW = CONST.AXES[3];
            string axisV = CONST.AXES[4];
            string axisU = CONST.AXES[5];

            double width = 5.0;

            GPTVector6D func = new GPTVector6D();

            GPTVector6D ToVector(UNIT unit)
            {
                return new GPTVector6D(
                    unit.UIget(axisX),
                    unit.UIget(axisY),
                    unit.UIget(axisZ),
                    unit.UIget(axisW),
                    unit.UIget(axisV),
                    unit.UIget(axisU));
            }

            GPTVector6D p1 = ToVector(unitA);
            GPTVector6D p2 = ToVector(unitB);

            GPTVector6D dir = func.Sub(p2, p1);
            double length = dir.magnitude;

            if (length == 0)
                return new List<UNIT>();

            dir = func.Div(dir, length);

            var corridorUnits = allUnits
                .Select(unit =>
                {
                    GPTVector6D point = ToVector(unit);
                    GPTVector6D relative = func.Sub(point, p1);

                    double distanceAlong = func.Dot(relative, dir);

                    GPTVector6D closestPointOffset =
                        func.Mul(dir, distanceAlong);

                    GPTVector6D perpendicularOffset =
                        func.Sub(relative, closestPointOffset);

                    double distanceAcross = perpendicularOffset.magnitude;

                    return new
                    {
                        Unit = unit,
                        DistanceAlong = distanceAlong,
                        DistanceAcross = distanceAcross
                    };
                })
                .Where(x =>
                    x.DistanceAlong >= 0 &&
                    x.DistanceAlong <= length &&
                    x.DistanceAcross <= width / 2.0)
                .OrderByDescending(x => x.DistanceAlong)
                .Select(x => x.Unit)
                .ToList();

            Fix6D(corridorUnits, unitA, unitB);

            if (corridorUnits.Count == 0)
                throw new Exception("USSoup, Corridor");

            return corridorUnits;
        }

        public void Fix2D(List<UNIT> corridorUnits, UNIT unitA, UNIT unitB)
        {
            corridorUnits.Remove(unitA);
            corridorUnits.Remove(unitB);

            corridorUnits.Insert(0, unitB);
            corridorUnits.Add(unitA);
        }

        public void Fix6D(List<UNIT> corridorUnits, UNIT unitA, UNIT unitB)
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

    public class Select
    {
        private TheMind mind { get; set; }
        private USSoup soup { get; set; }
        public static Select Create(TheMind mind, USSoup soup)
        {
            Select select = new Select();
            select.soup = soup;
            select.mind = mind;
            return select;
        }

        public UNIT ByPyth2D(List<UNIT> units, GPTVector2D v_near)
        {
            double min_distance = 10E20d;
            UNIT res = null;

            foreach (UNIT unit in units)
            {
                if (unit == mind.unit_current)
                    continue;

                GPTVector2D nearest = soup.Near2(unit);
                
                double distance = mind.calc.Pyth2D(v_near.xx, nearest.xx, v_near.yy, nearest.yy);

                if (distance < min_distance)
                {
                    min_distance = distance;
                    res = unit;
                }
            }

            return res;            
        }

        public UNIT ByPyth6D(List<UNIT> units, GPTVector6D v_near)
        {
            double min_distance = 10E20d;
            UNIT res = null;

            foreach (UNIT unit in units)
            {
                if (unit == mind.unit_current)
                    continue;

                GPTVector6D nearest = soup.Near6(unit);

                double distance = mind.calc.Pyth6D(v_near.xx, nearest.xx, v_near.yy, nearest.yy, v_near.zz, nearest.zz, v_near.ww, nearest.ww, v_near.vv, nearest.vv, v_near.uu, nearest.uu);

                if (distance < min_distance)
                {
                    min_distance = distance;
                    res = unit;
                }
            }

            return res;
        }

        public UNIT ByOther(List<UNIT> units, GPTVector2D near)
        {
            throw new NotImplementedException("USSoup, SelectOther");
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

        private bool Quick(bool _pro)
        {
            /*
             * make logic for initiating quick decision here
             * */

            if (!_pro)
                return false;

            if (mind.STATE == STATE.QUICKDECISION)
                return false;

            if (!CONST.SAMPLE50.RandomSample(mind))
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

            bool still_quick = _u.IsQDECISION() && mind.STATE == STATE.JUSTRUNNING;
            bool is_idle = _u.IsIDLE();

            if (still_quick || is_idle)
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

            if (units == null)
                throw new Exception("USSoup, Unit");

            GPTVector2D near2 = Near2(mind.unit_current);
            GPTVector6D near6 = Near6(mind.unit_current);
            List<UNIT> list = new List<UNIT>() { mind.unit_current };
            UNIT res = null;

            if (CONST.select_curr == SELECTCURRENT.PYTH2)
            {
                res = Select.Create(mind, this).ByPyth2D(units, near2);

                list = GPT.Create().Corridor2D(units, mind.unit_current, res);

                list[0].Update2D(near2);
            }

            if (CONST.select_curr == SELECTCURRENT.PYTH6)
            {
                res = Select.Create(mind, this).ByPyth6D(units, near6);

                list = GPT.Create().Corridor6D(units, mind.unit_current, res);

                list[0].Update6D(near6);
            }

            if (CONST.select_curr == SELECTCURRENT.OTHER)
                res = Select.Create(mind, this).ByOther(units, near2);

            if (res == null)
                return (new List<UNIT>() { UNIT.CreateIdle(mind) }).ToArray();
            
            return list.ToArray();
        }

        public GPTVector2D Near2(UNIT unit)
        {
            GPTVector2D func = new GPTVector2D();
            GPTVector2D near = new GPTVector2D();
            GPTVector2D vec = unit.ToVector2D();
            GPTVector2D vec_u = vec.Unit();
            int _out = (int)((Down)mind.down).Output(vec);
            GPTVector2D dir_u = _out < 0 ? vec_u.Unit().ReverseUnit() : vec_u.Unit();

            bool same = (int)func.ToDegrees(vec_u) == (int)func.ToDegrees(dir_u);

            if (same)
                near = vec;

            if (!same)
                near = vec.Reverse();

            return near;
        }

        public GPTVector6D Near6(UNIT unit)
        {
            GPTVector2D func = new GPTVector2D();
            GPTVector6D near = new GPTVector6D();
            GPTVector2D vec2 = unit.ToVector2D();
            GPTVector6D vec6 = unit.ToVector6D();
            GPTVector2D vec_u = vec2.Unit();
            int _out = (int)((Down)mind.down).Output(vec2);
            GPTVector2D dir_u = _out < 0 ? vec_u.Unit().ReverseUnit() : vec_u.Unit();

            bool same = (int)func.ToDegrees(vec_u) == (int)func.ToDegrees(dir_u);

            if (same)
                near = vec6;

            if (!same)
                near = vec6.Reverse();

            return near;
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
