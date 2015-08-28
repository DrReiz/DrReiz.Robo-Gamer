//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Xml.XPath;
//using MetaTech.Library;
//using MetaTech.QSharp;

//namespace Cs10._2
//{

//  class Accessor<T>
//  {
//    public Func<T> Get;
//    public Action<T> Set;
//  }
//  static class Travian
//  {

//    static Lazy<T> Lazy<T>(Func<T> construct)
//    {
//      return new Lazy<T>(construct);
//    }
//    static string ReadAllTextIfExists(string path)
//    {
//      return System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : null;
//    }

//    static T Cache<T>(Accessor<T> proxy, Func<T> constructor)
//    {
//      var value = proxy.Get != null ? proxy.Get() : default(T);
//      if (value != null)
//        return value;
//      value = constructor();
//      if (proxy.Set != null)
//        proxy.Set(value);
//      return value;
//    }
//    static Accessor<string> FileAccess(string path)
//    {
//      return new Accessor<string>
//      {
//        Get = () => ReadAllTextIfExists(path),
//        Set = text => System.IO.File.WriteAllText(path, text)
//      };
//    }
//    static Func<string> WebGet(this Lazy<CookiedWebClient> client, string url)
//    {
//      return () => client.Value.DownloadString("http://ts4.travian.us/" + url)
//          .Replace(@"xmlns=""http://www.w3.org/1999/xhtml""", "");

//    }
//    static Dictionary<string, string> ParseQuery(string uri)
//    {
//      var values = new Dictionary<string, string>();

//      var parameters = System.Web.HttpUtility.ParseQueryString(new Uri(uri).Query);
//      for (var i = 0; i < parameters.Count; ++i)
//      {
//        //Console.WriteLine("{0}:{1}", parameters.GetKey(i), parameters.Get(i));
//        values[parameters.GetKey(i)] = parameters.Get(i);
//      }
//      return values;
//    }
//    static IQNodeBuilder q = null;
//    public static void Execute()
//    {

//      var startupCode = QFunctions3.Compile(QSharpTranslator.ParseQuery1(@"
//              crown
//              {
//               code
//               {
//                 .http-get('http://kinfo.ru/nui/nview-data-ccode(0)').qss-parse0()
//               };
//              }
//            "));

//      var crown = startupCode(Array<QNode>.Empty, q.QContext()).SelectMany(group => group).ToArray();

//      var qcontext = q.QContext(new QNode("crown", crown));
//      var json_parse = QFunctions3.Compile("json-parse");
//      var qnodeToText = QFunctions3.Compile("qnode-to-qs-string", "collections");
//      var qsParse = QFunctions3.Compile("qss-parse0");
      
//      //Console.WriteLine(text2);
//      var server = "http://tx3.travian.us/";
//      var client = Lazy(() => Login(server, "GrandJan", "htodjo1j"));


//      if (true)
//      {
//        var data = qsParse(new[] { new QNode(System.IO.File.ReadAllText("map.qs")) }, qcontext).SelectMany(_=>_).ToArray();

//        var cells = data.P("cell")
//          .Select(cell => 
//            { 
//              var html = SoftTech.Html.HtmlHlp2.LoadXElementFromText(cell.P("html").QValue().AsString());
//              return new
//                {
//                  x = ConvertHlp.ToInt(cell.P("x").QValue().AsString()),
//                  y = ConvertHlp.ToInt(cell.P("y").QValue().AsString()),
//                  html,
//                  kindMark = html.XPathSelectElements("//*").Count(),
//                  kinds = html.Element("div").Attribute_Get("class").NotNull().Split(' ')
//                }; 
//            }
//           )
//          .ToArray();

//        foreach (var group in cells.GroupBy(cell => cell.kindMark).OrderByDescending(group => group.Count()))
//          Console.WriteLine("{0}:{1} ({2}) {{{3}}}", group.Key, group.Count(), 
//            group.Take(3).Select(cell => string.Format("{0}:{1}", cell.x, cell.y)).JoinToString(", "),
//            group.SelectMany(cell => cell.kinds).GroupBy(kind => kind)
//             .Select(kgroup => string.Format("{0}:{1}", kgroup.Key, kgroup.Count()))
//             .JoinToString(", ")
//          );


