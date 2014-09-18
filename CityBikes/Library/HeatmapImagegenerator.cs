using System;
using System.Drawing;

namespace Library
{
    public static class HeatmapImagegenerator
    {
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
