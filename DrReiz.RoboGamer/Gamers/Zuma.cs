using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using Point = System.Drawing.Point;

namespace DrReiz.RoboGamer
{
    public class Zuma : DirectGamer
    {
        public static readonly string VmIp = "169.254.200.106";

        //public static readonly System.Drawing.Rectangle VmRect = new System.Drawing.Rectangle();

        static readonly string[] hsv_names = new[] { "h", "s", "v" };
        public static void Execute()
        {
            var frogRect = new Rectangle(320, 218, 150, 150);

            if (Other(frogRect))
                return;

            var screenChecks = ScreenChecks();
            screenChecks = ResolveChecks(screenChecks, @"..\..\Zuma.Data");

            Func<Bitmap, string> check = image => screenChecks.Where(_check => image.Check(_check.Points)).Select(_check => _check.Name).FirstOrDefault();


            var vmClient = new VmClient();

            var gameScreenRect = vmClient.GameScreenRect;

            var game_width = gameScreenRect.Width;
            var game_height = gameScreenRect.Height;


            //var startButtonPoint = new Point(950, 430);
            //var startMissionPoint = new Point(600, 750);
            var startButtonPoint = new Point(950, 520);
            var startMissionPoint = new Point(180, 360);
            var endChallengePoint = new Point(320, 590);

            var vm_host = VmIp;

            var client = new MouseClient(vm_host);


            var background = LoadBitmap("../../Zuma.Data/background.png");
            var hsv_background = new Mat(background.ToIplImage(), true).CvtColor(ColorConversion.RgbToHsv).Split();


            if (false)
            {
                var fireflyImage = LoadBitmap(@"p:\Projects\_Other\Cs10\Gamering\bin\Release\FireFlies.h\46.png");
                var hsv_fireflyImage = new Mat(fireflyImage.ToIplImage(), true).CvtColor(ColorConversion.RgbToHsv).Split();
                //for (var i = 0; i < 3; ++i)
                //{
                //  hsv_fireflyImage[i].Absdiff(hsv_background[i]).ImWrite(string.Format("ff{0}.png", i));
                //}

                var hsv_v = hsv_fireflyImage[2].Absdiff(hsv_background[2]);
                hsv_v.Rectangle(new Rect(frogRect.X, frogRect.Y, frogRect.Width, frogRect.Height), new Scalar(0), -1);
                hsv_v = hsv_v.Threshold(80, 255, ThresholdType.Binary);
                hsv_v.ImWrite("ff_v.png");
                return;
            }


            foreach (var file in System.IO.Directory.GetFiles("History"))
                System.IO.File.Delete(file);

            string prevScreenName = null;
            string prevKnownScreenName = null;
            var history = new List<Bitmap>();
            var isSave = false;

            var lastActionTime = DateTime.UtcNow;

            for (var tick = 0; ; tick++)
            {
                try
                {
                    var bmp = GetScreenImage(gameScreenRect);

                    if (Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Escape)
                            break;
                        else if (keyInfo.Key == ConsoleKey.Spacebar)
                        {
                            bmp.Save("0.png");
                            for (var i = 0; i < history.Count; ++i)
                                history[i].Save(string.Format("{0}.png", i + 1));
                            Console.WriteLine("save");
                        }
                    }

                    var screenName = check(bmp);
                    //var screenName = "action";
                    Console.Write("{0}: {1}{2}", tick, screenName, new string(' ', 20) + new string('\x8', 40));
                    switch (screenName)
                    {
                        case "main":
                            Console.WriteLine("main");
                            client.MouseEvent(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, startButtonPoint.X * 65536 / game_width, startButtonPoint.Y * 65536 / game_height);
                            System.Threading.Thread.Sleep(400);
                            client.MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0);
                            System.Threading.Thread.Sleep(150);
                            client.MouseEvent(MouseEventFlags.LEFTUP, 0, 0);
                            System.Threading.Thread.Sleep(50);
                            System.Threading.Thread.Sleep(4000);
                            break;
                        case "challenge":
                            client.MouseEvent(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, startMissionPoint.X * 65536 / game_width, startMissionPoint.Y * 65536 / game_height);
                            System.Threading.Thread.Sleep(400);
                            client.MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0);
                            System.Threading.Thread.Sleep(150);
                            client.MouseEvent(MouseEventFlags.LEFTUP, 0, 0);
                            System.Threading.Thread.Sleep(50);
                            break;
                        case "end_challenge":
                            client.MouseEvent(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, endChallengePoint.X * 65536 / game_width, endChallengePoint.Y * 65536 / game_height);
                            System.Threading.Thread.Sleep(400);
                            client.MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0);
                            System.Threading.Thread.Sleep(150);
                            client.MouseEvent(MouseEventFlags.LEFTUP, 0, 0);
                            System.Threading.Thread.Sleep(50);
                            break;
                        //case "action":
                        //  mouse_event(MouseEventFlags.LEFTDOWN, 0, 0);
                        //  System.Threading.Thread.Sleep(150);
                        //  mouse_event(MouseEventFlags.LEFTUP, 0, 0);
                        //  System.Threading.Thread.Sleep(50);

