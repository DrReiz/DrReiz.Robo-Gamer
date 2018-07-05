using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DrReiz.GumballGamer.Messages;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.Extensions.Logging;


namespace DrReiz.GumballGamer
{
    class OrleansServer
    {
        public static async Task Execute()
        {
            await StartSilo();


        }
        public static async Task StartSilo()
        {
            using (var host = new SiloHostBuilder()
                 .UseLocalhostClustering()
                 //.ConfigureLogging(logging => logging.AddConsole())
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PingGrain).Assembly).WithReferences())
                 .Build())
            {
                await host.StartAsync();

                Console.WriteLine("Silo started. Press any key to terminate...");
                Console.ReadKey();
            }
        }
    }

    public class PingGrain : Orleans.Grain, IGumballPing
    {
        public async Task<byte[]> CaptureScreenshot()
        {
            using (var client = new AdbClient("emulator-5554"))
            {
                var bitmap = client.CaptureScreenshot();
                using (var stream = new System.IO.MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }

        public Task<string> Ping(string msg)
        {
            Console.WriteLine("Gumball pinged");
            return Task.FromResult($"Gumball ping response: {DateTime.UtcNow.Ticks}");
        }
    }

}
