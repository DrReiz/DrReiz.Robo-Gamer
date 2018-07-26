using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrReiz.GumballGamer.Messages;

namespace DrReiz.GumballGamer
{
    public static class ImageStorage
    {
        public static string Save(GameContext context, System.Drawing.Bitmap bitmap)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var bytes = stream.ToArray();

                var name = $"{DateTime.UtcNow:yyMMdd.HHmmss.ffff}";

                System.IO.File.WriteAllBytes(System.IO.Path.Combine(context.StorageDir, $"{name}.png"), bytes);

                return name;
            }
        }
    }
}
