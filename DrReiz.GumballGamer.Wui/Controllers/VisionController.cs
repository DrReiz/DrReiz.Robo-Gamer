using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
        public object Index()
        {
            return
                Directory
                    .EnumerateFiles(Path.Combine(hostingEnvironment.WebRootPath, "Data/Vision"))
                    .Select(filename => Path.GetFileName(filename))
                    .OrderByDescending(filename => filename)
                    .Take(100);
        }

    }
}
