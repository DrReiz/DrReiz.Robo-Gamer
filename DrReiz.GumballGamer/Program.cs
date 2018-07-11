using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.Phash;
using NitroBolt.Functional;
using Newtonsoft.Json.Linq;

namespace DrReiz.GumballGamer
{
    class Program
    {
        //C:\Users\Serge\AppData\Local\Android\sdk\platform-tools >  adb start-server
        //adb devices -- посмотреть список устройств
        //adb kill-server -- с последующим start-server, помогает увидеть blue-stacks
        //
        //посмотреть текущее разрешение
        //adb shell dumpsys displays
        //https://stackoverflow.com/questions/7527459/android-device-screen-size
        static void Main(string[] args)
        {
            GOcr.GocrToPerception();
            //GOcr.Execute();
            return;
            Task.Run(OrleansServer.Execute).Wait();
            return;
            Perception();
            return;
            MonitorScreenshotsByAdb();
            return;
            var screenshot1 = AdbScreenCapture();
            System.IO.File.WriteAllBytes("screen-capture.png", screenshot1);
            return;
            MonitorScreenshots();
            return;
            PhashExecuting();
            return;
            SaveMazeCellImages();
            return;

            var vm_title = "BlueStacks";

            var handle = DirectGamer.FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            //DirectGamer.PostMessage(new System.Runtime.InteropServices.HandleRef(null, handle), DirectGamer.WM_SIZE, new IntPtr(0), new IntPtr((1200 << 16) + 600));

            DirectGamer.RECT rect;
            DirectGamer.GetWindowRect(handle, out rect);
            Console.WriteLine($"{rect.Right - rect.Left} {rect.Bottom - rect.Top}");
            var screenshot = DirectGamer.GetScreenImage(new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));
            screenshot.Save("gumballs.screenshot.png");

            var screenWidth = 900;
            var screenHeight = 1600;
            var headerHeight = 40;
            var footerHeight = 40;

            var cellWidth = 167;
            //var cellHeight = 180;
            var cellHeight = 156;
            var mazeX = 30;
            var mazeY = 277;

            if (true)
            {
                var dx = screenshot.Width / (float)screenWidth;
                var dy = (screenshot.Height - headerHeight - footerHeight) / (float)screenHeight;

                var image = new Bitmap(screenshot);
                using (var g = Graphics.FromImage(image))
                {
                    var pen = new Pen(Color.Red);

                    Action<Pen, float, float, float, float> drawLine = (p, x1, y1, x2, y2) => g.DrawLine(p, x1 * dx, y1 * dy + headerHeight, x2 * dx, y2 * dy + headerHeight);

                    for (var y = 0; y <= 6; ++y)
                        drawLine(pen, mazeX, mazeY + y * cellHeight, mazeX + cellWidth * 5, mazeY + y * cellHeight);
                    for (var x = 0; x <= 5; ++x)
                        drawLine(pen, mazeX + x * cellWidth, mazeY, mazeX + x * cellWidth, mazeY + 6 * cellHeight);
                }
                image.Save("gumballs.lines.png");
            }


            return;

            //DirectGamer.PostMessage(new System.Runtime.InteropServices.HandleRef(null, handle), DirectGamer.WM_LBUTTONDOWN, new IntPtr(DirectGamer.MK_LBUTTON), new IntPtr((100 << 16) + 100));
            if (true)
            {
                // 2 4 ?? дверь, не сработала
                var x = 0;
                var y = 4;
                Shell($"input tap {mazeX + cellWidth / 2 + x * cellWidth} {mazeY + cellHeight/2 + y * cellHeight}"); //maze cell 1 2
            }
            //screenshot: cell 1 2: 110 322 - 206 426
            //Shell("input tap 193 620"); //maze cell 0 2

            //Shell($"input tap 365 {787 + cellHeight}"); //maze cell 1 2
            //Shell("input tap 193 599"); //maze cell 2 2

            //Shell("input tap 194 600"); //maze cell 1 2
            //Shell("input tap 364 786"); //maze cell 1 2

            //Shell("input tap 365 787"); //maze cell 2 3
            return;
            //Shell($"input tap 10 800"); //maze рамка
            //Shell($"input tap 20 850"); //maze рамка
            //Shell($"input tap 25 850"); //maze cell 0 3
            //Shell("input tap 20 50"); //левый верхний угол
            //Shell("input tap 100 300"); //maze cell 0 0
            //Shell($"input tap 170 620"); //maze cell 0 2
            //Shell($"input tap 300 800"); //maze cell 1 3
            //Shell("input tap 500 300"); //maze cell 2 0
            //Shell("input tap 700 300"); //maze cell 3 0
            //Shell("input tap 650 300"); //maze cell 3 0
            //Shell("input tap 600 300"); //maze cell 3 0