//        var server_dir = new Uri(server).Host;
//        if (!System.IO.Directory.Exists(server_dir))
//          System.IO.Directory.CreateDirectory(server_dir);

//        foreach (var cell in cells
//          .Where(cell => 
//            {
//              if (cell.kinds.Contains("landscape"))
//                return false;
//              if (cell.kinds.Contains("oasis"))
//                return false;
//              if (cell.kinds.Contains("village"))
//              {
//                return cell.html.XPathSelectElement("div/h1[@class='textWithCoords']/span[@class='coordinates coordinatesWithText']/span[@class='coordText']").Value_Get() != "Abandoned valley";
//              }
//              return true;
//            }).Take(30))
//        {
//          Console.WriteLine("- {0}:{1} {2}", cell.x, cell.y, cell.kinds.JoinToString(", "));
//          if (cell.kinds.Contains("village"))
//          {
//            var playerTable = cell.html.XPathSelectElement("div/div[@id='map_details']/h4[text()='Player']")._n(_ => _.ElementsAfterSelf().FirstOrDefault());
//            //Console.WriteLine(playerTable);
//            var tribe = playerTable.XPathSelectElement("tbody/tr[th/text()='Tribe:']/td").Value_Get();
//            var population = playerTable.XPathSelectElement("tbody/tr[th/text()='Population:']/td").Value_Get();
//            var allianceElement = playerTable.XPathSelectElement("tbody/tr[th/text()='Alliance:']/td/a");
//            var allianceId = ConvertHlp.ToInt(allianceElement.Attribute_Get("href")._n(_ => _.Split('=').ElementAtOrDefault(1)));
//            var allianceName = allianceElement.Value_Get();
//            var ownerElement = playerTable.XPathSelectElement("tbody/tr[th/text()='Owner:']/td/a");
//            var ownerId = ConvertHlp.ToInt(ownerElement.Attribute_Get("href")._n(_ => _.Split('=').ElementAtOrDefault(1)));
//            var ownerName = ownerElement.Value_Get();
//            Console.WriteLine("  {0}, {1}:{2}, {3}:{4}, {5}", population, ownerId, ownerName, allianceId, allianceName, tribe);
//            foreach (var optionDiv in cell.html.XPathSelectElements("div/div[@class='detailImage']/div[@class='options']/div[@class='option']"))
//            {
//              var name = optionDiv.Elements().FirstOrDefault().Value_Get();
//              var url = optionDiv.Element("a").Attribute_Get("href");
//              var description = optionDiv.Elements().FirstOrDefault().Attribute_Get("title");
//              //Console.WriteLine(optionDiv);
//              Console.WriteLine("  {0}:{1} ({2})", name, url, description);
//            }

//            var reportTable = cell.html.XPathSelectElement("div/div[@id='map_details']/h4[text()='Reports']")._n(_ => _.ElementsAfterSelf().FirstOrDefault());
//            foreach (var report in reportTable.XPathSelectElements("tbody/tr/td/a"))
//            {
//              var name = report.Value_Get();
//              var url = report.Attribute_Get("href");
//              Console.WriteLine("{0}:{1}", name, url);
//              var parameters = ParseQuery(server + url);
//              Console.WriteLine(parameters.Select(pair => string.Format("{0}:{1}", pair.Key, pair.Value)).JoinToString(", "));
//              if (parameters.FindObject("aid") == "272")
//              {
//                var report_html_text = Cache(FileAccess(server_dir + "/" + "berichte" + "-" + parameters["id"] + ".html"),
//                  () =>
//                  {
//                    var msg_url = string.Format("berichte.php?id={0}&aid=272", parameters["id"]);
//                    Console.WriteLine(msg_url);
//                    return client.Value.DownloadString(server + msg_url)
//                      .Replace(@"xmlns=""http://www.w3.org/1999/xhtml""", "");
//                  }
//                );
//                var report_html = SoftTech.Html.HtmlHlp2.LoadXElementFromText(report_html_text);
//                var report_div = report_html.XPathSelectElement("//div[@id='report_surround']");
//                var time_div = report_div.XPathSelectElement(".//div[@id='time']");
//                Console.WriteLine(time_div.XPathSelectElement("div[@class='header text']").Value_Get());

