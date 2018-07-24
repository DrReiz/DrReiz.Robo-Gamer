using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer
{
    public class AdbClient:IDisposable
    {
        public static readonly int AdbPort_Default = 5037;
        public AdbClient(string device, int? adbPort = null)
        {
            this.Device = device;
            this.AdbPort = adbPort ?? AdbPort_Default;

            this.TcpClient = new TcpClient("127.0.0.1", AdbPort);
            this.AdbStream = this.TcpClient.GetStream();

            AdbApi.RunCommand(AdbStream, $"host:transport:{Device}");
        }
        public readonly string Device;
        public readonly int AdbPort;

        public readonly TcpClient TcpClient;
        public readonly NetworkStream AdbStream;

        public System.Drawing.Bitmap CaptureScreenshot()
        {
            var bytes = AdbApi.CaptureScreenshot(AdbStream);
            var bitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(new System.IO.MemoryStream(bytes));
            bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
            return bitmap;
        }
        public void Tap(int x, int y)
        {
            AdbApi.Tap(AdbStream, x, y);
        }

        public void Dispose()
        {
            if (AdbStream != null)
                AdbStream.Dispose();
            if (TcpClient != null)
                ((IDisposable)TcpClient).Dispose();
        }
    }
}
