using NitroBolt.Functional;
using NitroBolt.Wui;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Web;

namespace DrReiz.RoboGamer
{
    public class Main
    {
        public static NitroBolt.Wui.HtmlResult<HElement> HView(object _state, JsonData[] jsons, HContext context)
        {
            var state = _state.As<MainState>() ?? new MainState();

            foreach (var json in jsons.OrEmpty())
            {
                switch (json.JPath("data", "command")?.ToString())
                {
                    case "mouse-event":
                        {
                            var x = ConvertHlp.ToInt(json.JPath("data", "x"));
                            var y = ConvertHlp.ToInt(json.JPath("data", "y"));
                            if (x != null && y != null)
                            {
                                using (var client = new MouseClient(Zuma.VmIp))
                                {
                                    client.MoveTo(x.Value, y.Value, 800, 600);
                                }
                            }
                        }
                        break;
                    case "reset":
                        {
                            using (var client = new MouseClient(Zuma.VmIp))
                            {
                                client.Reset();
                            }
                        }
                        break;
                    case "screenshot":
                        {
                            new VmClient().Screenshot().Save(context.HttpContext.Server.MapPath($"~/App_Data/{DateTime.UtcNow.Ticks}.png"));
                        }
                        break;
                    default:
                        break;
                }
            }

            var page = Page(state);
            return new NitroBolt.Wui.HtmlResult<HElement>
            {
                Html = page,
                State = state,
            };
        }
        private static HElement Page(MainState state)
        {
            return h.Html
            (
              h.Head(
                h.Element("title", "RoboGamer.Wui"),
                h.Css
                (
                    @"
                      .section {min-height:100px;border:1px solid lightgray;margin-bottom:2px;}
                      .header {background-color:#E0E0FF;}
                    "
                )
              ),
              h.Body
              (
                  h.Div
                  (
                      h.data("name", "mouse-event"),
                      h.Input(h.type("text"), h.data("name", "x")),
                      h.Input(h.type("text"), h.data("name", "y")),
                      h.Input(h.type("button"), h.value("Move"), new hdata { { "container", "mouse-event" }, { "command", "mouse-event" } }, h.onclick(";"))
                  ),
                  h.Div
                  (
                      h.Input(h.type("button"), h.value("Reset"), new hdata { { "command", "reset" } }, h.onclick(";")),
                      h.Input(h.type("button"), h.value("Screenshot"), new hdata { { "command", "screenshot" } }, h.onclick(";"))
                  )
              )
           );
        }

        static readonly HBuilder h = null;
    }
    class MainState
    {
        public MainState()
        {
        }
    }
}