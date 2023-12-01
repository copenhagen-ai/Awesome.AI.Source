using Awesome.AI.Core;
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

        public static string Data(this string value)
        {
            return value.Split(':')[0];
        }

        public static string Class(this string value)
        {
            if (!value.Contains(":"))
                return "null";
            return value.Split(':')[1];
        }

        public static bool TheHack1(this bool _b, TheMind mind)
        {
            /*
             * >> this is the hack/cheat <<
             * */

            bool do_hack = mind.parms.hmode1 == HACKMODES.HACK;
            if (do_hack)
                return !_b;
            return _b;
        }

        public static bool TheHack2(this bool _b, TheMind mind)
        {
            /*
             * >> this is the hack/cheat <<
             * */

            bool do_hack = mind.parms.hmode2 == HACKMODES.HACK;
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
    }
}
