using System;

namespace Library
{
    /// <summary>
    /// Represents a heat map in terms of a grid of 0-1 values.
    /// </summary>
    public class Heatmap
    {
        private readonly int width, height;
        private readonly double[,] grid;

        private Heatmap(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Heatmap width must be >= 1.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("width", "Heatmap height must be >= 1.");

            this.width = width;
            this.height = height;
            this.grid = new double[width, height];
        }

        /// <summary>
        /// Gets the value of a single cell in the heat map.
        /// </summary>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <returns>A value in the range 0-1 representing the weight in the <c>[x, y]</c> cell.</returns>
        public double this[int x, int y]
        {
            get { return grid[x, y]; }
        }

        /// <summary>
        /// Gets the width of the heat map (the number of cells).
        /// </summary>
        public int Width
        {
            get { return width; }
        }
        /// <summary>
        /// Gets the height of the heat map (the number of cells).
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Gets the total number of cells in the heat map.
        /// </summary>
        public int CellCount
        {
            get { return width * height; }
        }

        /// <summary>
        /// Constructs a <see cref="Heatmap"/> from an array of integers by comparing its values to those in the min-max range.
        /// </summary>
        /// <param name="data">The data that should be translated into a <see cref="Heatmap"/>.</param>
        /// <param name="min">The minimum value of values in the <see cref="Heatmap"/>. Anything below this value is set to <c>0</c>.</param>
        /// <param name="max">The maximum value of values in the <see cref="Heatmap"/>. Anything above this value is set to <c>1</c>.</param>
        /// <returns>A new <see cref="Heatmap"/> constructed from <paramref name="data"/>.</returns>
        public static Heatmap ConstructByCount(int[,] data, int min, int max)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Rank != 2)
                throw new ArgumentException("The data for the map must be a two-dimensional array.", "data");

            int w = data.GetLength(0);
            int h = data.GetLength(1);

            if (w <= 0)
                throw new ArgumentOutOfRangeException("data", "Heatmap width must be >= 1.");
            if (h <= 0)
                throw new ArgumentOutOfRangeException("data", "Heatmap height must be >= 1.");

            double mi = min;
            double ma = max - min;

            Heatmap map = new Heatmap(w, h);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    double d = data[x, y] - mi;
                    if (d < 0) d = 0;
                    else if (d > ma) d = ma;
                    map.grid[x, y] = d / ma;
                }

            return map;
        }
    }
}
