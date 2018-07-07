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

        static string storageDir = @"t:\Data\Gumball\Screenshots";

        [HttpGet("/visionShots/")]
        public object Shots()
        {
            var dirs = new[] { Path.Combine(hostingEnvironment.WebRootPath, "Data/Vision"), storageDir };

            return
                dirs.SelectMany(dir => 
                    Directory
                    .EnumerateFiles(dir)
                    .Select(filename => Path.GetFileName(filename))
                    .OrderByDescending(name => name)
                    .Take(100))
                    .OrderByDescending(name => name);
        }

        [HttpGet("/screenshot/{name}")]
        public IActionResult Screenshot(string name)
        {
            return File(System.IO.File.ReadAllBytes(Path.Combine(storageDir, name + ".png")), "image/png");
        }


        [HttpGet("/ping")]
        public async Task<string> Ping()
        {
            using (var client = new ClientBuilder()
                 .UseLocalhostClustering()
                 //.ConfigureLogging(logging => logging.AddConsole())
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ISample).Assembly).WithReferences())
                 .Build())
            {
                await client.Connect();

                var pingerId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
                var pinger = client.GetGrain<ISample>(pingerId);
                return await pinger.Ping(DateTime.UtcNow.Ticks.ToString());
            }

        }
        [HttpPost("/screenshot/capture")]
        public async Task<IActionResult> CaptureScreenshot()
        {
            using (var client = new ClientBuilder()
                 .UseLocalhostClustering()
                 //.ConfigureLogging(logging => logging.AddConsole())
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGumball).Assembly).WithReferences())
                 .Build())
            {
                await client.Connect();

                var gumballId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
                var gumball = client.GetGrain<IGumball>(gumballId);
                var name = await gumball.CaptureScreenshot();
                return Json(new { visionShot= name });
            }

        }
    }
}
