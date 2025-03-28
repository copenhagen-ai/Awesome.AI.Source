using Awesome.AI.Common;
using Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreInternals
{
    public class Filters
    {
        private TheMind mind;
        private Filters() { }
        public Filters(TheMind mind)
        {
            this.mind = mind;
        }

        public bool Direction(UNIT _x)
        {
            /*
             * this filter can be on or off, just have to tweek HardMom and ToDownZero/ToDownPrev
             * */

            if (_x == null)
                throw new ArgumentNullException();

            if (mind.parms.state == STATE.QUICKDECISION)
                return true;

            double f_a = _x.Variable;
            double f_b = mind.curr_unit.Variable;

            bool go_up = mind.Direction();

            //remember static: high, dynamic: low.. at zero
            return go_up ? /*up*/f_a < f_b : /*down*/f_a >= f_b;
        }

        public bool UnitIsValid(UNIT _u)
        {
            if (_u == null)
                throw new ArgumentNullException();

            bool ok = _u.IsValid;

            return ok;
        }

        public bool LowCut(UNIT _u)//aka SayNo
        {
            /*
             * Lowcut filter?
             * */

            if (_u == null)
                throw new ArgumentNullException();

            if (mind.parms.state == STATE.QUICKDECISION)
                return true;

            double lower_border = mind.parms.low_cut;
            double force = _u.Variable;

            if (force < lower_border)
                return true;
            return false;
        }

        public bool Credits(UNIT unit)
        {
            if (unit == null)
                throw new ArgumentNullException();

            if (mind.parms.state == STATE.QUICKDECISION)
                return true;

            return unit.credits > 1.0d;
        }

        /*public static bool HighPass(UNIT _u)//aka SayNo
        {
            /*
             * Lowcut filter?
             * /
            List<UNIT> units = Memory.UNITS();
            int count = units.Count();
            int div = (int)(count * Params.high_pass);
            units = units.OrderBy(x => x.force).Take(div).ToList();

            bool res = units.Contains(_u);
            return res;
        }/**/

        //public bool Neighbor(UNIT _u, List<UNIT> list)
        //{
        //    if (_u.root.StartsWith("lost "))
        //        ;

        //    bool res;
        //    double slope = mind.parms.slope;//0.666
        //    int n_visit = _u.Next.IsNEXTPREV() ? _u.Next.visited : -1;
        //    int p_visit = _u.Prev.IsNEXTPREV() ? _u.Prev.visited : -1;
        //    int visited = _u.visited;

        //    if (n_visit >= 0 && p_visit >= 0)
        //        res = n_visit >= visited * slope && p_visit >= visited * slope;
        //    else if (n_visit >= 0)
        //        res = n_visit >= visited * slope;
        //    else if (p_visit >= 0)
        //        res = p_visit >= visited * slope;
        //    else
        //        throw new Exception();

        //    return res;
        //}

        //public static bool Neighbor(UNIT _u, List<UNIT> list)
        //{
        //    if (_u.root.StartsWith("lost "))
        //        ;

        //    list = list.Where(x=>!x.IsSPECIAL()).ToList();

        //    bool res;
        //    double slope = Params.slope;//0.666
        //    double n_visit = _u.Next(list).IsNEXTPREV() ? _u.Next(list).visited : -1;
        //    double p_visit = _u.Prev(list).IsNEXTPREV() ? _u.Prev(list).visited : -1;
        //    double visited = _u.visited;

        //    double[] arr = new double[] { visited, n_visit, p_visit };

        //    double lowest = 1000;
        //    foreach (double d in arr)
        //    {
        //        if (d == -1)
        //            continue;

        //        if (d < lowest)
        //            lowest = d;
        //    }

        //    n_visit = n_visit == -1 ? -1 : n_visit - lowest;
        //    p_visit = p_visit == -1 ? -1 : p_visit - lowest;
        //    visited = visited == -1 ? -1 : visited - lowest;

        //    if (n_visit >= 0 && p_visit >= 0)
        //        res = n_visit >= visited * slope && p_visit >= visited * slope;
        //    else if (n_visit >= 0)
        //        res = n_visit >= visited * slope;
        //    else if (p_visit >= 0)
        //        res = p_visit >= visited * slope;
        //    else
        //        res = true;// throw new Exception();

        //    return res;
        //}


        //public bool Theme(UNIT unit)
        //{
        //    if (unit == null)
        //        throw new ArgumentNullException();

        //    if (mind.theme == "none")
        //        return true;

        //    if (!mind.theme_on)
        //        return true;

        //    HUB top = mind.mem.HUBS_SUB(mind.theme);
        //    HUB curr = unit.HUB;
        //    if (top.GetSubject() == curr.GetSubject())
        //        return true;
        //    //foreach (HUB h in top.hubs)
        //    //{
        //    //    if (h.GetSubject() != "LEARNING" && h.GetSubject() == curr.GetSubject())
        //    //        return true;
        //    //}
        //    return false;
        //}

        //public static bool Elastic1(Direction dir)
        //{
        //    //return true;

        //    IMechanics mech = Params.GetMechanics();

        //    bool _b = dir.IsUP() ? false : true;

        //    double _out = Calc.NormalizeRange(dir.d_output, mech.low_out, mech.high_out, -4.0d, 4.0d);
        //    double logi = Calc.Logistic(_out);
        //    double percentage = Calc.NormalizeRange(logi, 0.0d, 1.0d, 50.0d, 95.0d);
        //    bool flip = Calc.MyChance0to100(percentage, _b);

        //    return flip;
        //}

        //public static bool Elastic2(Direction dir)
        //{
        //    //return true;

        //    IMechanics mech = Params.GetMechanics();

        //    //bool _b = dir.IsUP() ? false : true;

        //    double _out = Calc.NormalizeRange(dir.d_output, mech.low_out, mech.high_out, -4.0d, 4.0d);
        //    double logi = Calc.Quadratic(_out, 0.06d, 0.0d, 0.05d);
        //    double percentage = Calc.NormalizeRange(logi, 0.0d, 1.0d, 30.0d, 100.0d);
        //    bool flip = Calc.MyChance0to100(percentage, false);

        //    return flip;
        //}

        //private static int[] rows = null;
        //private static int[] rows_act = null;
        //private static int area_under_curve = 0;
        //private static int[] Setup(double _a, double _b, double _c, double _k, out int area)
        //{
        //    area = area_under_curve;
        //    if (rows != null)
        //        return rows;
        //    List<UNIT> units = Memory.UNITS_VAL();
        //    int count = units.Count;
        //    rows = new int[count];
        //    rows_act = new int[count];
        //    for (int i = 0; i < count; i++)
        //    {
        //        double _x = Calc.NormalizeRange(units[i].Index, 0.0d, 100.0d, -10.0d, 10.0d);
        //        rows[i] = (int)Calc.Quadratic(_x, _a, _b, _c);
        //    }

        //    //d_area_under_curve = Calc.Integral(10.0, _a, _b, _c, _k) - Calc.Integral(-10.0d, _a, _b, _c, _k);
        //    area_under_curve = (int)rows.Sum();
        //    area = area_under_curve;

        //    int sum = rows.Sum();
        //    if (sum > area)
        //        throw new Exception();

        //    return rows;
        //}

        //public static bool Ideal(UNIT unit)
        //{
        //    if (unit == null)
        //        throw new ArgumentNullException();
        //    if (unit.IsSPECIAL())
        //        return true;

        //    double _a = 0.05d;
        //    double _b = 0.0d;
        //    double _c = 1.0d;
        //    double _k = 0.0d;

        //    List<UNIT> units = Memory.UNITS_VAL().Where(x=>!x.IsSPECIAL()).ToList();
        //    int area;
        //    int index = units.IndexOf(unit);

        //    int[] rows = Setup(_a, _b, _c, _k, out area);

        //    int sum = rows_act.Sum();
        //    if (sum >= area)
        //        rows_act = new int[units.Count];

        //    if (units.Count != rows_act.Length)
        //        ;
        //    if (index >= rows_act.Length)
        //        ;
        //    if (index >= rows.Length)
        //        ;

        //    int _ra = rows_act[index];
        //    int _rb = rows[index];
        //    if (_ra < _rb)
        //    {
        //        rows_act[index]++;
        //        return true;
        //    }
        //    return false;
        //}
    }
}
