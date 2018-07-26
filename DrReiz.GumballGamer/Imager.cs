using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer
{
    class Imager
    {
        public static (int h, int s, int v) ToHsv(string filename)
        {
            var src = new Mat(filename);
            var hsv_src = new Mat();
            Cv2.CvtColor(src, hsv_src, ColorConversionCodes.RGB2HSV);

            var thumbnail = new Mat();
            Cv2.Resize(src, thumbnail, new Size(1, 1));

            var hsv_thumbnail = new Mat();
            Cv2.CvtColor(thumbnail, hsv_thumbnail, ColorConversionCodes.RGB2HSV);
            var p = hsv_thumbnail.Get<Vec3b>(0, 0);
            return (p[0], p[1], p[2]);
        }
    }
}
