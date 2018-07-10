using ShaniSoft.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
    }
}