//              }
//            }
//            Console.WriteLine(reportTable);
//          }
//        }


//        //foreach (var cell in data.P("cell"))
//        //{
//        //  Console.WriteLine(cell.P("html").QValue().AsString());
//        //}

//        return;
//      }

//      if (true)
//      {
//        var ajaxToken = "3b52355ba0a296d5d04db8df78660977";

//        var dx = 10;
//        var dy = 10;
//        var cx = 98;
//        var cy = 42;



//        var data = new List<QNode>();

//        for (var y = cy - dy; y <= cy + dy; ++y)
//        {
//          for (var x = cx - dx; x <= cx + dx; ++x)
//          {
//            var map_json_text = 
//                client.Value.UploadValuesAndGetString(server + "ajax.php?cmd=viewTileDetails", new Dictionary<string, string>()
//                 {
//                   {"cmd", "viewTileDetails"},
//                   {"x", x.ToString()},
//                   {"y", y.ToString()},
//                   {"ajaxToken", ajaxToken}
//                 }
//             );
//            var map_data = json_parse(new[] { new QNode(map_json_text) }, qcontext).SelectMany(_ => _).ToArray();
//            var map_data_html_text = map_data.P("response").P("object").P("data").P("object").P("html").QValue().AsString();

//            data.Add(new QNode("cell", new QNode("x", new QNode(x)), new QNode("y", new QNode(y)), new QNode("html", new QNode(map_data_html_text))));
//            //Console.WriteLine(map_data_html_text);
//            //var map_data_html = SoftTech.Html.HtmlHlp2.LoadXElementFromText(map_data_html_text);

//          }
//          Console.WriteLine(y);
//        }
//        qnodeToText(new[] { new QNode("data", data.ToArray()) }, qcontext.MakeNewVar("is-add-separator", new[]{new QNode("true")}))
//          .SelectMany(_=>_)
//          .ToArray()
//          .AsString()
//          .WriteTo("map.qs");

//        //var map_json_text = Cache(FileAccess("map-83-9.json"), 
//        //  ()=>
//        //    client.Value.UploadValuesAndGetString("http://ts4.travian.us/ajax.php?cmd=viewTileDetails", new Dictionary<string, string>()
//        //     {
//        //       {"cmd", "viewTileDetails"},
//        //       {"x", "83"},
//        //       {"y", "9"},
//        //       {"ajaxToken", ajaxToken}
//        //     })
//        // );
//        ////var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
//        ////var map_json = serializer.DeserializeObject(map_json_text);
//        //var qcontext = q.QContext();
//        //var map_data = QFunctions3.Compile("json-parse")(new[] { new QNode(map_json_text) }, qcontext).SelectMany(_=>_).ToArray();
//        ////Console.WriteLine(map_data.ToText());
//        //var map_data_html_text = map_data.P("response").P("object").P("data").P("object").P("html").QValue().AsString()
//        //  .WriteTo("map-83-9.html");
//        //Console.WriteLine(map_data_html_text);
//        //var map_data_html = SoftTech.Html.HtmlHlp2.LoadXElementFromText(map_data_html_text);

//        //var map_text = Cache(FileAccess("map.html"), client.WebGet("karte.php?fullscreen=1&zoom=2"));
//        return;
//      }

//      if (true)
//      {
        
//        var troopOverview_text = Cache(FileAccess("troop_overview.html"), client.WebGet("build.php?tt=1&id=39"));

//        var troopOverview_xml = SoftTech.Html.HtmlHlp2.LoadXElementFromText(troopOverview_text);
//        foreach (var div in troopOverview_xml.XPathSelectElements("//div[@id='build']//div[@class='data']"))
//        {
//          Console.WriteLine(div);
//        }
//        return;
//      }

