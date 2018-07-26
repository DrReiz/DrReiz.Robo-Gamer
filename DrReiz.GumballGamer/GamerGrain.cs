using System;
using System.Threading.Tasks;
using DrReiz.GumballGamer.Messages;


namespace DrReiz.GumballGamer
{
    public class GamerGrain : Orleans.Grain, IGamer
    {
        public async Task<string> CaptureScreenshot(string game)
        {
            var context = GameContext.Get(game);

            using (var client = new AdbClient("emulator-5554"))
            {
                var bitmap = client.CaptureScreenshot();
                return ImageStorage.Save(context, bitmap);
                //using (var stream = new System.IO.MemoryStream())
                //{
                //    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                //    var bytes = stream.ToArray();

                //    var name = $"{DateTime.UtcNow:yyMMdd.HHmmss}";

                //    System.IO.File.WriteAllBytes(System.IO.Path.Combine(context.StorageDir, $"{name}.png"), bytes);

                //    return name;
                //}
            }
        }
        public async Task Tap(string game, int x, int y)
        {
            using (var client = new AdbClient("emulator-5554"))
            {
                client.Tap(x, y);
            }
        }
    }
}
