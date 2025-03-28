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
        }

        private List<double> saves { get; set; }

        public void SaveMomentum(double momentum)
        {
            if (double.IsNaN(momentum))
                throw new Exception("SaveMomentum");

            if (double.IsInfinity(momentum))
                throw new Exception("SaveMomentum");

            if (mind.cycles_all < Constants.FIRST_RUN)
                momentum = RandomDouble(0.0d, 1.0d);

            if (momentum == 0.0d)
                return;

            if (saves == null)
            {
                saves = new List<double>();
                for (int i = 0; i < 495; i++)
                    saves.Add(RandomDouble(0.0d, 1.0d));
            }

            saves.Add(momentum);
            if (saves.Count > 500)
                saves.RemoveAt(0);
        }

        public double[] MyRandomDouble(int count)
        {
            try
            {
                double[] res = new double[count];
                for (int i = 0; i < count; i++)
                {
                    string rand = Rand(i);

                    int index = rand.Length < 10 ? rand.Length : 10;

                    res[i] = double.Parse($"0.{rand[..index]}", CultureInfo.InvariantCulture);
                }

                return res;
            }
            catch (Exception _e)
            {
                throw new Exception("MyRandomDouble");
            }
        }

        public int[] MyRandomInt(int count, int i_max)
        {
            try
            {
                /*
                 * max 999
                 * 0 <= res < i_max
                 */

                if (i_max > 999)
                    throw new Exception("MyRandomInt");

                int[] res = new int[count];

                for (int i = 0; i < count; i++)
                {
                    string rand = Rand(i);

                    double dec = double.Parse($"{rand[0]}{rand[1]}{rand[2]}") / 1000;
                    res[i] = mind.calc.RoundInt((double)i_max * dec);
                }

                return res;
            }
            catch
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
                string rand = "" + saves[index];

                //remove exponent
                int index_e = rand.ToUpper().IndexOf('E');
                if (rand.ToUpper().Contains("E"))
                    rand = rand[..index_e];

                //reverse, this is the random part
                string res = "";
                for (int i = rand.Length; i > 0; i--)
                    res += char.IsDigit(rand[i - 1]) ? rand[i - 1] : "";

                return res;
            }
            catch
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

        //public int MyRandom(int i_max)
        //{
        //    /*
        //     * max 999
        //     */

        //    if (i_max > 999)
        //        throw new Exception();

        //    double promil = GetPromil();
        //    int res = mind.calc.RoundInt((double)i_max * promil);

        //    return res;
        //}

        //private double GetPromil()
        //{
        //    double _out = -1.0d;
        //    string rand = "";
        //    try
        //    {
        //        IMechanics mech = mind.parms._mech;

        //        if (double.IsNaN(mech.dir.d_momentum))
        //            throw new Exception();

        //        if (double.IsInfinity(mech.dir.d_momentum))
        //            throw new Exception();

        //        if (mind.cycles_all < mind.parms.first_run)//make shure the system is running before proceeding
        //            return 0.5d;

        //        _out = mech.dir.d_momentum;
        //        int i_ct = 15;
        //        string s_promil = "error";
        //        double d_promil;

        //        rand = ("" + _out).ToUpper().Replace("E", "").Replace("-", "").Replace(",", "").Replace(".", "");//this is the random part
        //        rand = rand.Substring(0, rand.Length - 2);//remove exponent

        //        do
        //        {
        //            if (rand.Count() < i_ct + 3)
        //            { i_ct--; continue; }

        //            if (i_ct < 0)
        //                return 0.5d;

        //            string _a = "" + rand.ElementAt(i_ct);
        //            string _b = "" + rand.ElementAt(i_ct + 1);//+ or -
        //            string _c = "" + rand.ElementAt(i_ct + 2);//+ or -

        //            bool ok = "0123456789".Contains(_a);
        //            ok = ok && "0123456789".Contains(_b);
        //            ok = ok && "0123456789".Contains(_c);

        //            s_promil = ok ? $"{_a}{_b}{_c}" : "error";
        //            i_ct--;
        //        } while (s_promil == "error");

        //        d_promil = double.Parse(s_promil) / 1000;

        //        return d_promil;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("rand: " + _out);
        //        Console.WriteLine("rand: " + rand);
        //        Console.WriteLine(e.Message);

        //        return -9999.9999d;
        //    }
        //}
    }
}