//      if (false)
//      {
//        //var client = Login("grandmart@mail.ru", "htodjo1");
//        var sendTroops_text = client.Value.DownloadString("http://ts4.travian.us/build.php?id=39&tt=2");
//        System.IO.File.WriteAllText("send-troops.html", sendTroops_text);
//        sendTroops_text = sendTroops_text.Replace(@"xmlns=""http://www.w3.org/1999/xhtml""", "");
//        var sendTroops_Xml = SoftTech.Html.HtmlHlp2.LoadXElementFromText(sendTroops_text);
//        var sendTroops_form = sendTroops_Xml.XPathSelectElements("//form").FirstOrDefault();
//        var sendTroops_values = GetValuesFromForm(sendTroops_form);
//        sendTroops_values["t1"] = "1";
//        sendTroops_values["x"] = "88";
//        sendTroops_values["y"] = "17";
//        sendTroops_values["c"] = "4";

//        var sendTroops2_text = client.Value.UploadValuesAndGetString("http://ts4.travian.us/build.php?id=39&tt=2", sendTroops_values)
//          .Replace(@"xmlns=""http://www.w3.org/1999/xhtml""", "");
//        System.IO.File.WriteAllText("send-troops2.html", sendTroops2_text);

//        var sendTroops2_Xml = SoftTech.Html.HtmlHlp2.LoadXElementFromText(sendTroops2_text);
//        var sendTroops2_form = sendTroops2_Xml.XPathSelectElements("//form").FirstOrDefault();
//        var sendTroops2_values = GetValuesFromForm(sendTroops2_form);

//        var sendTroops3_text = client.Value.UploadValuesAndGetString("http://ts4.travian.us/build.php?id=39&tt=2", sendTroops2_values)
//          .Replace(@"xmlns=""http://www.w3.org/1999/xhtml""", "");
//        System.IO.File.WriteAllText("send-troops3.html", sendTroops3_text);
//      }

//    }

//    private static CookiedWebClient Login(string server, string login, string password)
//    {
//      var client = new CookiedWebClient();
//      client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0_1 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A406 Safari/7534.48.3";

//      var values = new Dictionary<string, string>();
//      values["name"] = login;
//      values["password"] = password;
//      values["lowRes"] = "0";
//      values["s1"] = "Login";
//      values["w"] = "1000:700";
//      values["login"] = "1368000608";
//      var text2 = client.UploadValuesAndGetString(server + "dorf1.php", values);
//      return client;
//    }

//    private static Dictionary<string, string> GetValuesFromForm(System.Xml.Linq.XElement form)
//    {
//      if (form == null)
//        return null;

//      var values = new Dictionary<string, string>();
//      Console.WriteLine(form.ToString());
//      Console.WriteLine(form.Attribute_Get("action"));
//      foreach (var input in form.XPathSelectElements(".//input").Concat(form.XPathSelectElements(".//button")))
//      {
//        Console.WriteLine(input.ToString());
//        if (input.Attribute_Get("type") == "radio" && input.Attribute_Get("checked") != "checked")
//          continue;
//        var name = input.Attribute_Get("name");
//        var value = input.Attribute_Get("value");
//        Console.WriteLine("{0}:{1}", name, value);
//        if (name == null)
//          continue;
//        values[name] = value;
//      }
//      return values;
//    }
//  }
//  static class WebClientHlp
//  {
//    public static string UploadValuesAndGetString(this WebClient client, string url, Dictionary<string, string> values)
//    {
//      var _values = new System.Collections.Specialized.NameValueCollection();
//      foreach (var pair in values)
//        _values[pair.Key] = pair.Value;
//      return Encoding.UTF8.GetString(client.UploadValues(url, _values));
//    }
//  }

//  static class TextHlp
//  {
//    public static string WriteTo(this string text, string path)
//    {
//      System.IO.File.WriteAllText(path, text);
//      return text;
//    }
//  }
//}
