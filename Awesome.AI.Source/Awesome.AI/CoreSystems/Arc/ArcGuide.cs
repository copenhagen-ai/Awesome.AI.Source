namespace Awesome.AI.CoreSystems.Arc
{
    /*
     * find the ARC AGI datasets here: 
     * https://arcprize.org/arc-agi
     * */

    public class ArcGuide
    {
        private ArcSolver _solver;

        public ArcGuide()
        {
            _solver = new ArcSolver();
        }

        public List<Primitive> Solve(int[,] inputGrid, int[,] targetGrid, Dictionary<Primitive, float> primitiveProbs, int maxSequenceLength = 3, float threshold = 0.5f, float decay = 0.8f)
        {
            var primitives = primitiveProbs
                .Where(kvp => kvp.Value >= threshold)
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();

            var triedCandidates = new HashSet<string>();

            for (int length = 1; length <= maxSequenceLength; length++)
            {
                var sequences = GenerateSequences(primitives, length);

                foreach (var seq in sequences)
                {
                    int[,] candidate = (int[,])inputGrid.Clone();
                    foreach (var prim in seq)
                        candidate = _solver.ApplyPrimitive(candidate, prim);

                    string candidateKey = GridToString(candidate);
                    if (triedCandidates.Contains(candidateKey))
                        continue;

                    triedCandidates.Add(candidateKey);

                    if (_solver.Matches(candidate, targetGrid))
                        return seq;

                    foreach (var prim in seq)
                        primitiveProbs[prim] *= decay;
                }
            }

            return null;
        }

        private IEnumerable<List<Primitive>> GenerateSequences(List<Primitive> primitives, int length)
        {
            if (length == 1)
            {
                foreach (var p in primitives)
                    yield return new List<Primitive> { p };
            }
            else
            {
                foreach (var p in primitives)
                {
                    foreach (var subseq in GenerateSequences(primitives, length - 1))
                    {
                        var newSeq = new List<Primitive> { p };
                        newSeq.AddRange(subseq);
                        yield return newSeq;
                    }
                }
            }
        }

        private string GridToString(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            char[] chars = new char[rows * cols];
            int idx = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    chars[idx++] = (char)(grid[i, j] + '0');
            return new string(chars);
        }
    }
}