using Newtonsoft.Json.Linq;
using ShaniSoft.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DrReiz.GumballGamer.Messages;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DrReiz.GumballGamer
{
    public class GOcr
    {
        public static void Execute()
        {
            Console.WriteLine(Recognize(@"t:\Data\Jewel\Screenshots\180709.010331.png"));
        }
        public static string Recognize(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var source_image = System.Drawing.Bitmap.FromStream(new MemoryStream(bytes));
            //var image = Invert((Bitmap)source_image);
            var image = source_image;
            var tmpFilename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ppm");
            try
            {
                PNM.WritePNM(tmpFilename, image);
                var process = Process.Start(new ProcessStartInfo(@"p:\Tools\gOcr\gocr049.exe", "-f XML " + tmpFilename) { UseShellExecute = false, RedirectStandardOutput = true });
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
            finally
            {
                if (File.Exists(tmpFilename))
                    File.Delete(tmpFilename);
            }

        }
        static Bitmap Invert(Bitmap image)
        {
            var inverted = new Bitmap(image);
            var data = inverted.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                for (var y = 0; y < data.Height; ++y)
                {
                    var line = data.Scan0 + data.Stride * y;
                    for (var x = 0; x < 4*data.Width; x += 4)
                    {
                        var v = (uint)Marshal.ReadInt32(line + x);
                        //v = (uint)((v & 0xFF000000u) | (0xFFFFFFu - (v & 0xFFFFFFu)));
                        v = (uint)((v & 0xFF000000u) | (0xFFFFFFu - (v & 0xFFFFFFu)));
                        Marshal.WriteInt32(line + x, (int)v);
                    }
                }
            }
            finally
            {
                inverted.UnlockBits(data);
            }
            inverted.Save(@"p:\temp\inverted.png");
            return inverted;
        }

        public static void ToPerception() 
        {
            var shot = ToPerception("180709.010331", File.ReadAllText(@"p:\temp\180709.010331.gocr.xml"));
            var jshot = JObject.FromObject(shot);
            Console.WriteLine(jshot);
            File.WriteAllText(@"p:\temp\180709.010331.perception.json", jshot.ToString());
        }
        public static Shot ToPerception(string name, string gocrXml)
        {
            var xgocr = XElement.Parse(gocrXml);
            var areas = new List<Area>();
            foreach (var (xline, i) in xgocr.Elements("block").Elements("line").Select((xbox, i) => (xbox, i)))
            {
                var text = new StringBuilder();
                foreach (var xbox in xline.Elements())
                {
                    var ch = xbox.Name.LocalName == "space" ? " " : xbox.Attribute("value").Value;
                    if (ch == "(PICTURE)")
                        continue;
                    text.Append(ch);
                }
                var x = int.Parse(xline.Attribute("x").Value);

                areas.Add(new Area(x, int.Parse(xline.Attribute("y").Value), int.Parse(xline.Attribute("dx").Value), int.Parse(xline.Attribute("dy").Value),
                    $"t{i}", text.ToString()));
            }
            return new Shot(name, areas.ToArray());
        }
    }
 

    public class GocrGrain : Orleans.Grain, IGocr
    {
        public async Task<Shot> PerceptionText(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);

            return GOcr.ToPerception(name, GOcr.Recognize(path));
        }

    }
}
