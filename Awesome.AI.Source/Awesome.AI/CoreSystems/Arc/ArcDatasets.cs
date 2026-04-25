using Awesome.AI.Common;
using System.Text.Json;

namespace Awesome.AI.CoreSystems.Arc
{
    /*
     * find the ARC AGI datasets here: 
     * https://arcprize.org/arc-agi
     * */

    public static class ArcDataSets
    {
        public class ArcTask
        {
            public List<ArcPair> train { get; set; }
            public List<ArcPair> test { get; set; }
        }

        public class ArcPair
        {
            public List<List<int>> input { get; set; }
            public List<List<int>> output { get; set; }
        }

        public class ArcLoader
        {
            public static ArcTask LoadTask(string filePath)
            {
                string json = File.ReadAllText(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ArcTask>(json, options);
            }

            private static Random _rnd = new Random();

            public static ArcTask GetRandomTask()
            {
                string root = MyPath.Root;
                string data = MyHelper.IsDebug() ? "Data" : "DataFiles";
                string path = root + data + "\\arc\\training";

                var files = Directory.GetFiles(path, "*.json");

                if (files.Length == 0)
                    throw new Exception("No ARC task files found in folder.");

                string randomFile = files[_rnd.Next(files.Length)];

                //Console.WriteLine($"Selected task: {Path.GetFileName(randomFile)}");

                return LoadTask(randomFile);
            }

            public static (int[,] input, int[,] output) GetRandomTrainingPair()
            {
                var task = GetRandomTask();

                if (task.train == null || task.train.Count == 0)
                    throw new Exception("Task has no training data.");

                var pair = task.train[_rnd.Next(task.train.Count)];

                return (
                    ArcHelper.ToGridDynamic(pair.input),
                    ArcHelper.ToGridDynamic(pair.output)
                );
            }

            public static (int[,] input, int[,] output) GetSimpleTrainingPair(ArcPrimitives primitives)
            {
                int[,] input = ArcHelper.GenerateSimpleGrid();

                Primitive primitive = ArcHelper.GetSimplePrimitive();

                int[,] output = primitives.ApplyPrimitive(input, primitive);

                return (
                    input,
                    output
                );
            }
        }

        public class SyntheticDataset
        {
            public static ArcSample GenerateSampleFromArcPair(ArcPrimitives solver, (int[,] input, int[,] output) pair, int maxSteps = 1)
            {
                int[,] inputGrid = pair.input;
                int[,] current = (int[,])inputGrid.Clone();

                int numPrimitives = Enum.GetNames(typeof(Primitive)).Length;
                bool[] labels = new bool[numPrimitives];

                for (int i = 0; i < maxSteps; i++)
                {
                    Primitive prim = ArcHelper.GetRandomPrimitive();
                    current = solver.ApplyPrimitive(current, prim);
                    labels[(int)prim] = true;
                }

                return new ArcSample
                {
                    Features = ArcHelper.FlattenSmart(inputGrid, current),
                    Labels = labels
                };
            }

            public static ArcSample[] GenerateDatasetFromArcFolder(ArcPrimitives solver, int datasetSize, int maxSteps)
            {
                ArcSample[] dataset = new ArcSample[datasetSize];

                for (int i = 0; i < datasetSize; i++)
                {
                    var pair = ArcLoader.GetRandomTrainingPair();
                    dataset[i] = GenerateSampleFromArcPair(solver, pair, maxSteps);
                }

                return dataset;
            }
        }

        public static class SimpleDataset
        {
            public static Primitive[] simple =
                {
                    Primitive.Rotate90,
                    Primitive.Rotate180,
                    Primitive.MirrorHorizontal,
                    Primitive.MirrorVertical
                };

            public static ArcSample[] Generate(int count, ArcPrimitives primitives)
            {
                var data = new List<(int[,], int[,], Primitive)>();

                for (int i = 0; i < count; i++)
                {
                    int[,] input = ArcHelper.GenerateSimpleGrid();

                    Primitive primitive = ArcHelper.GetSimplePrimitive();

                    int[,] output = primitives.ApplyPrimitive(input, primitive);

                    data.Add((input, output, primitive));
                }

                //return data;

                ArcSample[] res = new ArcSample[data.Count];

                for (int i = 0; i < data.Count; i++)
                {
                    bool[] labels = new bool[simple.Length];
                    labels[Array.IndexOf(simple, data[i].Item3)] = true;
                    res[i] = new ArcSample
                    {
                        Features = ArcHelper.FlattenSmart(data[i].Item1, data[i].Item2),
                        Labels = labels,
                    };
                }

                return res;
            }
        }
    }
}

//public static int[,] GenerateRandomGrid(int minSize = 3, int maxSize = 8, int numColors = 3)
//{
//    int rows = _rnd.Next(minSize, maxSize + 1);
//    int cols = _rnd.Next(minSize, maxSize + 1);

//    int[,] grid = new int[rows, cols];

//    for (int i = 0; i < rows; i++)
//        for (int j = 0; j < cols; j++)
//            grid[i, j] = _rnd.Next(numColors); // colors: 0..numColors-1

//    return grid;
//}

//public static ArcSample GenerateSample(ArcSolver2 solver)
//{
//    var input = GenerateRandomGrid();

//    Primitive primitive = GetRandomPrimitive();

//    var output = solver.ApplyPrimitive(input, primitive);

//    int numPrimitives = Enum.GetNames(typeof(Primitive)).Length;
//    bool[] labels = new bool[numPrimitives];

//    labels[(int)primitive] = true;

//    return new ArcSample
//    {
//        Features = ArcFlatten.FlattenSmart(input, output),
//        Labels = labels
//    };
//}

//public static ArcSample[] GenerateDataset(ArcSolver2 solver, int count)
//{
//    ArcSample[] samples = new ArcSample[count];

//    for (int i = 0; i < count; i++)
//        samples[i] = GenerateSample(solver);

//    return samples;
//}

//public static ArcSample GenerateSequenceSample(ArcSolver2 solver, int maxSteps = 3)
//{
//    var input = GenerateRandomGrid();

//    int steps = _rnd.Next(1, maxSteps + 1);

//    var current = (int[,])input.Clone();

//    int numPrimitives = Enum.GetNames(typeof(Primitive)).Length;
//    bool[] labels = new bool[numPrimitives];

//    for (int i = 0; i < steps; i++)
//    {
//        var prim = GetRandomPrimitive();
//        current = solver.ApplyPrimitive(current, prim);
//        labels[(int)prim] = true;
//    }

//    return new ArcSample
//    {
//        Features = ArcFlatten.FlattenSmart(input, current),
//        Labels = labels
//    };
//}

//private static Random _rnd = new Random();

// Pick a random input/output pair from ARC training data
//public static (int[,] input, int[,] output) PickRandomPair(List<(int[,], int[,])> trainingPairs)
//{
//    int idx = _rnd.Next(trainingPairs.Count);
//    return trainingPairs[idx];
//}