                        //  break;
                        case "action":
                            //if (prevScreenName == "mission" && screenName == "action")
                            //{
                            //  for (var i = 0; i < 100; ++i)
                            //  {
                            //    GetScreenImage(gameScreenRect).Save(string.Format("FireFlies/{0}.png", i));
                            //    System.Threading.Thread.Sleep(1);
                            //  }
                            //}

                            if (!isSave)
                                bmp.Save("b.png");
                            isSave = true;
                            client.MouseEvent(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, (frogRect.X + frogRect.Width / 2) * 65536 / game_width, 580 * 65536 / game_height);
                            System.Threading.Thread.Sleep(150);
                            if (lastActionTime.AddSeconds(5) < DateTime.UtcNow)
                            {
                                client.MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0);
                                System.Threading.Thread.Sleep(100);
                                client.MouseEvent(MouseEventFlags.LEFTUP, 0, 0);
                                System.Threading.Thread.Sleep(10);
                                lastActionTime = DateTime.UtcNow;
                                continue;
                            }

                            var ballPoint = new Point(75, 105);
                            var leftEyePoint = new Point(50, 57);
                            var rightEyePoint = new Point(100, 57);
                            var frogCenter = new Point(frogRect.X + 75, frogRect.Y + 75);

                            var frogBmp = GetScreenImage(new Rectangle(gameScreenRect.X + frogRect.X, gameScreenRect.Y + frogRect.Y, frogRect.Width, frogRect.Height));
                            var frogChannels = new Mat(frogBmp.ToIplImage(), true).CvtColor(ColorConversion.RgbToHsv).Split();
                            int ballColor = AverageColor(frogChannels[0], ballPoint, 2);
                            var leftEyeColor = AverageColor(frogChannels[1], leftEyePoint, 2);
                            var rightEyeColor = AverageColor(frogChannels[1], rightEyePoint, 2);
                            //Console.WriteLine("{0}, {1}, {2}", ballColor, leftEyeColor, rightEyeColor);

                            if (leftEyeColor != 25 || rightEyeColor != 25)
                                continue;
                            bmp = GetScreenImage(gameScreenRect);

                            var frogBallPoint = new Point(frogRect.X + ballPoint.X, frogRect.Y + ballPoint.Y);

                            var mat = new Mat(bmp.ToIplImage(), true);
                            var hsv = mat.CvtColor(ColorConversion.RgbToHsv).Split();
                            var hsv_diff = hsv.Zip(hsv_background, (ch, bg_ch) => ch.Absdiff(bg_ch)).ToArray();
                            var s_diff_th = hsv_diff[1].Threshold(25, 255, ThresholdType.Binary);
                            s_diff_th.Rectangle(new Rect(frogRect.X, frogRect.Y, frogRect.Width, frogRect.Height), new Scalar(0), -1);

                            var balls = DetectBalls(s_diff_th, hsv[0]);
                            var minBall = balls.Where(ball => Math.Abs(ball.H - ballColor) < 10).OrderBy(ball => Distance2(ball.Point, frogCenter)).FirstOrDefault();
                            if (minBall != null)
                            {
                                var targetPoint = RotatePointAroundCenter(minBall.Point, frogCenter, -8);

                                client.MouseEvent(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, targetPoint.X * 65536 / game_width, targetPoint.Y * 65536 / game_height);
                                System.Threading.Thread.Sleep(150);

                                client.MouseEvent(MouseEventFlags.LEFTDOWN, 0, 0);
                                System.Threading.Thread.Sleep(100);
                                client.MouseEvent(MouseEventFlags.LEFTUP, 0, 0);
                                System.Threading.Thread.Sleep(10);
                                lastActionTime = DateTime.UtcNow;
                            }
                            if (true)
                            {
                                var ballwidth = 36;
                                var ballwidth3 = ballwidth / 3;

                                var displayBmp = new Bitmap(bmp);
                                using (var g = Graphics.FromImage(displayBmp))
                                {
                                    g.DrawImageUnscaled(frogBmp, new Point(frogRect.X, frogRect.Y));
                                    //g.DrawImageUnscaled(frogBmp, 0, 0);
                                    foreach (var ball in balls)
                                    {
                                        g.DrawEllipse(Pens.Orange, ball.Point.X - ballwidth / 2, ball.Point.Y - ballwidth / 2, ballwidth, ballwidth);
                                        g.DrawString(ball.H.ToString(), SystemFonts.DefaultFont, Brushes.Black, ball.Point.X - ballwidth / 2, ball.Point.Y - ballwidth / 2);
                                    }
                                    if (true)
                                    {
                                        g.DrawEllipse(Pens.Red, frogBallPoint.X - ballwidth / 2, frogBallPoint.Y - ballwidth / 2, ballwidth, ballwidth);
                                        g.DrawString(ballColor.ToString(), SystemFonts.DefaultFont, Brushes.Black, frogBallPoint.X - ballwidth / 2, frogBallPoint.Y - ballwidth / 2);
                                    }
                                    if (minBall != null)
                                    {
                                        g.DrawEllipse(new Pen(Color.Green, 2), minBall.Point.X - ballwidth / 2, minBall.Point.Y - ballwidth / 2, ballwidth, ballwidth);
                                        var targetPoint = RotatePointAroundCenter(minBall.Point, frogCenter, -8);
                                        g.DrawEllipse(new Pen(Color.LightBlue, 2), targetPoint.X - ballwidth / 2, targetPoint.Y - ballwidth / 2, ballwidth, ballwidth);
                                    }

                                }
                                var prefix = string.Format("History/{0}.", tick);
                                displayBmp.Save(prefix + "detect.png");
                                //hsv_diff[0].ImWrite(prefix + "detect.h.png");
                                //s_diff_th.ImWrite(prefix + "detect.s diff.png");
                                //frogChannels[0].ImWrite(prefix + "frog h.png");
                                //frogChannels[1].ImWrite(prefix + "frog s.png");
                                //frogChannels[2].ImWrite(prefix + "frog v.png");
                            }