            //Shell("input tap 750 300"); //maze cell 4 0
            Shell("input tap 800 300"); //maze cell 4 0
            //Shell("input tap 900 300"); //вне экрана

            //Shell("input tap 800 300"); //maze cell 3 0
            //Shell("input tap 00 300"); //maze cell 3 0
            return;
            return;
            Shell("input keyevent 4");//back кнопка
            Shell("input keyevent 4");
        }

        static void Perception()
        {
            var perceptionJson = JObject.Parse(System.IO.File.ReadAllText(@"p:\Projects\DrReiz.Robo-Gamer\DrReiz.GumballGamer.Wui\wwwroot\data\perception.json"));
            var shots = perceptionJson["perceptionShots"] as JArray;

            //foreach (JObject shot in shots)
            //{
            //    var shotName = shot.Value<string>("shotName");
            //    foreach (JObject jarea in shot.Value<JArray>("areas"))
            //    {
            //        Console.WriteLine($"{jarea.Value<string>("name")}: {jarea.Value<string>("value")}");
            //    }
            //}
            var areas = shots.SelectMany(shot =>
            {
                var shotName = shot.Value<string>("shotName");
                return shot.Value<JArray>("areas").OfType<JObject>()
                  .Select(jarea => new { name = jarea.Value<string>("name"), value = jarea.Value<string>("value"), shot = shotName });
            })
            .GroupBy(pair => new { pair.name, pair.value })
            .ToDictionary(group => group.Key, group => group.Select(pair => pair.shot).Distinct().ToArray());

            foreach (var area in areas)
                Console.WriteLine($"{area.Key.name}--{area.Key.value}: {area.Value.JoinToString(", ")}");

            var areaShotNames = shots.Select(shot => shot.Value<string>("shotName"))
                .ToDictionary(name => name, name => name);


            var screenshots = System.IO.Directory.GetFiles(@"t:\Data\Gumball\Screenshots")
            .Select(screenshot => new { filename = screenshot, name = System.IO.Path.GetFileNameWithoutExtension(screenshot) })
            .ToArray();

            var moderateScreenshots = screenshots.Where(shot => areaShotNames.ContainsKey(shot.name)).ToArray();


            var rnd = new Random();

            string toFullname(string shotName) => @"p:\Projects\DrReiz.Robo-Gamer\DrReiz.GumballGamer.Wui\wwwroot\data\vision\" + shotName + ".png";

            foreach (JObject shot in shots)
            {
                var shotName = shot.Value<string>("shotName");
                foreach (JObject jarea in shot.Value<JArray>("areas"))
                {
                    var area = new
                    {
                        name = jarea.Value<string>("name"),
                        value = jarea.Value<string>("value"),
                        x = jarea.Value<int>("x"), y = jarea.Value<int>("y"), width = jarea.Value<int>("width"), height = jarea.Value<int>("height")
                    };
                    Console.WriteLine($"{area.name}: {area.value}");

                    var rect = new Rectangle(area.x, area.y, area.width, area.height);

                    //var Screenshots = screenshots.Where(screenshot)


                    //var areaBitmap = bitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    //areaBitmap.Save("area.png");


                    var rs = Enumerable.Range(0, 10).Select(i => rnd.Next(rect.Width * rect.Height * 3)).ToArray();

                    var groups = moderateScreenshots.Select(screenshot => new { screenshot, stamp = Pick(screenshot.filename, rect, rs) })
                        .GroupBy(pair => areas.Find(new { name = area.name, value = area.value }).OrEmpty().Contains(pair.screenshot.name));

                    var etalons = groups.FirstOrDefault(group => group.Key == true).OrEmpty();
                    var etalon = etalons.FirstOrDefault();

                    var others = groups.FirstOrDefault(group => group.Key == false).OrEmpty();

                    var etalonRate = etalons.Select(pair => pair.stamp.Zip(etalon.stamp, (e, p) => Math.Abs(e - p)).Max()).Max();
                    var otherRates = others.Select(pair => pair.stamp.Zip(etalon.stamp, (e, p) => Math.Abs(e - p)).Max()).ToArray();
                    var otherRate = otherRates.Any() ? otherRates.Min() : -1;

                    Console.WriteLine($"{etalonRate} {otherRate}");

                    //var etalon = Pick(@"p:\Projects\DrReiz.Robo-Gamer\DrReiz.GumballGamer.Wui\wwwroot\data\vision\" + shotName + ".png", rect, rs);
                    //foreach (var imageFilename in System.IO.Directory.GetFiles(@"t:\Data\Gumball\Screenshots"))
                    //{
                    //    var pick = Pick(imageFilename, rect, rs);
                    //    var difs = etalon.Zip(pick, (e, p) => Math.Abs(e - p));
                    //    var max = difs.Max();
                    //    Console.WriteLine($"{max,3}: {difs.Select(d => d.ToString()).JoinToString(" ")}");
                    //}
                }
            }

        }
        static int[] Pick(string imageFilename, Rectangle rect, int[] rs)
        {
            //var image = System.IO.File.ReadAllBytes(@"p:\Projects\DrReiz.Robo-Gamer\DrReiz.GumballGamer.Wui\wwwroot\data\171213.233315.png");
            var image = System.IO.File.ReadAllBytes(imageFilename);
            var bitmap = (Bitmap)System.Drawing.Bitmap.FromStream(new System.IO.MemoryStream(image));

            var bitmapData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                return rs.Select(r =>
                {
                    var y = r / 3 / rect.Width;
                    var x = r / 3 - y * rect.Width;
                    var plane = r % 3;

                    var p = bitmapData.Scan0 + bitmapData.Stride * y + 4 * x + plane;
                    var b = System.Runtime.InteropServices.Marshal.ReadByte(p);
                    return (int)b;
                })
                .ToArray();
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

        }


