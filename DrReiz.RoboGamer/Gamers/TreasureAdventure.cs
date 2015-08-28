using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Gamering
{
  class TreasureAdventure: DirectGamer
  {
    public static void Execute()
    {
      Console.WriteLine(MapVirtualKey(0x20, MAPVK_VK_TO_VSC));

      //return;

      var process = Process.GetProcessesByName("treasure_adventure_game_1.0").FirstOrDefault();
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

      //var isScanCode = true;

      for (; ; )
      {
        SendKey(VirtualKeyShort.LEFT, true);
        System.Threading.Thread.Sleep(50);
        SendKey(VirtualKeyShort.LEFT, false);
        SendKey(VirtualKeyShort.RIGHT, true);
        System.Threading.Thread.Sleep(50);
        SendKey(VirtualKeyShort.RIGHT, false);
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
        //foreach (var isScanCode in new[] { true })
        //{
        //  if (true)
        //  {
        //    var InputData = new INPUT[1];

        //    InputData[0].type = 1;
        //    //InputData[0].Vk = (ushort)DirectInputKeyScanCode;  //Virtual key is ignored when sending scan code
        //    InputData[0].U.ki.wVk = VirtualKeyShort.SPACE;
        //    InputData[0].U.ki.wScan = ScanCodeShort.SPACE;
        //    InputData[0].U.ki.dwFlags = isScanCode ? KEYEVENTF.SCANCODE : 0;
        //    InputData[0].U.ki.time = 0;
        //    InputData[0].U.ki.dwExtraInfo = UIntPtr.Zero;

        //    Console.WriteLine(Marshal.SizeOf(typeof(INPUT)));
        //    // Send Keyup flag "OR"ed with Scancode flag for keyup to work properly
        //    var r = SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        //    Console.WriteLine(r);
        //    if (r == 0)
        //      Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        //    //Marshal.ThrowExceptionForHR((int)r);
        //  }
        //  System.Threading.Thread.Sleep(10);
        //  if (true)
        //  {
        //    var InputData = new INPUT[1];

        //    InputData[0].type = 1;
        //    //InputData[0].Vk = (ushort)DirectInputKeyScanCode;  //Virtual key is ignored when sending scan code
        //    InputData[0].U.ki.wVk = VirtualKeyShort.SPACE;
        //    InputData[0].U.ki.wScan = ScanCodeShort.SPACE;
        //    //InputData[0].U.ki.dwFlags = (isScanCode ? KEYEVENTF.SCANCODE : 0) | KEYEVENTF.KEYUP;
        //    InputData[0].U.ki.dwFlags = KEYEVENTF.KEYUP;
        //    InputData[0].U.ki.time = 0;
        //    InputData[0].U.ki.dwExtraInfo = UIntPtr.Zero;

        //    // Send Keyup flag "OR"ed with Scancode flag for keyup to work properly
        //    var r = SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        //    Console.WriteLine(r);
        //  }
        //  System.Threading.Thread.Sleep(1000);
        //}
      }

      //for (; ; )
      //{
      //  //keybd_event(0x12, 56, 0, IntPtr.Zero);
      //  //keybd_event(0x31, 2, 0, IntPtr.Zero);
      //  //keybd_event(0x31, 2, 2, IntPtr.Zero);

      //  //keybd_event(0x20, 57, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
      //  //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.3));
      //  //keybd_event(0x20, 57, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, IntPtr.Zero);

      //  //keybd_event(0x20, 0, 0, IntPtr.Zero);
      //  //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.3));
      //  //keybd_event(0x20, 0, KEYEVENTF_KEYUP, IntPtr.Zero);

      //  keybd_event((byte)VirtualKeyShort.LEFT, 0, 0, IntPtr.Zero);
      //  keybd_event((byte)VirtualKeyShort.LEFT, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
      //  keybd_event((byte)VirtualKeyShort.RIGHT, 0, 0, IntPtr.Zero);
      //  keybd_event((byte)VirtualKeyShort.RIGHT, 0, KEYEVENTF_KEYUP, IntPtr.Zero);

      //  //keybd_event((byte)'d', 2, 0, IntPtr.Zero);
      //  //keybd_event(0x12, 56, 2, IntPtr.Zero);

      //  System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
      //}
    }
    static void SendKey(VirtualKeyShort vk, bool isDown)
    {
      var InputData = new INPUT[1];

      InputData[0].type = 1;
      InputData[0].U.ki.wVk = vk;
      InputData[0].U.ki.wScan = (ScanCodeShort)MapVirtualKey((uint)vk, MAPVK_VK_TO_VSC);
      //InputData[0].U.ki.dwFlags = (isScanCode ? KEYEVENTF.SCANCODE : 0) | KEYEVENTF.KEYUP;
      InputData[0].U.ki.dwFlags = (isDown ? 0 : KEYEVENTF.KEYUP) | KEYEVENTF.SCANCODE;
      InputData[0].U.ki.time = 0;
      InputData[0].U.ki.dwExtraInfo = UIntPtr.Zero;

      var r = SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
    }

  

  }
  //[StructLayout(LayoutKind.Sequential)]
  //public struct INPUT
  //{
  //  internal uint type;
  //  internal VirtualKeyShort wVk;
  //  internal ScanCodeShort wScan;
  //  internal KEYEVENTF dwFlags;
  //  internal int time;
  //  internal UIntPtr dwExtraInfo;
  //  internal static int Size
  //  {
  //    get { return Marshal.SizeOf(typeof(INPUT)); }
  //  }
  //}


}
