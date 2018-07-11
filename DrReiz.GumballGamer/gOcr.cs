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

namespace DrReiz.GumballGamer
{
    public class GOcr
    {
        public static void Execute()
        {
            var bytes = File.ReadAllBytes(@"t:\Data\Jewel\Screenshots\180709.010331.png");
            var image = System.Drawing.Bitmap.FromStream(new MemoryStream(bytes));            
            var tmpFilename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ppm");
            try
            {
                PNM.WritePNM(tmpFilename, image);
                var process = Process.Start(new ProcessStartInfo(@"p:\Tools\gOcr\gocr049.exe", "-f XML " + tmpFilename) { UseShellExecute = false, RedirectStandardOutput = true });
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(output);
            }
            finally
            {
                if (File.Exists(tmpFilename))
                    File.Delete(tmpFilename);
            }
        }
        public static void GocrToPerception() 
        {
            var xgocr = XElement.Parse(File.ReadAllText(@"p:\temp\180709.010331.gocr.xml"));
            var areas = new List<Area>();
            foreach (var (xbox,i) in xgocr.Elements("block").Elements("line").Elements("box").Select((xbox,i) => (xbox, i)))
            {
                areas.Add(new Area(int.Parse(xbox.Attribute("x").Value), int.Parse(xbox.Attribute("y").Value), int.Parse(xbox.Attribute("dx").Value), int.Parse(xbox.Attribute("dy").Value),
                    $"t{i}", xbox.Attribute("value").Value));
            }
            var jshot = JObject.FromObject(new Shot("180709.010331", areas.ToArray()));
            Console.WriteLine(jshot);
            File.WriteAllText(@"p:\temp\180709.010331.perception.json", jshot.ToString());
        }
    }
    public partial class Shot
    {
        public readonly string shotName;
        public readonly Area[] areas;
    }
    public partial class Area
    {
        public readonly int x;
        public readonly int y;
        public readonly int width;
        public readonly int height;
        public readonly string name;
        public readonly string value;
    }
}
