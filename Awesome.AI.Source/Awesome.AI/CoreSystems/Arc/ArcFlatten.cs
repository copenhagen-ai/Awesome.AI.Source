namespace Awesome.AI.CoreSystems.Arc
{
    /*
     * find the ARC AGI datasets here: 
     * https://arcprize.org/arc-agi
     * */

    public class ArcFlatten
    {
        private static int[,] CropBoundingBox(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int minI = rows, maxI = 0, minJ = cols, maxJ = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i, j] != 0) // assume 0 = background
                    {
                        minI = Math.Min(minI, i);
                        maxI = Math.Max(maxI, i);
                        minJ = Math.Min(minJ, j);
                        maxJ = Math.Max(maxJ, j);
                    }
                }

            // 🚨 Edge case: everything is background
            if (maxI == 0)
            {
                // return original grid OR 1x1 grid
                return (int[,])grid.Clone();
            }

            int newRows = maxI - minI + 1;
            int newCols = maxJ - minJ + 1;

            int[,] result = new int[newRows, newCols];

            for (int i = 0; i < newRows; i++)
                for (int j = 0; j < newCols; j++)
                    result[i, j] = grid[minI + i, minJ + j];

            return result;
        }


        public static float[] FlattenFixed(int[,] input, int[,] output, int maxSize = 10)
        {
            int size = maxSize * maxSize;
            float[] features = new float[size * 2];

            int idx = 0;

            // input
            for (int i = 0; i < maxSize; i++)
                for (int j = 0; j < maxSize; j++)
                    features[idx++] =
                        (i < input.GetLength(0) && j < input.GetLength(1))
                        ? input[i, j]
                        : 0;

            // output
            for (int i = 0; i < maxSize; i++)
                for (int j = 0; j < maxSize; j++)
                    features[idx++] =
                        (i < output.GetLength(0) && j < output.GetLength(1))
                        ? output[i, j]
                        : 0;

            return features;
        }

        public static float[] FlattenSmart(int[,] input, int[,] output, int maxSize = 10)
        {
            input = CropIfNeeded(input, maxSize);
            output = CropIfNeeded(output, maxSize);

            return FlattenFixed(input, output, maxSize);
        }

        private static int[,] CropIfNeeded(int[,] grid, int maxSize)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            if (rows <= maxSize && cols <= maxSize)
                return grid;

            // crop bounding box of non-zero content
            return CropBoundingBox(grid);
        }
    }
}

//public static float[] Flatten(int[,] input, int[,] output, int size = 10)
//{
//    //int size_a = input.GetLength(0);
//    //int size_b = input.GetLength(1);
//    //int size_out_a = output.GetLength(0);
//    //int size_out_b = output.GetLength(1);

//    float[] features = new float[size * size * 2];
//    int idx = 0;

//    for (int i = 0; i < size; i++)
//        for (int j = 0; j < size; j++)
//            features[idx++] = input[i, j];

//    for (int i = 0; i < size; i++)
//        for (int j = 0; j < size; j++)
//            features[idx++] = output[i, j];

//    return features;
//}

//public static float[] FlattenDynamic(int[,] input, int[,] output)
//{
//    int rows = Math.Max(input.GetLength(0), output.GetLength(0));
//    int cols = Math.Max(input.GetLength(1), output.GetLength(1));

//    int size = rows * cols;

//    float[] features = new float[size * 2];

//    int idx = 0;

//    // input
//    for (int i = 0; i < rows; i++)
//        for (int j = 0; j < cols; j++)
//            features[idx++] = (i < input.GetLength(0) && j < input.GetLength(1)) ? input[i, j] : 0;

//    // output
//    for (int i = 0; i < rows; i++)
//        for (int j = 0; j < cols; j++)
//            features[idx++] = (i < output.GetLength(0) && j < output.GetLength(1)) ? output[i, j] : 0;

//    return features;
//}