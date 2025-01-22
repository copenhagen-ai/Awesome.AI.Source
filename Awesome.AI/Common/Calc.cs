using Awesome.AI.Core;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Common
{
    public class Calc
    {
        TheMind mind;
        private Calc() { }
        public Calc(TheMind mind) 
        { 
            this.mind = mind;
        }

        public double Normalize(double value, double valmin, double valmax)
        {
            double normalized = (value - valmin) / (valmax - valmin);
            return normalized;
        }

        public double NormalizeRange(double val, double valmin, double valmax, double ranmin, double ranmax)
        {
            return (((val - valmin) / (valmax - valmin)) * (ranmax - ranmin)) + ranmin;
        }

        public double Denormalize(double normalized, double min, double max)
        {
            double denormalized = (normalized * (max - min) + min);
            return denormalized;
        }

        public bool MyChance0to100(double val, bool gt)
        {
            int[] num = mind.rand.MyRandomInt(1, 100);
            return gt ? num[0] > val : num[0] < val;
        }

        public bool Chance0to100(double val, bool gt)
        {
            int num = mind.rand.RandomInt(100);
            return gt ? num > val : num < val;
        }

        public bool Chance0to1(double val, bool gt)
        {
            double num = mind.rand.RandomDouble(0.0d, 1.0d);
            return gt ? num > val : num < val;
        }

        public string FormatDouble(double d, int decimals)
        {
            string s = "" + d;
            string res = "";
            if (s.Contains(','))
            {
                res = s.Split(',')[0];
                char[] arr = s.Split(',')[1].ToCharArray();
                if (decimals > 0)
                    res += ",";
                for (int i = 0; i < decimals; i++)
                    res += i < arr.Length ? "" + arr[i] : "0";
            }
            else
            {
                if (decimals > 0)
                    s += ",";
                for (int i = 0; i < decimals; i++)
                    s += "0";
                res = s;
            }

            return res;
        }

        public double ToPercent(double num, double high)
        {
            double res = 0.0d;
            res = mind.calc.Normalize(num, 0.0f, high) * 100.0d;

            return res;
        }

        public double GetPercentage(double num, double high)
        {
            /*
             * How many percent is 50 from 500? 	50 / 500 * 100% = 10%
             * */

            double res = num / high * 100.0d;

            return res;
        }

        public int RoundOff(int i)
        {
            return ((int)Math.Round(i / 10.0)) * 10;
        }

        public double Round(double num, int dec, bool up)
        {
            int pow = (int)Math.Pow((double)10, (double)dec);
            double res;
            res = up ? Math.Ceiling(num * pow) / (double)pow : Math.Floor(num * pow) / (double)pow;

            return res;
        }

        public double RoundDouble(double num, int dec)
        {
            double res;
            res = Math.Round(num, dec);

            return res;
        }

        public int RoundInt(double num)
        {
            //round to nearest, up or down
            int res;
            res = System.Convert.ToInt32(num);

            return res;
        }

        public int RoundUp(double num)
        {
            int res = (int)Math.Ceiling((double)num);

            return res;
        }

        public int RoundDown(double num)
        {
            int res = (int)Math.Floor((double)num);

            return res;
        }        

        public int HighestIndex(double[] arr)
        {
            double max = arr.Max();
            int index = arr.ToList().IndexOf(max);
            return index;
        }

        public double FrictionCoefficient(bool is_static, double credits)
        {
            //should friction be calculated from position???

            if (is_static)
                return mind.parms.base_friction;

            Calc calc = mind.calc;
            double x = credits;
            double friction = 0.0d;

            switch (mind.mech)
            {
                //this could be better, use sigmoid/logistic
                case MECHANICS.HILL: friction = calc.Linear(x, -0.5d, 7d) / 10; break;
                case MECHANICS.CONTEST: friction = calc.Linear(x, -0.25d, 2.5d) / 10; break;
                default: throw new Exception();
            }

            if (friction < 0.0d)
                friction = 0.0d;

            if (friction > 10.0d)
                friction = 10.0d;

            return friction;
        }

        public double Reciprocal(double x)
        {
            if (x < 0.0d)
                throw new Exception();
            
            if (x == 0.0d)
                throw new Exception();

            return 1 / x;
        }

        public double Linear(double x, double a, double b)
        {
            if (x < 0.0d)
                throw new Exception();

            double y = a * x + b;                       //y er mass

            //if (y > 2.0)                                //er egentlig ikke nødvendig
            //    throw new Exception();
            //if (y < 0.0)
            //    throw new Exception();

            return y;
        }

        public double Quadratic(double x, double a, double b, double c)
        {
            //if (x < 0.0d)
            //    throw new Exception();

            double y = a * (x * x) + (b * x) + c;         //y er mass

            //if (y >= 10.0)                              //10 er top grænsen, 10 * 10
            //    y = 10.0;
            //if (y < 0.1)                                //er egentlig ikke nødvendig
            //    throw new Exception();

            return y;
        }

        public double Logistic(double x)
        {
            /*
             * logistic function
             * f(x) = L / (1 + e^-k(x - x0))
             * 
             * standard logistic function
             * f(x) = 1/(1 + e^-x)
             * */
            double y = 1d / (1d + Math.Exp(-x));

            return y;
        }        

        public double Pyth(double _a, double _b)
        {
            if (_a <= 0.0d)
                throw new Exception();
            if (_b <= 0.0d)
                throw new Exception();

            double _dist = Math.Sqrt(_a * _a + _b * _b);

            return _dist;
        }

        public double PythNear(double theta, double hypotenuse)//hosliggende
        {
            /*
             * cos(v) = hosliggende katete / hypotenusen
             * sin(v) = modstående katete / hypotenusen
             * tan(v) = modstående katete / hosliggende katete
             * */
            if (theta > 180.0d)
                throw new Exception();
            if (theta < -180.0d)
                throw new Exception();
            if (hypotenuse <= 0.0d)
                throw new Exception();

            double _rad = ToRadiansFromDegrees(theta);
            double _cos = Math.Cos(_rad);
            double res = _cos * hypotenuse;

            return res;
        }

        public double PythFar(double theta, double hypotenuse)//modstående
        {
            /*
             * cos(v) = hosliggende katete / hypotenusen
             * sin(v) = modstående katete / hypotenusen
             * tan(v) = modstående katete / hosliggende katete
             * */
            if (theta >= 90.0d)
                throw new Exception();
            if (theta <= 0.0d)
                throw new Exception();
            if (hypotenuse <= 0.0d)
                throw new Exception();

            double _rad = ToRadiansFromDegrees(theta);
            double _sin = Math.Sin(_rad);
            double res = _sin * hypotenuse;

            return res;
        }

        public double ToRadiansFromDegrees(double angle)
        {
            double res = angle * (Math.PI / 180.0d);

            return res;
        }

        public double ToDegreesFromRadians(double radians)
        {
            double res = radians * (180.0d / Math.PI);

            return res;
        }

        public double ToDegreesFromSlope(double slope)
        {
            double res = Math.Atan(slope) * (180.0d / Math.PI);

            return res;
        }

        public double SlopeCoefficient(double x, double a, double b)
        {
            /*
             * derivative of:    f(x) = ax^2 + bx + c
             *                   f'(x)= 2ax + b
             * */

            double res = 2 * a * x + b;

            return res;
        }

        public double Integral(double x, double a, double b, double c, double k)
        {
            /*
             * derivative of:    f(x) = ax^2 + bx + c
             *                   F(x)= (1/3)ax^3 + (1/2)bx^2 + cx + k
             * */

            double res = (1/3) * a * (x*x*x) + (1/2) * b * (x*x) + c * x + k;

            return res;
        }

        public double Roots(double? _x, double _a, double _b, double _c, out double _x1, out double _x2)
        {
            _x1 = (-_b + Math.Sqrt(_b * _b - 4 * _a * _c)) / (2 * _a);
            _x2 = (-_b - Math.Sqrt(_b * _b - 4 * _a * _c)) / (2 * _a);

            if (_x.IsNull())
                return -1d;

            double res = _x >= 0.0d ? _x2 : _x1;

            return res;
        }
    }
}