                            break;
                        case null:
                            foreach (var _check in screenChecks)
                            {
                                Console.WriteLine(_check.Name);
                                bmp.Display(_check.Points);
                            }
                            bmp.Save("unknown.png");
                            break;
                    }
                    prevScreenName = screenName;
                    history.Insert(0, bmp);
                    const int maxHistoryLength = 10;
                    if (history.Count > maxHistoryLength)
                        history.RemoveRange(maxHistoryLength, history.Count - maxHistoryLength);
                    if (screenName != null)
                        prevKnownScreenName = screenName;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }
        }
        static ScreenCheck[] ResolveChecks(ScreenCheck[] checks, string dir)
        {
            return checks.Select(check => ResolveCheck(check, dir)).ToArray();
        }
        static ScreenCheck ResolveCheck(ScreenCheck check, string dir)
        {
            var path = System.IO.Path.Combine(dir, check.Name + ".png");
            if (!System.IO.File.Exists(path))
                return check;

            var bmp = (Bitmap)Bitmap.FromFile(path);
            return new ScreenCheck(check.Name,
                check.Points
                    .Select(point => new CheckPoint(point.Point.X, point.Point.Y, (uint)bmp.GetPixel(point.Point.X, point.Point.Y).ToArgb()))
                    .ToArray()
             );

        }

        private static ScreenCheck[] ScreenChecks()
        {
            return new[]
                {
                  new ScreenCheck
                  (
                    "main",
                    new[]
                    {
                      new CheckPoint(200, 190, 0xff554a22),
                      new CheckPoint(65, 400, 0xfff44c41)
                    }
                  ),
                  //new
                  //{
                  //  Name = "mission",
                  //  Points = new[]
                  //  {
                  //    new CheckPoint(200, 190, 0xffb5d0c7),
                  //    new CheckPoint(65, 400, 0xffad7630)
                  //  }
                  //},
                  //new
                  //{
                  //  Name = "action",
                  //  Points = new[]
                  //  {
                  //    new CheckPoint(950, 10, 0xff72554b),
                  //    new CheckPoint(10, 10, 0xff462b1d),
                  //  }
                  //},
                  new ScreenCheck
                  (
                    "challenge",
                    new[]
                    {
                      new CheckPoint(261, 75, 0xff814919),
                      new CheckPoint(616, 522, 0xff3e402b)
                    }
                  ),
                  new ScreenCheck
                  (
                    "action",
                    new[]
                    {
                      new CheckPoint(798, 42, 0xff534533),
                      new CheckPoint(181, 45, 0xff34361a),
                    }
                  ),
                  new ScreenCheck
                  (
                    "end_challenge",
                    new[]
                    {
                      new CheckPoint(29, 82, 0xffb5797b),
                      new CheckPoint(739, 525, 0xffce7929),
                    }
                  ),
              };
        }

        static int Distance2(Point p1, Point p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        private static void DetectBallsForArticle()
        {
            var g_src = new Mat("0.png", LoadMode.GrayScale);
            var src = new Mat("0.png");
            var hsv_src = new Mat();
            Cv2.CvtColor(src, hsv_src, ColorConversion.RgbToHsv);

            var background = new Mat("background.png");
            var g_background = new Mat("background.png", LoadMode.GrayScale);
            var hsv_background = new Mat();
            Cv2.CvtColor(background, hsv_background, ColorConversion.RgbToHsv);
            var canny = new Mat();
            var dst2 = new Mat();

            Cv2.Canny(g_src, canny, 50, 200);
            Cv2.Threshold(src, dst2, 50, 200, OpenCvSharp.ThresholdType.Binary);
            //Cv2.Subtract(g_src, g_background, dst2);
            //Cv2.Absdiff(g_src, g_background, dst2);

            //Cv2.Subtract(src, background, dst2);
            Cv2.Absdiff(src, background, dst2);

            //dst2.ImWrite("diff.bmp");

            Mat[] dst2_channels;
            Cv2.Split(dst2, out dst2_channels);

            Mat[] background_channels;
            Cv2.Split(background, out background_channels);

            Mat[] hsv_background_channels;
            Cv2.Split(hsv_background, out hsv_background_channels);

            Mat[] hsv_src_channels;
            Cv2.Split(hsv_src, out hsv_src_channels);

            var div_0 = new Mat();
            //Cv2.Divide(dst2_channels[1], background_channels[1], div_0, scale:50);
            Cv2.Divide(background_channels[1], dst2_channels[1], div_0, scale: 40);

            Mat dst2_01 = new Mat();
            Mat dst2_12 = new Mat();
            Mat dst2_012 = new Mat();

            Cv2.Absdiff(dst2_channels[0], dst2_channels[1], dst2_01);
            Cv2.Absdiff(dst2_channels[1], dst2_channels[2], dst2_12);
            Cv2.Add(dst2_01, dst2_12, dst2_012);

            var hsv_diff = Enumerable.Range(0, 3).Select(i => new Mat()).ToArray();
            for (var i = 0; i < 3; ++i)
                Cv2.Absdiff(hsv_src_channels[i], hsv_background_channels[i], hsv_diff[i]);

            //Cv2.Compare(dst2_channels[2], t_dst, t_dst);

            var dst3 = new Mat();
            Cv2.Threshold(dst2_012, dst3, 60, 255, ThresholdType.Binary);
            //OpenCvSharp.CPlusPlus.Cv2.CvtColor(dst2, dst3, OpenCvSharp.ColorConversion.RgbToGray);

            //var circles = OpenCvSharp.CPlusPlus.Cv2.HoughCircles(dst3, OpenCvSharp.HoughCirclesMethod.Gradient, 1, 10, minRadius:10, maxRadius: 80);
            //foreach (var circle in circles)
            //  Console.WriteLine(circle);

            //Console.WriteLine(hsv_diff[0]);

            //hsv_diff[1].ImWrite("hsv_diff_s.bmp");
            DetectBallView(hsv_diff[1], hsv_diff[0]);
            return;

            //using (new Window("src image", src))
            //using (new Window("dst image", background))
            ////using (new Window("canny", canny))
            //using (new Window("dst2 image", dst2))
            //using (new Window("diff0", dst2_channels[1]))
            //using (new Window("bg0", background_channels[1]))
            //using (new Window("dst3 image", div_0))
            using (new Window("src h", hsv_src_channels[0]))
            using (new Window("bg h", hsv_background_channels[0]))
            using (new Window("d h", hsv_diff[0]))
            using (new Window("src s", hsv_src_channels[1]))
            using (new Window("bg s", hsv_background_channels[1]))
            using (new Window("d s", hsv_diff[1]))
            using (new Window("src v", hsv_src_channels[2]))
            using (new Window("bg v", hsv_background_channels[2]))
            using (new Window("d v", hsv_diff[2]))
            {
                Cv2.WaitKey();
            }
        }

        private static void MakeImagesForArticle()
        {
            var resizeK = 0.2;

            var dir = "Example/";

            var src = new Mat("0.bmp");
            var src_g = new Mat("0.bmp", LoadMode.GrayScale);

            var src_1 = new Mat("1.bmp");
            var src_1_g = new Mat("1.bmp", LoadMode.GrayScale);

            var background = new Mat("background.bmp");
            var background_g = new Mat("background.bmp", LoadMode.GrayScale);

            src.Resize(resizeK).ImWrite(dir + "0.png");
            src_g.Resize(resizeK).ImWrite(dir + "0 g.png");
            src_g.ThresholdStairs().Resize(resizeK).ImWrite(dir + "0 g th.png");

            var canny = new Mat();
            Cv2.Canny(src_g, canny, 50, 200);
            canny.Resize(0.5).ImWrite(dir + "0 canny.png");

            Mat[] src_channels;
            Cv2.Split(src, out src_channels);

            for (var i = 0; i < src_channels.Length; ++i)
            {
                var channels = Enumerable.Range(0, src_channels.Length).Select(j => new Mat(src_channels[0].Rows, src_channels[0].Cols, src_channels[0].Type())).ToArray();
                channels[i] = src_channels[i];
                var dst = new Mat();
                Cv2.Merge(channels, dst);
                dst.Resize(resizeK).ImWrite(dir + string.Format("0 ch{0}.png", i));
                src_channels[i].ThresholdStairs().Resize(resizeK).ImWrite(dir + string.Format("0 ch{0} th.png", i));
            }

            if (true)
            {
                src.Resize(0.4).ImWrite(dir + "0.png");
                src_1.Resize(0.4).ImWrite(dir + "1.png");
                background.Resize(0.4).ImWrite(dir + "bg.png");

                var dst_01 = new Mat();
                Cv2.Absdiff(src, src_1, dst_01);
                dst_01.Resize(resizeK).ImWrite(dir + "01.png");
                dst_01.Cut(new Rect(50, src.Height * 4 / 5 - 50, src.Width / 5, src.Height / 5)).ImWrite(dir + "01 part.png");
                dst_01.Cut(new Rect(50, src.Height * 4 / 5 - 50, src.Width / 5, src.Height / 5)).CvtColor(ColorConversion.RgbToGray).ImWrite(dir + "01 g.png");
                dst_01.CvtColor(ColorConversion.RgbToGray).ThresholdStairs().Resize(resizeK).ImWrite(dir + "01 g th.png");

                var dst_01_g = new Mat();
                Cv2.Absdiff(src_g, src_1_g, dst_01_g);
                dst_01_g.Cut(new Rect(50, src.Height * 4 / 5 - 50, src.Width / 5, src.Height / 5)).ImWrite(dir + "0g1g.png");
                dst_01_g.ThresholdStairs().Resize(resizeK).ImWrite(dir + "0g1g th.png");
            }

            if (true)
            {
                var dst_0b = new Mat();
                Cv2.Absdiff(src, background, dst_0b);
                dst_0b.Resize(0.6).ImWrite(dir + "0b.png");

                var dst_0b_g = new Mat();
                Cv2.Absdiff(src_g, background_g, dst_0b_g);
                dst_0b_g.Resize(0.3).ImWrite(dir + "0b g.png");
                dst_0b_g.ThresholdStairs().Resize(0.3).ImWrite(dir + "0b g th.png");
            }
            if (true)
            {
                var hsv_src = new Mat();
                Cv2.CvtColor(src, hsv_src, ColorConversion.RgbToHsv);


                var hsv_background = new Mat();
                Cv2.CvtColor(background, hsv_background, ColorConversion.RgbToHsv);

                var hsv_background_channels = hsv_background.Split();

                var hsv_src_channels = hsv_src.Split();

                if (true)
                {
                    var all = new Mat(src.ToIplImage(), true);
                    for (var i = 0; i < hsv_src_channels.Length; ++i)
                    {
                        hsv_src_channels[i].CvtColor(ColorConversion.GrayToRgb).CopyTo(all, new Rect(i * src.Width / 3, src.Height / 2, src.Width / 3, src.Height / 2));
                    }
                    src_g.CvtColor(ColorConversion.GrayToRgb).CopyTo(all, new Rect(src.Width / 2, 0, src.Width / 2, src.Height / 2));
                    all.Resize(0.3).ImWrite(dir + "all.png");
                }

                foreach (var pair in new[] { "h", "s", "v" }.Select((channel, index) => new { channel, index }))
                {
                    var diff = new Mat();
                    Cv2.Absdiff(hsv_src_channels[pair.index], hsv_background_channels[pair.index], diff);
                    diff.Resize(0.3).With_Title(pair.channel).ImWrite(dir + string.Format("0b {0}.png", pair.channel));
                    diff.ThresholdStairs().Resize(0.3).ImWrite(dir + string.Format("0b {0} th.png", pair.channel));

                    hsv_src_channels[pair.index].Resize(resizeK).With_Title(pair.channel).ImWrite(dir + string.Format("0 {0}.png", pair.channel));

                    foreach (var d in new[] { -100, -50, 50, 100 })
                    {
                        var delta = new Mat(hsv_src_channels[pair.index].ToIplImage(), true);
                        delta.Rectangle(new Rect(0, 0, delta.Width, delta.Height), new Scalar(Math.Abs(d)), -1);

                        var new_channel = new Mat();
                        if (d >= 0)
                            Cv2.Add(hsv_src_channels[pair.index], delta, new_channel);
                        else
                            Cv2.Subtract(hsv_src_channels[pair.index], delta, new_channel);

                        //delta.ImWrite(dir + string.Format("d{0}{1}.png", pair.channel, d));
                        //new_channel.ImWrite(dir + string.Format("q{0}{1}.png", pair.channel, d));

                        var new_hsv = new Mat();
                        Cv2.Merge(hsv_src_channels.Select((channel, index) => index == pair.index ? new_channel : channel).ToArray(), new_hsv);

                        var res = new Mat();
                        Cv2.CvtColor(new_hsv, res, ColorConversion.HsvToRgb);
                        res.Resize(resizeK).With_Title(string.Format("{0} {1:+#;-#}", pair.channel, d)).ImWrite(dir + string.Format("0 {0}{1}.png", pair.channel, d));
                    }
                }
                //if (true)
                //{
                //  var mat = new Mat(src.ToIplImage(), true);
                //  mat.CopyTo(
                //}

            }
        }
        static int AverageColor(Mat mat, Point p, int radius)
        {
            var sum = 0;
            var count = 0;
            for (var x = p.X - radius; x < p.X + radius; x++)
                for (var y = p.Y - radius; y < p.Y + radius; y++)
                {
                    sum += mat.Get<byte>(y, x);
                    count++;
                }
            return sum / count;
        }
        static int DispersionColor(Mat mat, Point p, int radius)
        {
            int? min = null;
            int? max = null;
            for (var x = p.X - radius; x < p.X + radius; x++)
                for (var y = p.Y - radius; y < p.Y + radius; y++)
                {
                    var v = mat.Get<byte>(y, x);
                    if (min == null || v < min.Value)
                        min = v;
                    if (max == null || max.Value < v)
                        max = v;
                }
            return max.Value - min.Value;
        }

        static Mat DetectBallView(Mat s, Mat h)
        {
            //Console.WriteLine(mat);
            //Console.WriteLine("{0}:{1}", mat.Width, mat.Height);
            //Console.WriteLine("{0}, {1}, {2}", mat.Step(0), mat.Step(), mat.Step(1));
            //return;
            const int ballWidth = 36;

            var balls = DetectBalls(s, h, ballWidth);


            var detectM = QuickDetectBalls_Field(s.Threshold(20, 255, ThresholdType.Binary), ballWidth);
            if (true)
            {
                var debugMat = new Mat();
                Cv2.CvtColor(s, debugMat, ColorConversion.GrayToRgb);

                const int ballWidth_3 = ballWidth / 3;

                var detectCount = 0;
                var allDetectCount = 0;
                for (var y = 0; y < detectM.GetLength(1); ++y)
                    for (var x = 0; x < detectM.GetLength(0); ++x)
                    {
                        if (detectM[x, y] > 0.8 * ballWidth_3 * ballWidth_3)
                        {
                            debugMat.Circle(x * ballWidth_3 + ballWidth_3 / 2, y * ballWidth_3 + ballWidth_3 / 2, 2, Color.LightGreen.ToScalar(), -1);
                            detectCount++;
                        }
                        allDetectCount++;
                    }

                foreach (var ball in balls)
                {
                    debugMat.Circle(ball.Point.X, ball.Point.Y, ballWidth / 2, Color.Orange.ToScalar(), thickness: 1);
                    debugMat.PutText(ball.H.ToString(), new OpenCvSharp.CPlusPlus.Point(ball.Point.X - ballWidth / 4, ball.Point.Y), FontFace.HersheySimplex, 0.5, Color.Red.ToScalar());
                }

                if (true)
                {
                    var p = new Point(400, 150);
                    var center = new Point(400, 300);

                    var resPoint = RotatePointAroundCenter(p, center, 90);
                    var resPoint2 = RotatePointAroundCenter(p, center, 45);
                    var resPoint3 = RotatePointAroundCenter(p, center, 135);
                    debugMat.Circle(resPoint.X, resPoint.Y, 3, Color.LightBlue.ToScalar(), -1);
                    debugMat.Circle(resPoint2.X, resPoint2.Y, 3, Color.LightBlue.ToScalar(), -1);
                    debugMat.Circle(resPoint3.X, resPoint3.Y, 3, Color.LightBlue.ToScalar(), -1);
                    debugMat.Circle(center.X, center.Y, 3, Color.LightBlue.ToScalar(), -1);
                    debugMat.Circle(p.X, p.Y, 3, Color.LightBlue.ToScalar(), -1);
                }

                Console.WriteLine("detect count: {0}%", 100 * detectCount / allDetectCount);
                debugMat.ImWrite("q.bmp");
                return debugMat;
            }
        }
        static Point RotatePointAroundCenter(Point p1, Point center, double angle)
        {
            var mat = Cv2.GetRotationMatrix2D(new Point2f(center.Y, center.X), angle, 1);
            var v = new Mat(3, 1, MatType.CV_64FC1, new double[] { p1.Y, p1.X, 1 });
            var q = (mat * v).ToMat();
            return new Point((int)q.Get<double>(1, 0), (int)q.Get<double>(0, 0));
        }

        private static Ball[] DetectBalls(Mat s, Mat h, int ballWidth = 36, double resizeK = 0.5)
        {
            return new Ball[] { };
            //TODO пропал Mat1b из последней версии

            //var resizedBallWidth = (int)(ballWidth * resizeK);
            //var _mat = s.Resize(resizeK).Threshold(20, 255, ThresholdType.Binary);
            //_mat.IsEnabledDispose = false;
            //var mat = new Mat1b(_mat);            

            //int minCounter = (int)(resizedBallWidth * resizedBallWidth * 0.5);
            //int distance2 = (int)(resizedBallWidth * resizedBallWidth * 0.6);

            //var mask = MakeCircleMask(resizedBallWidth);

            //var maximums = new[] { new { x = 0, y = 0, c = 0 } }.Take(0).ToArray();

            //if (true)
            //{
            //  //var indexer = mat.GetGenericIndexer<byte>();
            //  var indexer = mat.GetIndexer();

            //  var watcher = new System.Diagnostics.Stopwatch();
            //  watcher.Start();

            //  for (var cy = 0; cy < mat.Height; cy += 1)
            //  {
            //    var sizeY = Math.Min(resizedBallWidth, mat.Height - cy);
            //    for (var cx = 0; cx < mat.Width; cx += 1)
            //    {
            //      var sizeX = Math.Min(resizedBallWidth, mat.Width - cx);

            //      var counter = 0;

            //      for (var y = 0; y < sizeY; ++y)
            //      {
            //        var line = new IntPtr(mat.Data.ToInt64() + mat.Step() * (y + cy));
            //        for (var x = 0; x < sizeX; ++x)
            //        {
            //          //if (mask[y, x] > 0 && System.Runtime.InteropServices.Marshal.ReadByte(line + x + cx) > 0)
            //          if (mask[y, x] > 0 && indexer[cy + y, cx + x] > 0) // System.Runtime.InteropServices.Marshal.ReadByte(line + x + cx) > 0)
            //          {
            //            counter++;
            //          }
            //        }
            //      }

            //      if (counter > minCounter && !maximums.Any(max => counter <= max.c && (max.x - cx) * (max.x - cx) + (max.y - cy) * (max.y - cy) < distance2))
            //      {
            //        maximums = new[] { new { x = cx, y = cy, c = counter } }.Concat(maximums.Where(max => max.c > counter || (max.x - cx) * (max.x - cx) + (max.y - cy) * (max.y - cy) > distance2)).ToArray();
            //      }
            //    }
            //  }
            //  watcher.Stop();
            //  Console.WriteLine(watcher.Elapsed);

            //  return maximums
            //    .Select(max => new { x = (int)(max.x / resizeK) + ballWidth / 2, y = (int)(max.y / resizeK) + ballWidth / 2 })
            //    .Select(p => new Ball(p.x, p.y, AverageColor(h, new Point(p.x , p.y), 2)))
            //    .ToArray();
            //}
        }

        private static int[,] QuickDetectBalls_Field(Mat mat, int ballWidth)
        {
            var ballWidth_3 = ballWidth / 3;
            var detectM = new int[mat.Width / ballWidth_3 + 1, mat.Height / ballWidth_3 + 1];
            if (true)
            {
                var watcher = new System.Diagnostics.Stopwatch();
                watcher.Start();
                for (var y = 0; y < mat.Height; y += 1)
                {
                    var line = new IntPtr(mat.Data.ToInt64() + mat.Step() * y);

                    for (var x = 0; x < mat.Width; x += 1)
                    {
                        if (System.Runtime.InteropServices.Marshal.ReadByte(line + x) > 0)
                            detectM[x / ballWidth_3, y / ballWidth_3]++;
                    }
                }
                watcher.Stop();
                Console.WriteLine(watcher.Elapsed);
            }
            return detectM;
        }
        static Ball[] QuickDetectBalls(Mat s, Mat h, int ballWidth = 36)
        {
            var ballWidth_3 = ballWidth / 3;
            var detectField = QuickDetectBalls_Field(s, ballWidth);
            var marks = new int?[detectField.GetLength(0), detectField.GetLength(1)];
            Func<int, int, int?> findMark = (x, y) => (x < 0 || y < 0) ? null : marks[x, y];

            var ballGroups = new List<List<Ball>>();
            Func<int?, List<Ball>> findBallGroup = i => i == null ? null : ballGroups[i.Value];

            for (var y = 0; y < detectField.GetLength(1); ++y)
                for (var x = 0; x < detectField.GetLength(0); ++x)
                {
                    if (detectField[x, y] < 0.8 * ballWidth_3 * ballWidth_3)
                        continue;

                    var p = new Point(x * ballWidth_3 + ballWidth_3 / 2, y * ballWidth_3 + ballWidth_3 / 2);
                    var color = AverageColor(h, p, 2);
                    var dispersion = DispersionColor(h, p, 2);
                    if (dispersion > 10)
                        continue;
                    var ball = new Ball(p, color);

                    if (true)
                    {
                        var mark = findMark(x - 1, y);
                        var group = findBallGroup(mark);
                        if (group != null && Math.Abs(group.FirstOrDefault().H - color) < 10)
                        {
                            group.Add(ball);
                            marks[x, y] = mark;
                            continue;
                        }
                    }
                    if (true)
                    {
                        var mark = findMark(x, y - 1);
                        var group = findBallGroup(mark);
                        if (group != null && Math.Abs(group.FirstOrDefault().H - color) < 10)
                        {
                            group.Add(ball);
                            marks[x, y] = mark;
                            continue;
                        }
                    }
                    ballGroups.Add(new List<Ball>(new[] { ball }));
                    marks[x, y] = ballGroups.Count - 1;
                }
            return ballGroups.Select(group => new Ball((int)group.Select(ball => ball.Point.X).Average(), (int)group.Select(ball => ball.Point.Y).Average(), (int)group.Select(ball => ball.H).Average())).ToArray();
        }

        private static byte[,] MakeCircleMask(int ballWidth)
        {
            byte[,] mask = new byte[ballWidth, ballWidth];
            if (true)
            {
                var centerX = ballWidth / 2;
                var centerY = ballWidth / 2;
                var r2 = ballWidth * ballWidth / 4;
                for (var y = 0; y < ballWidth; ++y)
                    for (var x = 0; x < ballWidth; ++x)
                    {
                        if ((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY) < r2)
                            mask[y, x] = 1;
                    }
            }
            return mask;
        }

        public static bool Other(System.Drawing.Rectangle frogRect)
        {
            if (false)
            {
                var mat = Cv2.GetRotationMatrix2D(new Point2f(10, 10), 45, 1);
                Console.WriteLine(mat);
                //Console.WriteLine(mat.Dims);
                Console.WriteLine(mat.Size());
                var p = new Mat(3, 1, MatType.CV_64FC1);
                p.Set<double>(0, 0, 0);
                p.Set<double>(1, 0, 0);
                p.Set<double>(2, 0, 1);
                Console.WriteLine(p);
                //var q = mat.Cross(p);
                var q = (mat * p).ToMat();
                Console.WriteLine(q);
                Console.WriteLine(q.Get<double>(0, 0));
                Console.WriteLine(q.Get<double>(1, 0));
                return true;
            }
            if (false)
            {
                var dir = "Example2/";

                var src = new Mat("0.bmp");
                var frog = src.Cut(new Rect(frogRect.X, frogRect.Y, frogRect.Width, frogRect.Height));
                frog.ImWrite(dir + "/frog.bmp");


                var frog_hsv_channels = frog.CvtColor(ColorConversion.RgbToHsv).Split();
                for (var i = 0; i < 3; ++i)
                    frog_hsv_channels[i].ImWrite(dir + string.Format("frog {0}.png", hsv_names[i]));

                frog_hsv_channels[1].Threshold(210, 255, ThresholdType.Binary).ImWrite(dir + "frog s th.png");
                return true;
            }
            if (false)
            {
                MakeImagesForArticle();

                return true;
            }
            if (false)
            {
                DetectBallsForArticle();
                return true;
            }
            if (false)
            {
                var images = Enumerable.Range(0, 3).Select(i => string.Format("{0}.bmp", i)).Select(name => LoadBitmap(name).ToImage()).ToArray();
                //var zeroImage = new int[images[0].GetLength(0), images[0].GetLength(1)];

                var diff0_1 = images[0].Diff(images[1], 0, 0, 10);
                var diff1_2 = images[1].Diff(images[2], 0, 0, 10);
                diff0_1.ToBitmap().Save("diff s 0 1.bmp");
                diff1_2.ToBitmap().Save("diff s 1 2.bmp");
                diff0_1.And(diff1_2).ToBitmap().Save("diff s.bmp");

                foreach (var t in new[] { 0, 10, 20, 50, 100 })
                {
                    for (var dy = 0; dy < 10; ++dy)
                    {
                        images[1].Diff(images[0], 0, dy, t).ToBitmap().Save(string.Format("diff 0 {0} {1}.bmp", dy, t));
                    }
                }
                //foreach (var t in new[] { 0, 10, 20, 50, 100, 150 })
                //{
                //  images[1].Diff(zeroImage, 0, 0, t).ToBitmap().Save(string.Format("diff-z {0}.bmp", t));
                //}
                return true;
            }
            return false;
        }
    }
    class Ball
    {
        public Ball(Point p, int h)
        {
            this.Point = p;
            this.H = h;
        }
        public Ball(int x, int y, int h) : this(new Point(x, y), h)
        {
        }
        public readonly Point Point;
        public readonly int H;
    }
    static class OpenCvHlp
    {
        public static Scalar ToScalar(this Color color)
        {
            return new Scalar(color.B, color.G, color.R);
        }
        public static void CopyTo(this Mat src, Mat dst, Rect rect)
        {
            var mask = new Mat(src.Rows, src.Cols, MatType.CV_8UC1);
            mask.Rectangle(rect, new Scalar(255), -1);
            src.CopyTo(dst, mask);
        }
        public static Mat Absdiff(this Mat src, Mat src2)
        {
            var dst = new Mat();
            Cv2.Absdiff(src, src2, dst);
            return dst;
        }
        public static Mat CvtColor(this Mat src, ColorConversion code)
        {
            var dst = new Mat();
            Cv2.CvtColor(src, dst, code);
            return dst;
        }
        public static Mat Threshold(this Mat src, double thresh, double maxval, ThresholdType type)
        {
            var dst = new Mat();
            Cv2.Threshold(src, dst, thresh, maxval, type);
            return dst;
        }

        public static Mat ThresholdStairs(this Mat src)
        {
            var dst = new Mat(src.Rows, src.Cols, src.Type());

            var partCount = 10;
            var partWidth = src.Width / partCount;

            for (var i = 0; i < partCount; ++i)
            {
                var th_mat = new Mat();
                Cv2.Threshold(src, th_mat, 255 / 10 * (i + 1), 255, ThresholdType.Binary);
                th_mat.Rectangle(new Rect(0, 0, partWidth * i, src.Height), new Scalar(0), -1);
                th_mat.Rectangle(new Rect(partWidth * (i + 1), 0, src.Width - partWidth * (i + 1), src.Height), new Scalar(0), -1);

                Cv2.Add(dst, th_mat, dst);
            }
            var color_dst = new Mat();
            Cv2.CvtColor(dst, color_dst, ColorConversion.GrayToRgb);
            for (var i = 0; i < partCount; ++i)
            {
                color_dst.Line(partWidth * i, 0, partWidth * i, src.Height, new CvScalar(50, 200, 50), thickness: 2);
            }
            return color_dst;
        }
        public static Mat With_Title(this Mat mat, string text)
        {
            var res = new Mat(mat.ToIplImage(), true);
            res.Rectangle(new Rect(res.Width / 2 - 10, 30, 20 + text.Length * 15, 25), new Scalar(0), -1);
            res.PutText(text, new OpenCvSharp.CPlusPlus.Point(res.Width / 2, 50), FontFace.HersheyComplex, 0.7, new Scalar(150, 200, 150));
            return res;
        }
        public static Mat Resize(this Mat src, double k)
        {
            var dst = new Mat();
            Cv2.Resize(src, dst, new OpenCvSharp.CPlusPlus.Size((int)(src.Width * k), (int)(src.Height * k)));
            return dst;
        }
        public static Mat Cut(this Mat src, Rect rect)
        {
            return new Mat(src, rect);
        }
        public static Mat[] Split(this Mat hsv_background)
        {
            Mat[] hsv_background_channels;
            Cv2.Split(hsv_background, out hsv_background_channels);
            return hsv_background_channels;
        }

    }
    static class Hlp
    {
        public static PointF ToPointF(this Mat mat)
        {
            return new PointF((float)mat.Get<double>(1, 0), (float)mat.Get<double>(0, 0));
        }
        public static Mat ToMat(this PointF point)
        {
            return new Mat(3, 1, MatType.CV_64FC1, new double[] { point.Y, point.X, 1 });
        }

        public static byte[] ToBytes(this Bitmap bmp)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
    //class MatHlp
    //{
    //  public static byte Pixel(this Mat mat, int x, int y)
    //  {
    //    return System.Runtime.InteropServices.Marshal.ReadByte(mat.Data + mat.
    //  }
    //}
    public class ScreenCheck
    {
        public ScreenCheck(string name, CheckPoint[] points)
        {
            this.Name = name;
            this.Points = points;
        }
        public readonly string Name;
        public readonly CheckPoint[] Points;
    }
}
