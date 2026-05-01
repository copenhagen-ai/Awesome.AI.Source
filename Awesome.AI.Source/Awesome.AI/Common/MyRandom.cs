using Awesome.AI.Core;
using Awesome.AI.Variables;
using System.Globalization;

namespace Awesome.AI.Common
{
    public class MyRandom
    {
        private TheMind mind;
        private MyRandom() { }
        public MyRandom(TheMind mind)
        {
            this.mind = mind;
            
            saves ??= new List<double>();

            for (int i = 0; i < 450; i++)
                saves.Add(RandomDouble(0.0d, 1.0d));
        }

        private List<double> saves { get; set; }
        public void SaveDeltaVel(double dv)
        {
            if (double.IsNaN(dv))
                throw new Exception("SaveMomentum");

            if (double.IsInfinity(dv))
                throw new Exception("SaveMomentum");

            if (mind.cycles_all < CONST.FIRST_RUN)
                dv = saves[mind.cycles_all];

            if (dv == 0.0d)
                return;

            if (saves.Contains(dv))
                return;
            
            saves.Add(dv);
            if (saves.Count > 500)
                saves.RemoveAt(0);
        }

        private int shift_b {  get; set; }
        public double[] MyRandomDouble(int count)
        {
            try
            {
                double[] res = new double[count];
                for (int i = 0; i < count; i++)
                {
                    string rand = Rand(i + shift_b);

                    int index = rand.Length < 10 ? rand.Length : 10;

                    res[i] = double.Parse($"0.{rand[..index]}", CultureInfo.InvariantCulture);
                }

                shift_b++;
                if (shift_b >= 100)
                    shift_b = 0;

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("MyRandomDouble");
            }
        }

        private int shift_a {  get; set; }
        public int[] MyRandomInt(int count, int i_max)
        {
            try
            {
                /*
                 * max 999
                 * 0 <= res < i_max
                 */

                if (i_max > 99999)
                    throw new Exception("MyRandomInt");

                int[] res = new int[count];

                for (int i = 0; i < count; i++)
                {
                    string rand = Rand(i + shift_a);

                    double dec = double.Parse($"{rand[0]}{rand[1]}{rand[2]}{rand[3]}{rand[4]}") / 100000;
                    res[i] = (int)((double)(i_max + 1) * dec);
                }

                shift_a++;
                if (shift_a >= 100)
                    shift_a = 0;

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("MyRandomInt");
            }
        }

        private string Rand(int index)
        {
            try
            {
                if (index + 1 > saves.Count)
                    throw new Exception("Rand");

                //get momentum
                string rand = "" + (saves[index]);

                //remove exponent
                int index_e = rand.ToUpper().IndexOf('E');
                if (rand.ToUpper().Contains("E"))
                    rand = rand[..index_e];

                //reverse, this is the random part
                string res = "";
                for (int i = rand.Length; i > 0; i--)
                    res += char.IsDigit(rand[i - 1]) ? rand[i - 1] : "";

                //string res = "";
                //for (int i = 3; i < rand.Length; i++)
                //    res += char.IsDigit(rand[i]) ? rand[i] : "";

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("Rand");
            }
        }

        private Random r1 = new Random();
        public int RandomInt(int max)
        {
            int rand = r1.Next(0, max);
            return rand;
        }

        public int RandomInt(int low, int max)
        {
            int rand = r1.Next(low, max);
            return rand;
        }

        public double RandomDouble(double min, double max)
        {
            double rand = r1.NextDouble() * (max - min) + min;
            return rand;
        }
    }
}
