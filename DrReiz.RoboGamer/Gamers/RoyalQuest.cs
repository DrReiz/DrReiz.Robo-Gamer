using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace Gamering
{
  class RoyalQuest:DirectGamer
  {
    public static void Main1()
    {
      //Console.WriteLine(MapVirtualKey(0x20, MAPVK_VK_TO_VSC));
      //return;
      var process = Process.GetProcessesByName("rqmain").FirstOrDefault();
      Console.WriteLine(process.MainWindowTitle);
      var windowHandle = process.MainWindowHandle;

      SetForegroundWindow(windowHandle);

      RECT rect;
      GetWindowRect(windowHandle, out rect);

      var bmp = GetScreenImage(new Rectangle { X = rect.Left, Y = rect.Top, Width = rect.Right - rect.Left, Height = rect.Bottom - rect.Top });

      bmp.Save(@"test.png", ImageFormat.Png); 

      //SendKeys.SendWait("b");
      //keybd_event(0x42, 48, 0, IntPtr.Zero);
      //keybd_event(0x42, 48, 2, IntPtr.Zero);

      for (; ; )
      {
        //keybd_event(0x12, 56, 0, IntPtr.Zero);
        keybd_event(0x31, 2, 0, IntPtr.Zero);
        keybd_event(0x31, 2, 2, IntPtr.Zero);

        keybd_event(0x20, 57, 0, IntPtr.Zero);
        keybd_event(0x20, 57, 2, IntPtr.Zero);

        //keybd_event(0x12, 56, 2, IntPtr.Zero);

        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
      }

      //var width = 100;
      //var height = 100;



      //System.Threading.Thread.Sleep(TimeSpan.FromMinutes(1));

      //var window = new NativeWindow(); window.AssignHandle(windowHandle);
      
    }

  }

}
