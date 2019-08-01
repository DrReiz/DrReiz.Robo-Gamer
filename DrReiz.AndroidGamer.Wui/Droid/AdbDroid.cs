using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace DrReiz.AndroidGamer.Wui
{
    public class AdbDroid
    {
        public static string CaptureScreenshot(string game)
        {
            var context = GameContext.Get(game);

            using (var client = new AdbClient("emulator-5554"))
            {
                var bitmap = client.CaptureScreenshot();
                return ImageStorage.Save(context, bitmap);
            }
        }
        public static void Tap(string game, int x, int y)
        {
            using (var client = new AdbClient("emulator-5554"))
            {
                client.Tap(x, y);
            }
        }

    }
}
