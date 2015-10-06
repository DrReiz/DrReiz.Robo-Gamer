using NitroBolt.Wui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrReiz.RoboGamer
{
    public class HSync : HWebSynchronizeHandler
    {
        public HSync()
          : base(new Dictionary<string, Func<object, JsonData[], HContext, HtmlResult<HElement>>>
            {
             { "index", Main.HView },
            })
        {
        }
    }
}