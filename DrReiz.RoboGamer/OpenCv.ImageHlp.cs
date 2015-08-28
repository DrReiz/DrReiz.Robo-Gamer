using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.RoboGamer
{
    public static class ImageHlp
    {
        public static int[,] ToImage(this Bitmap bmp)
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                var image = new int[bmp.Width, bmp.Height];
                for (var y = 0; y < data.Height; ++y)
                {
                    var line = data.Scan0 + data.Stride * y;
                    for (var x = 0; x < data.Width; ++x)
                    {
                        image[x, y] = System.Runtime.InteropServices.Marshal.ReadInt32(line + 4 * x);
                    }
                }
                return image;
            }
            finally
            {
                bmp.UnlockBits(data);
            }

        }
        public static int[,] And(this int[,] image, int[,] prevImage)
        {
            var width = image.GetLength(0);
            var height = image.GetLength(1);

            var outImage = new int[width, height];

            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    var p1 = image[x, y];
                    var p2 = prevImage[x, y];
                    var isSame = (p1 & 0xFF) == 0 || (p2 & 0xFF) == 0;
                    outImage[x, y] = unchecked((int)(isSame ? 0xFF000000 : 0xFFFFFFFF));
                }
            }
            return outImage;
        }

        public static int[,] Diff(this int[,] image, int[,] prevImage, int dx = 0, int dy = 0, int t = 0)
        {
            var width = image.GetLength(0);
            var height = image.GetLength(1);

            var outImage = new int[width, height];

            var minY = dy < 0 ? -dy : 0;
            var maxY = height + (dy > 0 ? -dy : 0);
            var minX = dx < 0 ? -dx : 0;
            var maxX = width + (dx > 0 ? -dx : 0);

            for (var y = minY; y < maxY; ++y)
            {
                for (var x = minX; x < maxX; ++x)
                {
                    var p1 = image[x + dx, y + dy];
                    var p2 = prevImage[x, y];
                    var isSame = Math.Abs((p1 & 0xFF) - (p2 & 0xFF)) <= t
                      && Math.Abs(((p1 >> 8) & 0xFF) - ((p2 >> 8) & 0xFF)) <= t
                      && Math.Abs(((p1 >> 16) & 0xFF) - ((p2 >> 16) & 0xFF)) <= t;
                    outImage[x, y] = unchecked((int)(isSame ? 0xFF000000 : 0xFFFFFFFF));
                }
            }
            return outImage;
        }
        public static Bitmap ToBitmap(this int[,] image)
        {
            var width = image.GetLength(0);
            var height = image.GetLength(1);

            var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                for (var y = 0; y < data.Height; ++y)
                {
                    var line = data.Scan0 + data.Stride * y;
                    for (var x = 0; x < data.Width; ++x)
                    {
                        System.Runtime.InteropServices.Marshal.WriteInt32(line + 4 * x, image[x, y]);
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }
            return bmp;
        }
        public static int[,] Cut(this int[,] image, Rectangle r)
        {
            var res = new int[r.Width, r.Height];
            for (var y = 0; y < r.Height; ++y)
                for (var x = 0; x < r.Width; ++x)
                {
                    res[x, y] = image[x + r.X, y + r.Y];
                }
            return res;
        }

        public static bool Check(this Bitmap bmp, IEnumerable<CheckPoint> checks)
        {
            return checks.All(check => bmp.GetPixel(check.Point.X, check.Point.Y) == check.Color);
        }
        public static void Display(this Bitmap bmp, IEnumerable<CheckPoint> checks)
        {
            foreach (var check in checks)
                Console.WriteLine("{0,4},{1,4}:{2:x8} -> {3:x8}, {4}", check.Point.X, check.Point.Y, check.Color.ToArgb(), bmp.GetPixel(check.Point.X, check.Point.Y).ToArgb(), bmp.GetPixel(check.Point.X, check.Point.Y) == check.Color);
        }

    }

    public class CheckPoint
    {
        public CheckPoint(int x, int y, uint color)
        {
            this.Point = new Point(x, y);
            this.Color = Color.FromArgb((int)color);
        }
        public Point Point;
        public Color Color;
    }
}
