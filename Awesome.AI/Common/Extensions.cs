using Awesome.AI.Core;
using Awesome.AI.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Awesome.AI.Helpers.Enums;
using static Google.Protobuf.WellKnownTypes.Field.Types;

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

        //public static bool TheHack(this bool _b, TheMind mind)
        //{
        //    /*
        //     * >> this is the hack/cheat <<
        //     * */
        //    bool do_hack = mind.parms.hack == HACKMODES.HACK;
        //    if (do_hack)
        //        return !_b;
        //    return _b;
        //}

        public static SOFTCHOICE ToFuzzy(this double deltaMom, TheMind mind)
        {
            double high = mind.mech.d_out_high;
            double low = mind.mech.d_out_low;

            double norm = mind.calc.NormalizeRange(deltaMom, low, high, 0.0d, 100.0d);

            if (mind.parms.hack == HACKMODES.HACK)
                norm = 100.0d - norm;

            switch (norm)
            {
                case < 20.0d: return SOFTCHOICE.VERYNO;
                case < 40.0d: return SOFTCHOICE.NO;
                case < 60.0d: return SOFTCHOICE.DUNNO;
                case < 80.0d: return SOFTCHOICE.YES;
                case < 100.0d: return SOFTCHOICE.VERYYES;
                default: throw new NotSupportedException("ToFuzzy");
            }
        }

        public static bool PeriodNO(this List<HARDCHOICE> Ratio, TheMind mind)
        {
            /*
             * indifferent of the direction
             * */

            int count_no = Ratio.Count(x=>x == HARDCHOICE.NO);
            int count_yes = Ratio.Count(x=>x == HARDCHOICE.YES);

            bool res = count_no >= count_yes;

            if (mind.parms.hack == HACKMODES.HACK)
                res = !res;

            return res;
        }

        public static HARDCHOICE ToChoise(this double deltaMom, TheMind mind)
        {
            /*
             * "NO", is to say no to going downwards
             * */

            bool res = deltaMom <= 0.0d;

            if (mind.parms.hack == HACKMODES.HACK)
                res = !res;

            //is this a hack or ok???
            return res ? HARDCHOICE.NO : HARDCHOICE.YES;
        }

        public static HARDCHOICE ToChoice(this bool _q)
        {
            //is this a hack or ok???
            return _q ? HARDCHOICE.NO : HARDCHOICE.YES;
        }

        public static bool IsNo(this HARDCHOICE _q)
        {
            return _q == HARDCHOICE.NO;
        }
    }
}
