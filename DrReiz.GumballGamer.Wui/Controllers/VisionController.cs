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

        [HttpGet("/{game}/visionShots/")]
        public object Shots(string game)
        {
            var context = GameContext.Get(game);

            var dirs = new[] { Path.Combine(hostingEnvironment.WebRootPath, "Data/Vision"), context.StorageDir };

            return
                dirs.SelectMany(dir => 
                    Directory
                    .EnumerateFiles(dir)
                    .Select(filename => Path.GetFileName(filename))
                    .OrderByDescending(name => name)
                    .Take(100))
                    .OrderByDescending(name => name);
        }

        [HttpGet("/{game}/screenshot/{name}")]
        public IActionResult Screenshot(string game, string name)
        {
            var context = GameContext.Get(game);
            return File(System.IO.File.ReadAllBytes(Path.Combine(context.StorageDir, name + ".png")), "image/png");
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
        [HttpPost("/{game}/screenshot/capture")]
        public async Task<IActionResult> CaptureScreenshot(string game)
        {
            using (var client = new ClientBuilder()
                 .UseLocalhostClustering()
                 //.ConfigureLogging(logging => logging.AddConsole())
                 .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGamer).Assembly).WithReferences())
                 .Build())
            {
                await client.Connect();

                var gameId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
                var gamer = client.GetGrain<IGamer>(gameId);
                var name = await gamer.CaptureScreenshot(game);
                return Json(new { visionShot= name });
            }

        }
    }
}
