using DrReiz.GumballGamer.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer.Wui.Controllers
{
    public class VisionController: Controller
    {
        public VisionController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        IHostingEnvironment hostingEnvironment;

        [HttpGet("/visionShots/")]
        public object Shots()
        {
            return
                Directory
                    .EnumerateFiles(Path.Combine(hostingEnvironment.WebRootPath, "Data/Vision"))
                    .Select(filename => Path.GetFileName(filename))
                    .OrderByDescending(filename => filename)
                    .Take(100);
        }

        [HttpGet("/screenshot/{name}")]
        public IActionResult Screenshot(string name)
        {
            var dir = @"t:\Data\Gumball\Screenshots";
            return File(System.IO.File.ReadAllBytes(Path.Combine(dir, name + ".png")), "image/png");
        }


        [HttpGet("/ping")]
        public async Task<string> Ping()
        {
            using (var client = new ClientBuilder()
                 .UseLocalhostClustering()
                 //.ConfigureLogging(logging => logging.AddConsole())
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGumballPing).Assembly).WithReferences())
                 .Build())
            {
                await client.Connect();

                var pingerId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
                var pinger = client.GetGrain<IGumballPing>(pingerId);
                return await pinger.Ping(DateTime.UtcNow.Ticks.ToString());
            }

        }
        [HttpPost("/screenshot/capture")]
        public async Task<IActionResult> CaptureScreenshot()
        {
            using (var client = new ClientBuilder()
                 .UseLocalhostClustering()
                 //.ConfigureLogging(logging => logging.AddConsole())
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGumballPing).Assembly).WithReferences())
                 .Build())
            {
                await client.Connect();

                var pingerId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
                var pinger = client.GetGrain<IGumballPing>(pingerId);
                var bytes = await pinger.CaptureScreenshot();
                return File(bytes, "image/png");
            }

        }
    }
}
