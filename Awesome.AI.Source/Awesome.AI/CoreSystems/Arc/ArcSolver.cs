namespace Awesome.AI.CoreSystems.Arc
{
    /*
     * find the ARC AGI datasets here: 
     * https://arcprize.org/arc-agi
     * */

    public class ArcSolver
    {
        private ArcPrimitives _primitives;

        public ArcSolver()
        {
            _primitives = new ArcPrimitives();
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
                        candidate = _primitives.ApplyPrimitive(candidate, prim);

                    string candidateKey = ArcHelper.GridToString(candidate);
                    if (triedCandidates.Contains(candidateKey))
                        continue;

                    triedCandidates.Add(candidateKey);

                    if (_primitives.Matches(candidate, targetGrid))
                        return seq;

                    foreach (var prim in seq)
                        primitiveProbs[prim] *= decay;
                }
            }

            return null;
        }

        private List<List<Primitive>> GenerateSequences(List<Primitive> primitives, int length)
        {
            int n = primitives.Count;
            var result = new List<List<Primitive>>();

            if (n == 0 || length <= 0)
                return result;

            int[] indices = new int[length];

            while (true)
            {
                var sequence = new List<Primitive>(length);
                for (int i = 0; i < length; i++)
                    sequence.Add(primitives[indices[i]]);

                result.Add(sequence);

                int position = length - 1;
                while (position >= 0)
                {
                    indices[position]++;
                    if (indices[position] < n)
                        break;

                    indices[position] = 0;
                    position--;
                }

                if (position < 0)
                    break;
            }

            return result;
        }
    }
}

//private IEnumerable<List<Primitive>> GenerateSequences(List<Primitive> primitives, int length)
//{
//    if (length == 1)
//    {
//        foreach (var p in primitives)
//            yield return new List<Primitive> { p };
//    }
//    else
//    {
//        foreach (var p in primitives)
//        {
//            foreach (var subseq in GenerateSequences(primitives, length - 1))
//            {
//                var newSeq = new List<Primitive> { p };
//                newSeq.AddRange(subseq);
//                yield return newSeq;
//            }
//        }
//    }
//}