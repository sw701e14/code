using System;
using System.Collections.Generic;

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
                throw new ArgumentOutOfRangeException("height", "Heatmap height must be >= 1.");

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
        /// <param name="min">
        /// The minimum value of values in the <see cref="Heatmap"/>.
        /// Anything below <paramref name="min"/> is set to <c>0</c>.
        /// If set to <c>null</c>, the minimum is set to be the lowest value in <paramref name="data"/>.</param>
        /// <param name="max">
        /// The maximum value of values in the <see cref="Heatmap"/>.
        /// Anything above <paramref name="max"/> is set to <c>1</c>.
        /// If set to <c>null</c>, the maximum is set to be the greatest value in <paramref name="data"/>.</param>
        /// <returns>A new <see cref="Heatmap"/> constructed from <paramref name="data"/>.</returns>
        public static Heatmap ConstructByCount(int[,] data, int? min, int? max)
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

            if (!min.HasValue && !max.HasValue)
                getMinMax(data, w, h, out min, out max);
            else if (!min.HasValue)
                getMin(data, w, h, out min);
            else if (!max.HasValue)
                getMax(data, w, h, out max);

            double mi = min.Value;
            double ma = max.Value - min.Value;

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
        private static void getMax(int[,] data, int w, int h, out int? max)
        {
            max = int.MinValue;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (data[x, y] > max)
                        max = data[x, y];
        }
        private static void getMin(int[,] data, int w, int h, out int? min)
        {
            min = int.MaxValue;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (data[x, y] < min)
                        min = data[x, y];
        }
        private static void getMinMax(int[,] data, int w, int h, out int? min, out int? max)
        {
            min = int.MaxValue;
            max = int.MinValue;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (data[x, y] < min)
                        min = data[x, y];
                    if (data[x, y] > max)
                        max = data[x, y];
                }
        }

        /// <summary>
        /// Constructs a <see cref="Heatmap"/> from a collection of elements by mapping the elements to cells and counting them.
        /// </summary>
        /// <typeparam name="T">The type of the elements that are mapped to the <see cref="Heatmap"/>.</typeparam>
        /// <param name="width">The width of the <see cref="Heatmap"/> (in number of cells).</param>
        /// <param name="height">The height of the <see cref="Heatmap"/> (in number of cells).</param>
        /// <param name="collection">The elements that are mapped to the <see cref="Heatmap"/>.</param>
        /// <param name="min">
        /// The minimum count of a cell, corresponding to a value of <c>0</c>.
        /// Cells with count less than <paramref name="min"/> is set to <c>0</c>.
        /// If set to <c>null</c>, the minimum is set to be the lowest count.
        /// </param>
        /// <param name="max">
        /// The maximum count of a cell, corresponding to a value of <c>1</c>.
        /// Cells with count greater than <paramref name="max"/> is set to <c>1</c>.
        /// If set to <c>null</c>, the maximum is set to be the greatest count.</param>
        /// <param name="mapper">A method that performs mapping of <typeparamref name="T"/> onto a grid.</param>
        /// <returns>A new <see cref="Heatmap"/> constructed from <paramref name="collection"/>.</returns>
        public static Heatmap ConstructByCount<T>(int width, int height, IEnumerable<T> collection, int? min, int? max, MapToIndex<T> mapper)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Heatmap width must be >= 1.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Heatmap height must be >= 1.");

            int x, y;
            int[,] data = new int[width, height];
            foreach(var e in collection)
            {
                mapper(e, width, height, out x, out y);
                data[x, y]++;
            }

            return ConstructByCount(data, min, max);
        }
    }
}
