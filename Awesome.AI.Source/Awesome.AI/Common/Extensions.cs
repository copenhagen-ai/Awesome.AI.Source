using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Common
{
    public static class Extensions
    {
        public static void BusyWait(this string txt, int count)
        {
            //because i dont want to implement async
            for (int i = 0; i < count; i++) 
                Console.WriteLine(txt);
        }

        public static bool RandomSample(this int count, TheMind mind)
        {
            //this is a replacement, for just performing task when (mind)do_process
            return mind.calc.Chance(count, 10);
        }

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
            
            double res = mind.calc.Normalize(_x, _l, _h, CONST.MIN, CONST.MAX);

            return res;
        }

        public static bool Below(this double val)
        {
            return val < 0.0d;
        }

        public static bool Probability(this bool _b, TheMind mind)
        {
            return mind.prob.Use(_b, mind);
        }

        public static bool Qubit(this bool _b1, bool _b2, TheMind mind)
        {
            return mind.quantum.usage.DoQuantumXOR(_b1, _b2);
        }

        [Obsolete("Legazy Method", false)]
        public static bool TheHack(this bool _b, TheMind mind)
        {
            /*
             * >> this is the hack/cheat <<
             * */
            bool do_hack = CONST.hack == HACKMODE.HACK;
            if (do_hack)
                return !_b;
            return _b;
        }
    }
}
