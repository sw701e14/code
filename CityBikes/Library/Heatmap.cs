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
    }
}
