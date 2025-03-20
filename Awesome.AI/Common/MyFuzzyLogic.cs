using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Common
{
    public class MyFuzzyLogic
    {
        /// <summary>
        /// Computes the membership value of a given input within a specific fuzzy set.
        /// </summary>
        /// <param name="set">The fuzzy set (VERYYES, YES, DUNNO, NO, VERYNO).</param>
        /// <param name="x">The input value to evaluate.</param>
        /// <returns>The membership value (between 0 and 1).</returns>
        public static double GetMembershipValue(FUZZYDOWN set, double x)
        {
            return set switch
            {
                FUZZYDOWN.VERYYES => TriangularMembership(x, 8, 10, 12),
                FUZZYDOWN.YES => TriangularMembership(x, 6, 8, 10),
                FUZZYDOWN.MAYBE => TriangularMembership(x, 4, 5, 6),
                FUZZYDOWN.NO => TriangularMembership(x, 2, 3, 4),
                FUZZYDOWN.VERYNO => TriangularMembership(x, 0, 1, 2),
                _ => 0
            };
        }

        /// <summary>
        /// Computes the membership value of a given input using a triangular membership function.
        /// </summary>
        private static double TriangularMembership(double x, double a, double b, double c)
        {
            if (x <= a || x >= c) return 0;
            if (x == b) return 1;

            if (b - a == 0) return (c - x) / (c - b);  // Prevent divide by zero when a == b
            if (c - b == 0) return (x - a) / (b - a);  // Prevent divide by zero when b == c

            if (x > a && x < b) return (x - a) / (b - a);
            return (c - x) / (c - b);
        }

        /// <summary>
        /// Performs fuzzy AND operation (minimum value between two memberships).
        /// </summary>
        public static double And(double muA, double muB)
        {
            return Math.Min(muA, muB);
        }

        /// <summary>
        /// Performs fuzzy OR operation (maximum value between two memberships).
        /// </summary>
        public static double Or(double muA, double muB)
        {
            return Math.Max(muA, muB);
        }

        /// <summary>
        /// Performs fuzzy NOT operation (complement of the membership value).
        /// </summary>
        public static double Not(double muA)
        {
            return 1 - muA;
        }

        /// <summary>
        /// Performs centroid defuzzification, which calculates the weighted average 
        /// of all possible values in the fuzzy set. The result is a crisp value 
        /// that best represents the fuzzy set.
        /// </summary>
        public static double CentroidDefuzzification(Dictionary<double, double> fuzzySet)
        {
            double numerator = fuzzySet.Sum(pair => pair.Key * pair.Value);
            double denominator = fuzzySet.Sum(pair => pair.Value);
            return denominator == 0 ? 0 : numerator / denominator;
        }
    }

    // Example usage
    public class FUsage
    {
        public FUsage()
        {
            //double x = 5; // Input value to be evaluated
            //double membershipValue = MyFuzzyLogic.GetMembershipValue(FUZZYDOWN.DUNNO, x);
            //Console.WriteLine($"Fuzzy Membership of {x} in DUNNO: {membershipValue}");

            //Dictionary<double, double> fuzzySet = new Dictionary<double, double>
            //{
            //    {2, 0.2},
            //    {4, 0.6},
            //    {6, 0.8},
            //    {8, 1.0},
            //    {10, 0.5}
            //};

            //double defuzzifiedValue = MyFuzzyLogic.CentroidDefuzzification(fuzzySet);
            //Console.WriteLine($"Defuzzified Value: {defuzzifiedValue}");
        }
    }
}