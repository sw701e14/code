using System;

namespace Library
{
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

        public double this[int x, int y]
        {
            get { return grid[x, y]; }
        }

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public int CellCount
        {
            get { return width * height; }
        }
    }
}