        //https://blog.shvetsov.com/2013/02/grab-android-screenshot-to-computer-via.html
        static byte[] AdbScreenCapture()
        {
            var command = "shell:screencap -p";

            var port = 5037;
            //var port = 5555;
            using (var client = new TcpClient("127.0.0.1", port))
            {
                using (var stream = client.GetStream())
                {
                    SendToAdb(stream, "host:transport:emulator-5554", isPayload: false);
                    //SendToAdb(stream, $"shell:{command}", isPayload: true);

                    var fullCommand = $"{command.Length:x4}{command}";
                    Console.WriteLine(fullCommand);
                    var buf = Encoding.UTF8.GetBytes(fullCommand);
                    stream.Write(buf, 0, buf.Length);
                    var okay = ReceiveFromAdb(stream, length: 4);
                    Console.WriteLine($">>{okay.Length} {okay}");
                    if (okay.Substring(0, 4) != "OKAY")
                        throw new Exception($"error adb receive: {okay}");

                    return AdbApi.ReplaceDAToA(AdbApi.ReadAll(stream));
                    //var prefix = ReceiveFromAdb(stream, 4);
                    //if (prefix.Length == 0)
                    //    return null;
                    //Console.WriteLine($">>{prefix.Length} {prefix}");
                    //var length = Convert.ToInt32(prefix);

                    //var answerBuf = new byte[length];
                    //int len = stream.Read(answerBuf, 0, answerBuf.Length);
                    //Console.WriteLine(len);
                    
                    //return answerBuf;
                }
            }

        }

