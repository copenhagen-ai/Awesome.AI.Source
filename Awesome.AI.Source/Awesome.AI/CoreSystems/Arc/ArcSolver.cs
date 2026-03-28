namespace Awesome.AI.CoreSystems.Arc
{
    /*
     * find the ARC AGI datasets here: 
     * https://arcprize.org/arc-agi
     * */

    public class ArcSolver
    {
        public bool Matches(int[,] a, int[,] b)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            if (rows != b.GetLength(0) || cols != b.GetLength(1))
                return false;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (a[i, j] != b[i, j])
                        return false;

            return true;
        }

        public int[,] ApplyPrimitive(int[,] grid, Primitive primitive)
        {
            switch (primitive)
            {
                case Primitive.Rotate90: return Rotate90(grid);
                case Primitive.Rotate180: return Rotate180(grid);
                case Primitive.Rotate270: return Rotate270(grid);
                case Primitive.MirrorHorizontal: return MirrorHorizontal(grid);
                case Primitive.MirrorVertical: return MirrorVertical(grid);
                case Primitive.TranslateDown: return Translate(grid);
                case Primitive.TranslateRight: return TranslateRight(grid);
                case Primitive.ColorMap: return ColorMap(grid);
                case Primitive.ColorFill: return ColorFill(grid);
                case Primitive.CropBoundingBox: return CropBoundingBox(grid);
                case Primitive.ExpandCanvas: return ExpandCanvas(grid);
                case Primitive.ObjectCopy: return ObjectCopy(grid);
                default: return (int[,])grid.Clone();
            }
        }

        private int[,] Rotate90(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[cols, rows];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[j, rows - 1 - i] = grid[i, j];

            return result;
        }

        private int[,] Rotate180(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[rows - 1 - i, cols - 1 - j] = grid[i, j];

            return result;
        }

        private int[,] Rotate270(int[,] grid)
        {
            return Rotate90(Rotate180(grid));
        }

        private int[,] MirrorHorizontal(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, cols - 1 - j] = grid[i, j];

            return result;
        }

        private int[,] MirrorVertical(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[rows - 1 - i, j] = grid[i, j];

            return result;
        }

        private int[,] Translate(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows - 1; i++)
                for (int j = 0; j < cols; j++)
                    result[i + 1, j] = grid[i, j];

            return result;
        }

        private int[,] TranslateRight(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols - 1; j++)
                    result[i, j + 1] = grid[i, j];

            return result;
        }

        private int[,] ColorMap(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = (int[,])grid.Clone();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (result[i, j] == 1) result[i, j] = 2;
                    else if (result[i, j] == 2) result[i, j] = 1;
                }

            return result;
        }

        private int[,] ColorFill(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[rows, cols];

            Dictionary<int, int> freq = new Dictionary<int, int>();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    int c = grid[i, j];
                    if (!freq.ContainsKey(c)) freq[c] = 0;
                    freq[c]++;
                }

            int maxColor = freq.OrderByDescending(x => x.Value).First().Key;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = maxColor;

            return result;
        }

        private int[,] CropBoundingBox(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int minI = rows, maxI = 0, minJ = cols, maxJ = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        minI = Math.Min(minI, i);
                        maxI = Math.Max(maxI, i);
                        minJ = Math.Min(minJ, j);
                        maxJ = Math.Max(maxJ, j);
                    }
                }

            if (maxI == 0)
            {
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

        private int[,] ExpandCanvas(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = new int[rows + 2, cols + 2];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i + 1, j + 1] = grid[i, j];

            return result;
        }

        private int[,] ObjectCopy(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int[,] result = (int[,])grid.Clone();

            int halfRows = rows / 2;
            int halfCols = cols / 2;

            for (int i = 0; i < halfRows; i++)
                for (int j = 0; j < halfCols; j++)
                    result[i + halfRows, j + halfCols] = grid[i, j];

            return result;
        }
    }
}