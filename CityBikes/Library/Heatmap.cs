using System;

namespace Library
{
    public class Heatmap
    {
        private double[,] grid;

        public Heatmap(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Heatmap width must be >= 1.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("width", "Heatmap height must be >= 1.");

            this.grid = new double[width, height];
        }

        public double this[int x, int y]
        {
            get { return grid[x, y]; }
        }
    }
}