        static void Shell(string command)
        {
            var port = 5037;
            //var port = 5555;
            using (var client = new TcpClient("127.0.0.1", port))
            {
                //client.Connect()
                using (var stream = client.GetStream())
                {
                    //SendToAdb(stream, "127.0.0.1:5556:get-product");
                    //SendToAdb(stream, "host:transport:127.0.0.1:5555", isPayload: false);
                    SendToAdb(stream, "host:transport:emulator-5554", isPayload: false);
                    //SendToAdb(stream, "shell:");
                    //SendToAdbShell(stream, "input keyevent 4\r");
                    //SendToAdbShell(stream, "input keyevent 4\r");
                    //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.1));
                    SendToAdb(stream, $"shell:{command}", isPayload: true);
                    //SendToAdb(stream, "shell:input keyevent 4", isPayload: true);
                    return;
                    SendToAdb(stream, "host:devices");
                    SendToAdb(stream, "host:version");
                    //SendToAdb(stream, "host:devices");

                    return;
                    //var command = "input keyevent 4\n";
                    //var buf = Encoding.UTF8.GetBytes(command);
                    //stream.Write(buf, 0, buf.Length);
                }
            }
        }
        static string SendToAdb(NetworkStream stream, string command, bool? isPayload = null)
        {
            var fullCommand = $"{command.Length:x4}{command}";
            Console.WriteLine(fullCommand);
            var buf = Encoding.UTF8.GetBytes(fullCommand);
            stream.Write(buf, 0, buf.Length);
            var okay = ReceiveFromAdb(stream, length: 4);
            Console.WriteLine($">>{okay.Length} {okay}");
            if (okay.Substring(0, 4) != "OKAY")
                throw new Exception($"error adb receive: {okay}");
            if (isPayload == false)
                return null;
            if (isPayload == true)
            {
                var payload = ReceiveFromAdbPacket(stream);
                Console.WriteLine(payload);
                return payload;
            }
            var answer = ReceiveFromAdb(stream);
            Console.WriteLine(answer);
            return answer;
        }
        static string SendToAdbShell(NetworkStream stream, string command)
        {
            Console.WriteLine($">> {command}");
            var buf = Encoding.UTF8.GetBytes(command);
            stream.Write(buf, 0, buf.Length);
            var res = ReceiveFromAdb(stream);
            Console.WriteLine($"<< {res}");
            return res;
        }

        static string ReceiveFromAdb(NetworkStream stream, int? length = null)
        {
            var answerBuf = new byte[length ?? 1000];
            int len = stream.Read(answerBuf, 0, answerBuf.Length);
            return Encoding.UTF8.GetString(answerBuf, 0, len);
        }
        static string ReceiveFromAdbPacket(NetworkStream stream)
        {
            var prefix = ReceiveFromAdb(stream, 4);
            if (prefix.Length == 0)
                return null;
            Console.WriteLine($">>{prefix.Length} {prefix}");
            var len = Convert.ToInt32(prefix);
            return ReceiveFromAdb(stream, len);
        }

