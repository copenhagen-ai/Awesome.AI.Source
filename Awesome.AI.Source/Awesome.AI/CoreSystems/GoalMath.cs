using Awesome.AI.Core;
using System.Text.RegularExpressions;

namespace Awesome.AI.CoreSystems
{
    public interface IMathStrategy
    {
        bool CanHandle(string input);
        string Solve(string input);
    }

    public class GoalMath
    {
        private ExampleMemory mem { get; set; }

        private TheMind mind;
        private GoalMath() { }
        public GoalMath(TheMind mind)
        {
            this.mind = mind;

            this.mind.result_math = "nothing yet..";

            this.mem = new ExampleMemory();
        }

        private List<string> _problems = new List<string>() 
        { 
            "calc: 5 * (3 + 2)", 
            "solve: 2x + 4 = 10", 
            "calc: 3 * (3 + 2)", 
            "solve: 2x + 2 = 10", 
            "calc: 2 * (3 + 2)", 
            "solve: 2x + 6 = 10" 
        };

        public string GetProblem(int index)
        {
            int rand = index == -1 ? 
                mind.rand.MyRandomInt(1, 59)[0] : 
                index;

            return _problems[rand / 10];
        }

        private List<IMathStrategy> _strategies = new();
        public void AddStrategy(IMathStrategy strategy)
        {
            if (Knows(strategy))
                return;

            _strategies.Add(strategy);
        }

        public void Solve(string problem, bool _pro)
        {
            if (!_pro)
                return;

            if (mind.z_current != "z_noise")
                return;

            if (!mind._quick.Result("MATHSOLVE"))
                return;

            foreach (var strategy in _strategies)
            {
                if (strategy.CanHandle(problem))
                {
                    mind.result_math = strategy.Solve(problem);
                    return;
                }
            }

            mind.result_math = "i don't know how to solve this yet.";
        }

        public void Learn(string problem, bool _pro)
        {
            if (!_pro)
                return;

            if (mind.z_current != "z_noise")
                return;

            if (!mind._quick.Result("MATHLEARN"))
                return;

            var result = mem.TrySolve(problem);

            if (result == null)
                mem.Learn(problem);

            else if (result == "calc")
                AddStrategy(new SolveExpressionStrategy());

            else if (result == "solve")
                AddStrategy(new LinearEquationStrategy());            
        }

        public bool Knows(IMathStrategy strategy)
        {
            foreach (var _st in _strategies)
            { 
                if (strategy.GetType() == _st.GetType()) 
                    return true;
            }

            return false;
        }
    }

    public class ExampleMemory
    {
        private Dictionary<string, string> _examples = new();

        public void Learn(string problem)
        {
            if (problem.StartsWith("calc"))
                _examples[problem] = "calc";

            if (problem.StartsWith("solve"))
                _examples[problem] = "solve";
        }

        public string? TrySolve(string problem)
        {
            return _examples.ContainsKey(problem)
                ? _examples[problem]
                : null;
        }        
    }

    // Simple linear equation solver: ax + b = c
    public class LinearEquationStrategy : IMathStrategy
    {
        public bool CanHandle(string input)
        {
            return input.Contains("solve") || (input.Contains("x") && input.Contains("="));
        }

        public string Solve(string input)
        {
            try
            {
                // Example: "2x + 4 = 10"
                var match = Regex.Match(input, @"(\d*)x\s*([\+\-]\s*\d+)?\s*=\s*(\d+)");

                if (!match.Success)
                    return "format not supported.";

                int a = string.IsNullOrEmpty(match.Groups[1].Value) ? 1 : int.Parse(match.Groups[1].Value);
                int b = match.Groups[2].Success ? int.Parse(match.Groups[2].Value.Replace(" ", "")) : 0;
                int c = int.Parse(match.Groups[3].Value);

                double x = (double)(c - b) / a;
                input = input.Replace("solve: ", "");
                return $"result: {input}, x = {x}";
            }
            catch
            {
                return "failed to evaluate expression.";
            }
        }
    }

    // Very simple expression evaluator (only +, -, *, /)
    public class SolveExpressionStrategy : IMathStrategy
    {
        public bool CanHandle(string expression)
        {
            return expression.Contains("calc");
        }

        public string Solve(string expression)
        {
            try
            {
                expression = expression.Replace("calc: ", "");
                var result = Convert.ToDouble(new System.Data.DataTable().Compute(expression, ""));
                return $"result: {expression} = {result}";
            }
            catch (Exception ex)
            {
                return "failed to evaluate expression.";
            }
        }
    }
}
