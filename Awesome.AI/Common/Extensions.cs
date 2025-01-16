using Awesome.AI.Core;
using Awesome.AI.Helpers;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Common
{
    public static class Extensions
    {
        public static bool IsNull<T>(this T source)
        {
            return source == null;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count() == 0;
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return source == null || source.Count() == 0;
        }

        public static bool HasValue(this string source)
        {
            return source != null && source != "";
        }

        public static bool HasValue(this double value)
        {
            return !Double.IsNaN(value) && !Double.IsInfinity(value);
        }


        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static double Convert(this double _x, TheMind mind)
        {
            double _l = 0.0d;
            double _h = 100.0d;
            
            double res = mind.calc.NormalizeRange(_x, _l, _h, Constants.MIN, Constants.MAX);

            return res;
        }

        public static bool TheHack1(this bool _b, TheMind mind)
        {
            /*
             * >> this is the hack/cheat <<
             * */

            bool do_hack = mind.parms.hack1 == HACKMODES.HACK;
            if (do_hack)
                return !_b;
            return _b;
        }

        public static bool TheHack2(this bool _b, TheMind mind)
        {
            /*
             * >> this is the hack/cheat <<
             * */

            bool do_hack = mind.parms.hack2 == HACKMODES.HACK;
            if (do_hack)
                return !_b;
            return _b;
        }

        public static bool IsYes(this THECHOISE q)
        {
            return q == THECHOISE.YES;
        }

        public static bool IsNo(this THECHOISE q)
        {
            return q == THECHOISE.NO;
        }

        public static bool IsNOT(this OPINION q)
        {
            return q == OPINION.NOT;
        }

        public static bool IsOK(this OPINION q)
        {
            return q == OPINION.OK;
        }

        public static bool IsNONE(this OPINION q)
        {
            return q == OPINION.NONE;
        }

        public static bool IsEITHER(this OPINION q)
        {
            return q == OPINION.NOT || q == OPINION.OK;
        }

        //public static string Data(this string value)
        //{
        //    return value.Split(':')[0];
        //}

        //public static string Class(this string value)
        //{
        //    if (!value.Contains(":"))
        //        return "null";
        //    return value.Split(':')[1];
        //}
    }
}
