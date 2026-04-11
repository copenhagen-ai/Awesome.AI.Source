using Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems.Arc
{
    /*
     * find the ARC AGI datasets here: 
     * https://arcprize.org/arc-agi
     * */

    public interface IArcStrategy
    {
        bool CanHandle(string input, string step);
        string Solve(out ArcClassifier mlp);
        string Solve(ArcClassifier mlp);
    }

    public class ArcGoal
    {
        ArcClassifier mlp = null;
        
        private ExampleMemory mem;

        private TheMind mind;
        private ArcGoal() { }
        public ArcGoal(TheMind mind)
        {
            this.mind = mind;

            this.mind.result_math = "nothing yet..";

            mem = new ExampleMemory();

            mind.result_arc = ", nothing yet..";

            step = "none";
        }

        private List<string> _problems = new List<string>()
        {
            "train",
            "solve"
        };

        public string GetProblem(int index)
        {
            int rand = index == -1 ?
                mind.rand.MyRandomInt(1, 19)[0] :
                index;

            return _problems[rand / 10];
        }

        private List<IArcStrategy> _strategies = new();
        public void AddStrategy(IArcStrategy strategy)
        {
            if (Knows(strategy))
                return;

            _strategies.Add(strategy);
        }

        private string step { get; set; }
        public void Solve(string problem, bool _pro)
        {
            //if (!mind.HasAccess(2))
            //    return;

            if (mind.bot.arc == ARC.DONTUSE)
                return;

            if (!_pro)
                return;

            if (!mind._quick.Result("ARCSOLVE"))
                return;

            if (!mind.result_arc.Contains("match"))
                mind.result_arc = ", solving..";

            foreach (var strategy in _strategies)
            {
                if (strategy.CanHandle(problem, step))
                {
                    if (problem == "train" && mlp == null)
                        mind.result_arc = strategy.Solve(out mlp);

                    if (problem == "solve" && mlp != null)
                        mind.result_arc = strategy.Solve(mlp);
                }
            }

            if (mind.result_arc.Contains("solving"))
                mind.result_arc = ", i don't know how to solve this yet.";

            if (mind.result_arc.Contains("training done"))
                step = "train";

            if (mind.result_arc.Contains("matches found"))
                step = "solve";
        }

        public void Learn(string problem, bool _pro)
        {
            if (mind.bot.arc == ARC.DONTUSE)
                return;

            if (!_pro)
                return;

            if (!mind._quick.Result("ARCLEARN"))
                return;

            var result = mem.TrySolve(problem);

            if (result == null)
                mem.Learn(problem);

            else if (result == "train")
                AddStrategy(new ArcTrain());

            else if (result == "solve")
                AddStrategy(new ArcSolve());
        }

        public bool Knows(IArcStrategy strategy)
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
            if (problem.StartsWith("train"))
                _examples[problem] = "train";

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

    public class ArcTrain : IArcStrategy
    {
        public bool CanHandle(string input, string step)
        {
            if(step != "none")
                return false;

            return input.Contains("train");
        }

        public string Solve(ArcClassifier mlp)
        {
            throw new NotImplementedException("ArcTrain, Solve");
        }

        public string Solve(out ArcClassifier mlp)
        {
            try
            {
                var primitives = new ArcPrimitives();

                Console.WriteLine("LOADING DATA..");

                var dataset = ArcDataSets.SimpleDataset.Generate(500, primitives);
                
                int inputSize = 10 * 10 * 2; // input+output flattened
                int hiddenSize = 64;
                int outputSize = 4;// Enum.GetNames(typeof(Primitive)).Length;

                mlp = new ArcClassifier(inputSize, hiddenSize, outputSize);

                Console.WriteLine("TRAINING..");

                mlp.Train(dataset, epochs: 1000, lr: 0.1f);
                
                Console.WriteLine("DONE TRAINING..");

                return $", result: training done";
            }
            catch (Exception ex)
            {
                mlp = null;
                return ", failed to train.";
            }
        }        
    }

    public class ArcSolve : IArcStrategy
    {
        private int match = 0;
        private int no_match = 0;
        public bool CanHandle(string expression, string step)
        {
            if (step != "train")
                return false;

            return expression.Contains("solve");
        }

        public string Solve(out ArcClassifier mlp)
        {
            throw new NotImplementedException("ArcTrain, Solve");
        }

        public string Solve(ArcClassifier mlp)
        {
            try
            {
                var solver = new ArcSolver();
                var primitives = new ArcPrimitives();

                var _in = ArcDataSets.ArcLoader.GetSimpleTrainingPair(primitives);

                int[,] inputGrid = _in.input;
                int[,] outputGrid = _in.output;


                var probs = new Dictionary<Primitive, float>();
                float[] _outs = mlp.Predict(ArcHelper.FlattenSmart(inputGrid, outputGrid));
                for (int i = 0; i < _outs.Length; i++)
                    probs.Add((Primitive)i, _outs[i]);
                
                List<Primitive> result = solver.Solve(inputGrid, outputGrid, probs);

                if (result == null)
                    no_match++;
                else
                    match++;
                    
                return $", result: no match[" + no_match + "], match[" + match + "]";
            }
            catch (Exception ex)
            {
                return ", failed to solve.";
            }
        }
    }
}
