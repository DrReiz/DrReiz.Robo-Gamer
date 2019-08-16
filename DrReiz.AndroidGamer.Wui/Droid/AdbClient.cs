using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.AndroidGamer.Wui
{
    //C:\Users\Serge\AppData\Local\Android\sdk\platform-tools >  adb start-server
    //adb devices -- посмотреть список устройств
    //adb kill-server -- с последующим start-server, помогает увидеть blue-stacks
    //
    //посмотреть текущее разрешение
    //adb shell dumpsys displays
    //https://stackoverflow.com/questions/7527459/android-device-screen-size
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
            var watch = new Stopwatch();
            watch.Start();

            var bytes = AdbApi.CaptureScreenshot(AdbStream);

            //System.IO.File.WriteAllBytes(@"p:\temp\q1.png", bytes);

            var captureTime = watch.Elapsed;
            watch.Reset();
            //Log.Information("Capture {captureTime}", captureTime);

            watch.Start();

            var bitmap = LoadRawBitmap(bytes);

            //var bitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(new System.IO.MemoryStream(bytes));
            bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);

            var rotateTime = watch.Elapsed;

            Log.Information("Capture {captureTime}, {rotateTime}", captureTime, rotateTime);

            return bitmap;

        }
        unsafe static System.Drawing.Bitmap LoadRawBitmap(byte[] data)
        {
            var width = BitConverter.ToInt32(data, 0);
            var height = BitConverter.ToInt32(data, 4);
            //Log.Information("Image: {width}, {height}, {bytes}", width, height, data.Length);
            fixed (byte * p = data)
            {
                var image = p + 12;
                for (var i = 0; i < data.Length - 12; i += 4)
                {
                    var b = image[i];
                    image[i] = image[i+2];
                    image[i + 2] = b;
                }
                var bitmap =  new System.Drawing.Bitmap(width, height, 4*width, System.Drawing.Imaging.PixelFormat.Format32bppRgb, new IntPtr(image));
                return new System.Drawing.Bitmap(bitmap);
            }

        }
        public void Tap(int x, int y)
        {
            AdbApi.Tap(AdbStream, x, y);
        }
        public void Back()
        {
            AdbApi.Back(AdbStream);
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
