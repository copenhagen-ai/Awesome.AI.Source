using Awesome.AI.Core;
using Awesome.AI.CoreInternals;
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

        public static bool IsYes(this HARDDOWN _q)
        {
            return _q == HARDDOWN.YES;
        }

        public static bool IsNo(this HARDDOWN _q)
        {
            return _q == HARDDOWN.NO;
        }

        public static double IsYesDouble(this HARDDOWN _q)
        {
            return _q.IsYes() ? -1.0 : 1.0d;
        }

        public static double IsNoDouble(this HARDDOWN _q)
        {
            return _q.IsNo() ? 1.0d : -1.0d;
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


        public static FUZZYDOWN ToFuzzy(this double deltaMom, TheMind mind)
        {
            double norm = mind.mech_current.p_100;

            //if (mind.parms.hack == HACKMODES.HACK)
            //    norm = 100.0d - norm;

            switch (norm)
            {
                case < 20.0d: return FUZZYDOWN.VERYNO;
                case < 40.0d: return FUZZYDOWN.NO;
                case < 60.0d: return FUZZYDOWN.MAYBE;
                case < 80.0d: return FUZZYDOWN.YES;
                case < 100.0d: return FUZZYDOWN.VERYYES;
                default: throw new NotSupportedException("ToFuzzy");
            }
        }

        public static PERIODDOWN ToPeriod(this List<HARDDOWN> Ratio, TheMind mind)
        {
            /*
             * indifferent of the direction
             * */

            int count_no = Ratio.Count(x=>x == HARDDOWN.NO);
            int count_yes = Ratio.Count(x=>x == HARDDOWN.YES);

            PERIODDOWN res = count_no >= count_yes ? 
                PERIODDOWN.NO : 
                PERIODDOWN.YES;

            //if (mind.parms.hack == HACKMODES.HACK)
            //    res = !res;

            return res;
        }

        public static HARDDOWN GoDownZero(this double deltaMom, TheMind mind)
        {
            /*
             * NO is to say no to going downwards
             * */

            if (mind.z_current != "z_noise")
                throw new NotImplementedException("Extensions, ToDownZero");

            SimpleAgent agent = new SimpleAgent(mind);
            //double time = mind.cycles * 0.1d;

            bool down1 = deltaMom <= 0.0d;
            bool down2 = agent.SimulateDown();
            //bool dontflip1 = CONST.SAMPLE100.RandomSample(mind);
            //bool dontflip2 = Math.Sin(time) > 0.4d;

            //if (CONST.Logic == LOGICTYPE.QUIRK)
            //    down1 = dontflip1 ? down1 : !down1;

            //if (CONST.Logic == LOGICTYPE.SINE)
            //    down1 = dontflip2 ? down1 : !down1;

            if (CONST.Logic == LOGICTYPE.CLASSICAL) //is this a logic error?
                down1 = !down1;//we flip direction

            if (CONST.Logic == LOGICTYPE.PROBABILITY)
                down1 = mind.prob.Use(down1, mind);

            if (CONST.Logic == LOGICTYPE.QUBIT)
                down1 = mind.quantum.usage.DoQuantumXOR(down1, down2);

            return down1 ? HARDDOWN.YES : HARDDOWN.NO;
        }

        public static HARDDOWN GoDownPrev(this double deltaMom, double prev, TheMind mind)
        {
            /*
             * NO is to say no to going downwards
             * */

            if (mind.z_current != "z_noise")
                throw new NotImplementedException("Extensions, ToDownPrev");

            SimpleAgent agent = new SimpleAgent(mind);
            //double time = mind.cycles * 0.1d;

            bool down1 = deltaMom <= prev;
            bool down2 = agent.SimulateDown();
            //bool dontflip1 = CONST.SAMPLE100.RandomSample(mind);
            //bool dontflip2 = Math.Sin(time) > 0.4d;

            //if (CONST.Logic == LOGICTYPE.QUIRK)
            //    down1 = dontflip1 ? down1 : !down1;

            //if (CONST.Logic == LOGICTYPE.SINE)
            //    down1 = dontflip2 ? down1 : !down1
            //    ;

            if (CONST.Logic == LOGICTYPE.CLASSICAL) //is this a logic error?
                down1 = !down1;//we flip direction

            if (CONST.Logic == LOGICTYPE.PROBABILITY)
                down1 = mind.prob.Use(down1, mind);

            if (CONST.Logic == LOGICTYPE.QUBIT)
                down1 = mind.quantum.usage.DoQuantumXOR(down1, down2);

            return down1 ? HARDDOWN.YES : HARDDOWN.NO;
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

        //public static HARDDOWN ToDirection(this bool _q)
        //{
        //    return _q ? HARDDOWN.YES : HARDDOWN.NO;
        //}        
    }
}
