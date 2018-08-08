using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DrReiz.GumballGamer
{
    public class MapController: ApiController
    {
        [Route("ping")]
        [HttpGet]
        public string Ping()
        {
            return "gamer.web-service";
        }
    }
}
