using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DrReiz.AndroidGamer.Wui.Controllers
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

            //var dirs = new[] { Path.Combine(hostingEnvironment.WebRootPath, "Data/Vision"), context.StorageDir };
            var dirs = new[] {context.StorageDir };

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
        [HttpPost("/{game}/screenshot/{name}/ocr-perception")]
        public IActionResult OcrPerception(string game, string name)
        {
            var context = GameContext.Get(game);

            //using (var client = new ClientBuilder()
            //     .UseLocalhostClustering()
            //     //.ConfigureLogging(logging => logging.AddConsole())
            //     .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ISample).Assembly).WithReferences())
            //     .Build())
            //{
            //    await client.Connect();

            //    var gocr = client.GetGrain<IGocr>("one");
            //    var shot = await gocr.PerceptionText(Path.Combine(context.StorageDir, name + ".png"));
            //    return Json(shot);
            //}

            return Json(new { });
        }


        [HttpGet("/ping")]
        public string Ping()
        {
            //using (var client = new ClientBuilder()
            //     .UseLocalhostClustering()
            //     //.ConfigureLogging(logging => logging.AddConsole())
            //     .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ISample).Assembly).WithReferences())
            //     .Build())
            //{
            //    await client.Connect();

            //    var pingerId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
            //    var pinger = client.GetGrain<ISample>(pingerId);
            //    return await pinger.Ping(DateTime.UtcNow.Ticks.ToString());
            //}
            return "-";
        }
        [HttpPost("/{game}/screenshot/capture")]
        public IActionResult CaptureScreenshot(string game)
        {
            var screenshotName = AdbDroid.CaptureScreenshot(game);
            return Json(new { visionShot= screenshotName });

            //using (var client = new ClientBuilder()
            //     .UseLocalhostClustering()
            //     //.ConfigureLogging(logging => logging.AddConsole())
            //     .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGamer).Assembly).WithReferences())
            //     .Build())
            //{
            //    await client.Connect();

            //    var gameId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
            //    var gamer = client.GetGrain<IGamer>(gameId);
            //    var name = await gamer.CaptureScreenshot(game);
            //    return Json(new { visionShot= name });
            //}

        }
        [HttpPost("/{game}/tap")]
        public IActionResult Tap(string game, [FromBody]TapRequest request)
        {
            AdbDroid.Tap(game, request.X, request.Y);
            return Json(new {});

            //using (var client = new ClientBuilder()
            //     .UseLocalhostClustering()
            //     //.ConfigureLogging(logging => logging.AddConsole())
            //     .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGamer).Assembly).WithReferences())
            //     .Build())
            //{
            //    await client.Connect();

            //    var gameId = new Guid("{2349992C-860A-4EDA-9590-000000000006}").ToString();
            //    var gamer = client.GetGrain<IGamer>(gameId);
            //    await gamer.Tap(game, 800, 400);
            //    return Json(new {});
            //}

        }
    }
    public class TapRequest
    {
        public int X;
        public int Y;
    }
}
