using LinqToDB;
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
        public VisionController(IHostingEnvironment hostingEnvironment, GamerDataContext dataContext)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.DataContext = dataContext;
        }

        IHostingEnvironment hostingEnvironment;
        GamerDataContext DataContext;

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
            var screenshotName = CaptureByAction(game, null);

            return Json(new { visionShot= screenshotName });

        }
        [HttpPost("/{game}/wait-capture")]
        public IActionResult WaitAndCapture(string game)
        {
            var screenshotName = CaptureByAction(game, "wait");

            return Json(new { visionShot = screenshotName });

        }
        [HttpPost("/{game}/tap")]
        public IActionResult Tap(string game, [FromBody]TapRequest request)
        {
            AdbDroid.Tap(game, request.X, request.Y);
            return Json(new {});

        }
        [HttpPost("/{game}/tap-capture")]
        public IActionResult TapAndCapture(string game, [FromBody]TapRequest request)
        {
            AdbDroid.Tap(game, request.X, request.Y);

            CaptureByAction(game, $"{request.X};{request.Y}");

            var screenshotName = CaptureByAction(game, "wait");

            return Json(new { visionShot = screenshotName });

        }

        public string CaptureByAction(string game, string action)
        {
            var screenshotName = AdbDroid.CaptureScreenshot(game);
            var step = new Step { Game = game, Action = action, Source = previousStep?.Target, Target = screenshotName };
            if (action != null && previousStep?.Game == game && step.Time < previousStep.Time.AddMinutes(1))
                DataContext.Insert(step);

            previousStep = step;

            return screenshotName;
        }
        static Step previousStep = null;

        [HttpPut("/shot/{name}/category")]
        public object PutShotCategory(string name, [FromBody] PutShotCategoryRequest request)
        {
            DataContext.Insert(new ShotCategory { Shot = name, Category = request.Category });
            return new { };
        }
        [HttpGet("/shot/{name}")]
        public object Shot(string name)
        {
            var categories = DataContext.ShotCategories.Where(category => category.Shot == name).ToArray();
            return new { Categories = categories.Select(category => new { Name = category.Category }) };
        }

    }
    public class TapRequest
    {
        public int X;
        public int Y;
    }
    public class PutShotCategoryRequest
    {
        public string Category;
    }
}
