using System;
using System.Drawing;

namespace Library
{
    /// <summary>
    /// Provides static methods for generating images that represent <see cref="Heatmap"/>s.
    /// </summary>
    public static class HeatmapImagegenerator
    {
        /// <summary>
        /// Generates an image that represents a given <see cref="Heatmap"/> using a userdefined color-rule.
        /// </summary>
        /// <param name="map">The <see cref="Heatmap"/> from which the image should be generated.</param>
        /// <param name="imageWidth">Width of the image (in pixels).</param>
        /// <param name="imageHeight">Height of the image (in pixels).</param>
        /// <param name="getColor">A method that, given the weight of a single cell, returns the color of the cell.</param>
        /// <returns>The generated <see cref="Heatmap"/>.</returns>
        public static Image Generate(Heatmap map, int imageWidth, int imageHeight, Func<double, Color> getColor)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            if (getColor == null)
                throw new ArgumentNullException("getColor");

            if (imageWidth <= 0)
                throw new ArgumentOutOfRangeException("imageWidth", "Image width must be >= 1.");

            if (imageHeight <= 0)
                throw new ArgumentOutOfRangeException("imageHeight", "Image height must be >= 1.");

            Bitmap bmp = new Bitmap(imageWidth, imageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                double w = (double)imageWidth / (double)map.Width;
                double h = (double)imageHeight / (double)map.Height;

                for (int y = 0; y < map.Height; y++)
                    for (int x = 0; x < map.Width; x++)
                    {
                        using (SolidBrush brush = new SolidBrush(getColor(map[x, y])))
                            g.FillRectangle(brush, (float)(x * w), (float)(y * h), (float)w, (float)h);
                    }
            }

            return bmp;
        }
    }
}
