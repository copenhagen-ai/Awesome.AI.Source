namespace Awesome.AI.Common
{
    using Awesome.AI.Core;
    using System;

    public class MyProbabilityHelper
    {
        // Constants
        private const double SQRT2PI = 2.5066282746310002;

        public bool Use(bool val, TheMind mind)
        {
            double norm = mind.mech["z_noise"].p_100;
            norm = mind.calc.Normalize(norm, 0.0d, 100.0d, -10.0d, 10.0d);
            double per = mind.prob.NormalCDF(norm, 0.0d, 2.0d) * 100.0d;

            bool flip = mind.calc.Chance(100, 100 - (int)per);

            return flip ? !val : val;
        }

        /// <summary>
        /// Calculates the standard normal PDF (bell curve).
        /// </summary>
        public double NormalPDF(double x, double mean, double stdDev)
        {
            double z = (x - mean) / stdDev;
            return Math.Exp(-0.5 * z * z) / (stdDev * SQRT2PI);
        }

        /// <summary>
        /// Calculates the standard normal CDF using an approximation (error function).
        /// </summary>
        public double NormalCDF(double x, double mean, double stdDev)
        {
            double z = (x - mean) / (stdDev * Math.Sqrt(2));
            return 0.5 * (1 + Erf(z));
        }

        /// <summary>
        /// Probability that a value lies between a and b.
        /// </summary>
        public double ProbabilityBetween(double a, double b, double mean, double stdDev)
        {
            return NormalCDF(b, mean, stdDev) - NormalCDF(a, mean, stdDev);
        }

        /// <summary>
        /// Approximation of the error function (erf).
        /// </summary>
        private double Erf(double x)
        {
            // Abramowitz and Stegun formula 7.1.26 approximation
            double t = 1.0 / (1.0 + 0.3275911 * Math.Abs(x));
            double[] a = { 0.254829592, -0.284496736, 1.421413741, -1.453152027, 1.061405429 };
            double sum = t * (a[0] + t * (a[1] + t * (a[2] + t * (a[3] + t * a[4]))));
            double result = 1.0 - sum * Math.Exp(-x * x);
            return x >= 0 ? result : -result;
        }
    }
}
