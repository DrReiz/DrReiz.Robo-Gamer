using Newtonsoft.Json;
using NitroBolt.CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.Gamer.DevConsole
{
    public static class VirtualBoxBrowser
    {
        [CommandLine("--virtualbox")]
        public async static Task Execute()
        {
            await Virtualbox_Execute();
            //await Bluestack_TakeScreenshot();
            //Bluestacks_Execute();
        }

        static unsafe void Screenshot(VirtualBox.Session session)
        {
            var img = new byte[10_000_000];
            fixed (byte* ptr = &img[0])
            {
                session.Console.Display.TakeScreenShot(0, ref img[0], 800, 600, VirtualBox.BitmapFormat.BitmapFormat_RGBA);
            }
        }

        [DllImport(@"C:\Program Files\Oracle\VirtualBox\VBoxRT.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void RTR3InitExe(int argc, string argv, int zeroflags);
        static async Task Virtualbox_Execute()
        {
            try
            {
                //var machineName = "Android";
                //var machineName = "Google Pixel 2";
                //var machineName = "Google Nexus 9";
                var machineName = "Win10";

                var virtualbox = new VirtualBox.VirtualBox();

                //Browse(virtualbox);
                //return;


                //RTR3InitExe(0, "", 0);

                var browsedMachine = virtualbox.FindMachine(machineName);
                if (browsedMachine == null)
                    throw new Exception($"machine '{machineName}' not found");
                Console.WriteLine(browsedMachine.State);

                var session = new VirtualBox.Session();
                Console.WriteLine(session.State);

                browsedMachine.LockMachine(session, VirtualBox.LockType.LockType_Shared);
                try
                {
                    Console.WriteLine("Locking...");
                    while (session.State != VirtualBox.SessionState.SessionState_Locked)
                        await Task.Delay(TimeSpan.FromSeconds(0.1));
                    Console.WriteLine("Locked");
                    Console.WriteLine(session.State);
                    Console.WriteLine(browsedMachine.State);

                    //Console.WriteLine("launching Vm..");
                    //browsedMachine.LaunchVMProcess(session, "gui", "").WaitForCompletion(10000);
                    //Console.WriteLine("launched");


                    var machine = session.Machine;
                    Console.WriteLine(machine.State);

                    //Console.WriteLine("Powering up...");
                    //Console.WriteLine(session.Console != null);
                    //var powerUpProgress = session.Console.PowerUp();
                    //powerUpProgress.WaitForCompletion(10000);
                    //Console.WriteLine(session.State);
                    //Console.WriteLine(machine.State);
                    //Console.WriteLine(powerUpProgress.ErrorInfo?.Text);

                    Console.WriteLine(session?.Console?.Display != null);
                    var watch = new Stopwatch();
                    watch.Start();

                    var img = (byte[])session.Console.Display.TakeScreenShotToArray(0, 800, 600, VirtualBox.BitmapFormat.BitmapFormat_RGBA);
                    Console.WriteLine(watch.Elapsed);
                    Console.WriteLine(img.Length);
                    File.WriteAllBytes($"q.{DateTime.UtcNow.Ticks}.bin", img);

                    //Screenshot(session);

                    //session.Console.Display.TakeScreenShot(0, )

                    //Console.WriteLine("Framebuffer attach");
                    //session.Console.Display.AttachFramebuffer(0, new Framebuffer(session.Console.Display));

                    //session.Console.Display.SetSeamlessMode(0);
                    //session.Console.Display.SetVideoModeHint(0, 1, 0, 0, 0, 640, 480, 32);
                    //for (; ; )
                    //{
                    //    Console.Write(".");
                    //    //await Task.Delay(TimeSpan.FromSeconds(1));
                    //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                    //}
                    Console.WriteLine(",");
                }
                finally
                {
                    session.UnlockMachine();
                }
            }
            catch(Exception exc)
            {
                Console.Error.WriteLine(exc);
                throw;
            }

            return;

        }

        static async Task Bluestack_TakeScreenshot()
        {
            try
            {
                //var machineName = "Android";
                //var machineName = "Google Pixel 2";
                //var machineName = "Google Nexus 9";
                var machineName = "Android";

                var virtualbox = new BlueStacks.VirtualBox();

                //Browse(virtualbox);
                //return;


                //RTR3InitExe(0, "", 0);

                var browsedMachine = virtualbox.FindMachine(machineName);
                if (browsedMachine == null)
                    throw new Exception($"machine '{machineName}' not found");
                Console.WriteLine(browsedMachine.State);

                var session = new BlueStacks.Session();
                Console.WriteLine(session.State);

                browsedMachine.LockMachine(session, BlueStacks.LockType.LockType_Shared);
                try
                {
                    Console.WriteLine("Locking...");
                    while (session.State != BlueStacks.SessionState.SessionState_Locked)
                        await Task.Delay(TimeSpan.FromSeconds(0.1));
                    Console.WriteLine("Locked");
                    Console.WriteLine(session.State);
                    Console.WriteLine(browsedMachine.State);

                    //Console.WriteLine("launching Vm..");
                    //browsedMachine.LaunchVMProcess(session, "gui", "").WaitForCompletion(10000);
                    //Console.WriteLine("launched");


                    var machine = session.Machine;
                    Console.WriteLine(machine.State);

                    //Console.WriteLine("Powering up...");
                    //Console.WriteLine(session.Console != null);
                    //var powerUpProgress = session.Console.PowerUp();
                    //powerUpProgress.WaitForCompletion(10000);
                    //Console.WriteLine(session.State);
                    //Console.WriteLine(machine.State);
                    //Console.WriteLine(powerUpProgress.ErrorInfo?.Text);

                    Console.WriteLine(session?.Console?.Display != null);
                    var watch = new Stopwatch();
                    watch.Start();

                    var img = (byte[])session.Console.Display.TakeScreenShotPNGToArray(0, 800, 600);
                    Console.WriteLine(watch.Elapsed);
                    Console.WriteLine(img.Length);
                    File.WriteAllBytes($"q.{DateTime.UtcNow.Ticks}.bin", img);

                    //Screenshot(session);

                    //session.Console.Display.TakeScreenShot(0, )

                    //Console.WriteLine("Framebuffer attach");
                    //session.Console.Display.AttachFramebuffer(0, new Framebuffer(session.Console.Display));

                    //session.Console.Display.SetSeamlessMode(0);
                    //session.Console.Display.SetVideoModeHint(0, 1, 0, 0, 0, 640, 480, 32);
                    //for (; ; )
                    //{
                    //    Console.Write(".");
                    //    //await Task.Delay(TimeSpan.FromSeconds(1));
                    //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                    //}
                    Console.WriteLine(",");
                }
                finally
                {
                    session.UnlockMachine();
                }
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc);
                throw;
            }

            return;

        }


        private static void Browse(VirtualBox.VirtualBox virtualbox)
        {
            //new BlueStacks.IMachine

            //var virtualbox = new VirtualBox.VirtualBox();

            Console.WriteLine(virtualbox.HomeFolder);
            Console.WriteLine(virtualbox.Host.OperatingSystem);
            Console.WriteLine(virtualbox.APIVersion);
            //foreach (VirtualBox.IMedium disk in virtualbox.HardDisks)
            //    Console.WriteLine(disk.Name);

            var machines = virtualbox.Machines;
            foreach (VirtualBox.IMachine machine in machines)
            {
                Console.WriteLine(machine.Name);
                if (machine.SessionName != "")
                    Console.WriteLine($"  {machine.SessionName}");
                Console.WriteLine($"  {machine.State}");
            }
        }

        static void Bluestacks_Execute()
        {
            var virtualbox = new BlueStacks.VirtualBox();

            //new BlueStacks.IMachine

            //var virtualbox = new VirtualBox.VirtualBox();

            Console.WriteLine(virtualbox.HomeFolder);
            Console.WriteLine(virtualbox.Host.OperatingSystem);
            Console.WriteLine(virtualbox.APIVersion);
            foreach (BlueStacks.IMedium disk in virtualbox.HardDisks)
                Console.WriteLine(disk.Name);

            var machines = virtualbox.Machines;
            var machine = (BlueStacks.IMachine)machines.GetValue(0);
            Console.WriteLine(machine.Name);
            Console.WriteLine(machine.LogFolder);

            //foreach (BlueStacks.IMachine machine in machines)
            //    Console.WriteLine(machine.Name);

        }
    }
}
