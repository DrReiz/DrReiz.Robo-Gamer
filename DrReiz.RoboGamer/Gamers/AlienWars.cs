using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Gamering
{
  class AlienWars:DirectGamer
  {
    public static void Execute()
    {
      var war_panel_width = 144;
      var war_area = new Rectangle(war_panel_width, 0, 800 - 2 * war_panel_width, 600);
      if (true)
      {
        var images = Enumerable.Range(0, 3).Select(i => string.Format("{0}.bmp", i)).Select(name => LoadBitmap(name).ToImage().Cut(war_area)).ToArray();
        var zeroImage = new int[images[0].GetLength(0), images[0].GetLength(1)];

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
        return;
      }

      var handle = FindWindow(null, "Windows81 [Running] - Oracle VM VirtualBox");
      if (handle == IntPtr.Zero)
        throw new Exception("Окно не найдено");
      //Console.WriteLine(handle);
      //SetForegroundWindow(handle);

      //CaptureWindow(handle).Save("q.bmp");

      //return;

      var vm_left = 8;
      var vm_right = 8;
      var vm_top = 50;
      var vm_bottom = 30;

      RECT rect;
      GetWindowRect(handle, out rect);

      var screenChecks =
        new[]
        {
          new
          {
            Name = "main",
            Points = new[]
            {
              new CheckPoint(200, 190, 0xffdbca24),
              new CheckPoint(65, 400, 0xff078194)
            }
          },
          new
          {
            Name = "mission",
            Points = new[]
            {
              new CheckPoint(200, 190, 0xffdbca24),
              new CheckPoint(65, 310, 0xff078194),
              new CheckPoint(65, 400, 0xff21241c),
            }
          },
          new
          {
            Name = "war",
            Points = new[]
            {
              new CheckPoint(140, 560, 0xffafaa9f),
              new CheckPoint(660, 560, 0xff6a6359),
            }
          },
        };

      Func<Bitmap, string> check = image => screenChecks.Where(_check => image.Check(_check.Points)).Select(_check => _check.Name).FirstOrDefault();

      var gameScreenRect = new System.Drawing.Rectangle(rect.Left + vm_left, rect.Top + vm_top, rect.Right - rect.Left - vm_right - vm_left, rect.Bottom - rect.Top - vm_bottom - vm_top);
      if (true)
      {
        var bmp = GetScreenImage(gameScreenRect);
        bmp.Save("q.bmp");
        //Display(bmp, screenChecks[0]);
      }

      var game_width = gameScreenRect.Width;
      var game_height = gameScreenRect.Height;


      var startButtonPoint = new Point(150, 325);
      var startMissionPoint = new Point(150, 345);

      var war_left = 142;
      var war_right = 658;
      var ship_width = 60;

      var vm_host = "192.168.1.35";

      var client = new System.Net.Sockets.TcpClient(vm_host, 7001);
      var clientStream = client.GetStream();
      var clientWriter = new System.IO.BinaryWriter(clientStream);

      Action<MouseEventFlags, int, int> mouse_event = (flags, x, y) =>
        {
          var messageStream = new System.IO.MemoryStream();
          var messageWriter = new System.IO.BinaryWriter(messageStream);
          messageWriter.Write(0);
          messageWriter.Write((uint)flags);
          messageWriter.Write(x);
          messageWriter.Write(y);
          messageWriter.Write(0);
          var message = messageStream.ToArray();
          clientWriter.Write(message.Length);
          clientWriter.Write(message);
        };

      var startTime = DateTime.UtcNow;
      string prevScreenName = null;
      string prevKnownScreenName = null;
      //Bitmap prevBmp = null;
      var history = new List<Bitmap>();
      for (; ; )
      {
        try
        {
          var bmp = GetScreenImage(gameScreenRect);
          var screenName = check(bmp);
          switch (screenName)
          {
            case "main":
              mouse_event(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, startButtonPoint.X * 65536 / game_width, startButtonPoint.Y * 65536 / game_height);
              System.Threading.Thread.Sleep(10);
              mouse_event(MouseEventFlags.LEFTDOWN, 0, 0);
              System.Threading.Thread.Sleep(150);
              mouse_event(MouseEventFlags.LEFTUP, 0, 0);
              System.Threading.Thread.Sleep(50);
              break;
            case "mission":
              mouse_event(MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE, startMissionPoint.X * 65536 / game_width, startMissionPoint.Y * 65536 / game_height);
              System.Threading.Thread.Sleep(10);
              mouse_event(MouseEventFlags.LEFTDOWN, 0, 0);
              System.Threading.Thread.Sleep(150);
              mouse_event(MouseEventFlags.LEFTUP, 0, 0);
              System.Threading.Thread.Sleep(50);
              break;
            case "war":
              {
                if (prevKnownScreenName != "war")
                  startTime = DateTime.UtcNow;
                var tick = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;

                var x = war_left + ship_width / 2 + ((tick / 10) % (war_right - war_left - ship_width));

                mouse_event((MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE), x * 65536 / game_width, 65536);
                mouse_event((MouseEventFlags.LEFTDOWN), 0, 0);
                System.Threading.Thread.Sleep(100);
                mouse_event((MouseEventFlags.LEFTUP), 0, 0);
                System.Threading.Thread.Sleep(50);

                if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Spacebar)
                {
                  bmp.Save("0.bmp");
                  for (var i = 0; i < history.Count; ++i)
                    history[i].Save(string.Format("{0}.bmp", i + 1));
                  Console.WriteLine("save");
                  //Analyze(prevBmp, bmp);
                }
              }
              break;
            case null:
              bmp.Save("unknown.bmp");
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

    public static Bitmap CaptureWindow(IntPtr handle)
    {
      var hdcSrc = GetWindowDC(handle);
      RECT windowRect;
      GetWindowRect(handle, out windowRect);
      var width = windowRect.Right - windowRect.Left;
      var height = windowRect.Bottom - windowRect.Top;
      var hdcDest = CreateCompatibleDC(hdcSrc);
      try
      {
        var hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);
        var hOld = SelectObject(hdcDest, hBitmap);
        try
        {

          BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, SRCCOPY);

          return (Bitmap)Image.FromHbitmap(hBitmap);
        }
        finally
        {
          SelectObject(hdcDest, hOld);
          DeleteObject(hBitmap);
        }
      }
      finally
      {
        DeleteDC(hdcDest);
        ReleaseDC(handle, hdcSrc);
      }
        
    }



  }

}