        static void MonitorScreenshotsByAdb()
        {
            var outDir = @"t:\Data\Gumball\Screenshots";
            var device = "emulator-5554";

            //using (var client = new AdbClient("emulator-5554"))
            if (true)
            {
                var hashes = new Dictionary<Shipwreck.Phash.Digest, int>(new DigestComparer());
                Shipwreck.Phash.Digest prevHash = null;

                for (var i = 0; ; i++)
                {

                    var screenshot = AdbScreenCapture(device);

                    var hash = ComputeDigest(screenshot);
                    //var isOther = prevHash == null || Shipwreck.Phash.ImagePhash.GetCrossCorrelation(prevHash, hash) < 1;
                    var isOther = prevHash == null || hashes.Keys.All(_hash => ImagePhash.GetCrossCorrelation(_hash, hash) < 1);
                    if (isOther)
                        screenshot.Save(System.IO.Path.Combine(outDir, $"{DateTime.UtcNow:yyMMdd.HHmmss}.png"));
                    //System.IO.File.WriteAllBytes(System.IO.Path.Combine(outDir, $"{DateTime.UtcNow:yyMMdd.HHmmss}.png"), stream.ToArray());
                    Console.WriteLine($"{i:d4} {hash} - {(isOther ? "saved" : "skipped")}");
                    prevHash = hash;
                    hashes[hash] = (hashes.ContainsKey(hash) ? hashes[hash] : 0) + 1;

                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.1));
                }

            }
        }
        static Bitmap AdbScreenCapture(string device)
        {
            using (var client = new AdbClient("emulator-5554"))
            {
                return client.CaptureScreenshot();
            }
        }

        static void MonitorScreenshots()
        {
            var outDir = @"t:\Data\Gumball\Screenshots";


            var vm_title = "BlueStacks";

            var handle = DirectGamer.FindWindow(null, vm_title);
            if (handle == IntPtr.Zero)
                throw new Exception("Окно не найдено");

            DirectGamer.RECT rect;
            DirectGamer.GetWindowRect(handle, out rect);
            Console.WriteLine($"{rect.Right - rect.Left} {rect.Bottom - rect.Top}");

            var hashes = new Dictionary<Shipwreck.Phash.Digest, int>(new DigestComparer());
            Shipwreck.Phash.Digest prevHash = null;

            for (var i = 0; ;i++ )
            {

                var screenshot = DirectGamer.GetScreenImage(new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));

                var hash = ComputeDigest(screenshot);
                //var isOther = prevHash == null || Shipwreck.Phash.ImagePhash.GetCrossCorrelation(prevHash, hash) < 1;
                var isOther = prevHash == null || hashes.Keys.All(_hash => ImagePhash.GetCrossCorrelation(_hash, hash) < 1);
                if (isOther)
                    screenshot.Save(System.IO.Path.Combine(outDir, $"{DateTime.UtcNow:yyMMdd.HHmmss}.png"));
                    //System.IO.File.WriteAllBytes(System.IO.Path.Combine(outDir, $"{DateTime.UtcNow:yyMMdd.HHmmss}.png"), stream.ToArray());
                Console.WriteLine($"{i:d4} {hash} - {(isOther ? "saved" : "skipped")}");
                prevHash = hash;
                hashes[hash] = (hashes.ContainsKey(hash) ? hashes[hash] : 0) + 1;

                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.1));
            }

        }

        private static Digest ComputeDigest(Bitmap screenshot)
        {
            var stream = new System.IO.MemoryStream();
            screenshot.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            return Shipwreck.Phash.ImagePhash.ComputeDigest(stream);
        }

        class DigestComparer : IEqualityComparer<Shipwreck.Phash.Digest>
        {
            public bool Equals(Digest x, Digest y)
            {
                return x.Coefficents.SequenceEqual(y.Coefficents);
            }

            public int GetHashCode(Digest obj)
            {
                return obj.Coefficents.Aggregate(0, (a, b) => a ^ b);
            }
        }

        static void SaveMazeCellImages()
        {
            var screenshot = Image.FromFile("gumballs.screenshot.png");

            var screenWidth = 900;
            var screenHeight = 1600;
            var headerHeight = 40;
            var footerHeight = 40;

            var cellWidth = 167;
            //var cellHeight = 180;
            var cellHeight = 156;
            var mazeX = 30;
            var mazeY = 277;

            if (true)
            {
                var dx = screenshot.Width / (float)screenWidth;
                var dy = (screenshot.Height - headerHeight - footerHeight) / (float)screenHeight;

                var image = new Bitmap(screenshot);
                using (var g = Graphics.FromImage(image))
                {
                    for (var yi = 0; yi < 6; ++yi)
                    {
                        for (var xi = 0; xi < 5; ++xi)
                        {
                            var x1 = mazeX + xi * cellWidth;
                            var y1 = mazeY + yi * cellHeight;
                            var x2 = x1 + cellWidth;
                            var y2 = y1 + cellHeight;
                            var sx1 = x1 * dx;
                            var sx2 = x2 * dx;
                            var sy1 = (y1 * dy + headerHeight);
                            var sy2 = y2 * dy + headerHeight;
                            if (sx2 >= image.Width)
                                sx2 = image.Width;
                            if (sy2 >= image.Height)
                                sy2 = image.Height;

                            image.Clone(new RectangleF(sx1, sy1, sx2 - sx1, sy2 - sy1), image.PixelFormat).Save($"cells/{xi}{yi}.png");
                        }
                    }

                    //var pen = new Pen(Color.Red);

                    //Action<Pen, float, float, float, float> drawLine = (p, x1, y1, x2, y2) => g.DrawLine(p, x1 * dx, y1 * dy + headerHeight, x2 * dx, y2 * dy + headerHeight);

                    //for (var y = 0; y <= 6; ++y)
                    //    drawLine(pen, mazeX, mazeY + y * cellHeight, mazeX + cellWidth * 5, mazeY + y * cellHeight);
                    //for (var x = 0; x <= 5; ++x)
                    //    drawLine(pen, mazeX + x * cellWidth, mazeY, mazeX + x * cellWidth, mazeY + 6 * cellHeight);
                }
                image.Save("gumballs.lines.png");
            }
        }
        static void PhashExecuting()
        {
            var filenames = System.IO.Directory.GetFiles("Cells");

            var hashes = filenames.Select(filename => new { filename, hash = Shipwreck.Phash.ImagePhash.ComputeDigest(filename) }).ToArray();
            foreach (var hash in hashes)
            {
                Console.WriteLine(hash.filename);
                foreach (var correlation in hashes.Select(otherHash => new { otherHash.filename, D = Shipwreck.Phash.ImagePhash.GetCrossCorrelation(hash.hash, otherHash.hash) })
                    .OrderByDescending(_cor => _cor.D)
                    .Take(5))
                {
                    Console.WriteLine($"  {correlation.D:f2} {correlation.filename}");
                }
            }
        }
    }
}
