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
        /// Generates an image that represents a given <see cref="Heatmap"/> using a two color scale.
        /// </summary>
        /// <param name="map">The <see cref="Heatmap"/> from which the image should be generated.</param>
        /// <param name="imageWidth">Width of the image (in pixels). If <c>null</c> is specified, the generated image will have the same width as the number of cells in the <see cref="Heatmap"/>.</param>
        /// <param name="imageHeight">Height of the image (in pixels). If <c>null</c> is specified, the generated image will have the same height as the number of cells in the <see cref="Heatmap"/>.</param>
        /// <param name="min">The <see cref="Color"/> associated with a cell value of <c>0.0</c>.</param>
        /// <param name="max">The <see cref="Color"/> associated with a cell value of <c>1.0</c>.</param>
        /// <returns>The generated <see cref="Heatmap"/>.</returns>
        public static Image Generate(Heatmap map, int imageWidth, int imageHeight, Color min, Color max)
        {
            return Generate(map, imageWidth, imageHeight, value =>
            {
                if (value <= 0.0)
                    return min;
                else if (value >= 1)
                    return max;
                else
                    return getColorAtPercent(min, max, value);
            });
        }
        /// <summary>
        /// Generates an image that represents a given <see cref="Heatmap"/> using a three color scale.
        /// </summary>
        /// <param name="map">The <see cref="Heatmap"/> from which the image should be generated.</param>
        /// <param name="imageWidth">Width of the image (in pixels). If <c>null</c> is specified, the generated image will have the same width as the number of cells in the <see cref="Heatmap"/>.</param>
        /// <param name="imageHeight">Height of the image (in pixels). If <c>null</c> is specified, the generated image will have the same height as the number of cells in the <see cref="Heatmap"/>.</param>
        /// <param name="min">The <see cref="Color"/> associated with a cell value of <c>0.0</c>.</param>
        /// <param name="mid">The <see cref="Color"/> associated with a cell value of <c>0.5</c>.</param>
        /// <param name="max">The <see cref="Color"/> associated with a cell value of <c>1.0</c>.</param>
        /// <returns>The generated <see cref="Heatmap"/>.</returns>
        public static Image Generate(Heatmap map, int? imageWidth, int? imageHeight, Color min, Color mid, Color max)
        {
            return Generate(map, imageWidth, imageHeight, value =>
                {
                    if (value <= 0.0)
                        return min;
                    else if (value < 0.5)
                        return getColorAtPercent(min, mid, value * 2);
                    else if (value == 0.5)
                        return mid;
                    else if (value >= 1)
                        return max;
                    else
                        return getColorAtPercent(mid, max, (value - 0.5) * 2);
                });
        }

        /// <summary>
        /// Generates an image that represents a given <see cref="Heatmap"/> using a userdefined color-rule.
        /// </summary>
        /// <param name="map">The <see cref="Heatmap"/> from which the image should be generated.</param>
        /// <param name="imageWidth">Width of the image (in pixels). If <c>null</c> is specified, the generated image will have the same width as the number of cells in the <see cref="Heatmap"/>.</param>
        /// <param name="imageHeight">Height of the image (in pixels). If <c>null</c> is specified, the generated image will have the same height as the number of cells in the <see cref="Heatmap"/>.</param>
        /// <param name="getColor">A method that, given the weight of a single cell, returns the color of the cell.</param>
        /// <returns>The generated <see cref="Heatmap"/>.</returns>
        public static Image Generate(Heatmap map, int? imageWidth, int? imageHeight, Func<double, Color> getColor)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            if (getColor == null)
                throw new ArgumentNullException("getColor");

            if (imageWidth <= 0)
                throw new ArgumentOutOfRangeException("imageWidth", "Image width must be >= 1.");

            if (imageHeight <= 0)
                throw new ArgumentOutOfRangeException("imageHeight", "Image height must be >= 1.");

            if (!imageWidth.HasValue)
                imageWidth = map.Width;
            if (!imageHeight.HasValue)
                imageHeight = map.Height;

            Bitmap bmp = new Bitmap(imageWidth.Value, imageHeight.Value, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                double w = (double)imageWidth.Value / (double)map.Width;
                double h = (double)imageHeight.Value / (double)map.Height;

                for (int y = 0; y < map.Height; y++)
                    for (int x = 0; x < map.Width; x++)
                    {
                        using (SolidBrush brush = new SolidBrush(getColor(map[x, y])))
                            g.FillRectangle(brush, (float)(x * w), (float)(y * h), (float)w, (float)h);
                    }
            }

            return bmp;
        }

        private static Color getColorAtPercent(Color color1, Color color2, double percent)
        {
            if (percent < 0 || percent > 1)
                throw new ArgumentOutOfRangeException("percent", "Percent must be a value between 0 and 1.");

            double resultAlpha = color1.A + percent * (color2.A - color1.A);
            double resultRed = color1.R + percent * (color2.R - color1.R);
            double resultGreen = color1.G + percent * (color2.G - color1.G);
            double resultBlue = color1.B + percent * (color2.B - color1.B);

            return Color.FromArgb((int)resultAlpha, (int)resultRed, (int)resultGreen, (int)resultBlue);
        }
    }
}
