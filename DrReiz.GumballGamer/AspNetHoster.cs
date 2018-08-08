using NitroBolt.CommandLine;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DrReiz.GumballGamer
{
    public class AspNetHoster
    {
        public void Configuration(IAppBuilder appBuilder)
        {

            // Configure Web API for self-host. 

            var config = new HttpConfiguration();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);

            config.EnsureInitialized(); //без этой команды проглатывает ошибки инициализации
        }

        [CommandLine("web-service")]
        public static void Execute()
        {
            string baseAddress = "http://localhost:8471/";

            using (Microsoft.Owin.Hosting.WebApp.Start<AspNetHoster>(url: baseAddress))
            {
                Console.WriteLine("web-service started");
                Console.ReadKey();

            }
        }
    }
}
