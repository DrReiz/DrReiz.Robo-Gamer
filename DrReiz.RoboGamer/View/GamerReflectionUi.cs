using SoftTech.Wui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetaTech.Library;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace Gamering
{
  public partial class GamerReflectionUi : Form
  {
    public GamerReflectionUi()
    {

      var files = System.IO.Directory.GetFiles(ApplicationHlp.MapPath("Analyze"))
        .OrderBy(file => ConvertHlp.ToDouble(System.IO.Path.GetFileNameWithoutExtension(file)))
        .ToArray();

      string selectedFile = null;
      Bitmap selectedBitmap = null;
      Mat[] selectedChannels = null;

      h.Protocol().Register("img",
        (url, context) =>
        {
          var urlPath = url.Trim('/', '\\').Substring((context.ProtocolPrefix + "://").Length);
          var ss = urlPath.Split('.');
          var imgFilename = ss.FirstOrDefault();
          var kind = ss.ElementAtOrDefault(1);
          var imgPath = files.FirstOrDefault(file => System.IO.Path.GetFileNameWithoutExtension(file) == imgFilename);
          switch (kind)
          {
            case "source":
              return System.IO.File.ReadAllBytes(imgPath);
            case "h":
              return selectedChannels.Else_Empty().ElementAtOrDefault(0)._f(_ => _.Resize(0.5).ToBitmap().ToBytes());
            case "s":
              return selectedChannels.Else_Empty().ElementAtOrDefault(1)._f(_ => _.Resize(0.5).ToBitmap().ToBytes());
            case "v":
              return selectedChannels.Else_Empty().ElementAtOrDefault(2)._f(_ => _.Resize(0.5).ToBitmap().ToBytes());
          }
          return null;
        }
        );

      InitializeComponent();


      //var filename = "unknown.bmp";


      //Console.WriteLine(img);



      var points = new CheckPoint[] { };

      new SoftTech.Wui.HDesktopSynchronizer(webBrowser1, () => HView(files, selectedFile, selectedBitmap), json =>
      {
        try
        {
          switch (json.JPath("data", "command").ToString_Fair())
          {
            case "select-file":
              {
                var path = json.JPath("data", "file").ToString_Fair();
                selectedFile = path;

                var img_data = System.IO.File.ReadAllBytes(path);
                var bitmap = (Bitmap)Bitmap.FromStream(new System.IO.MemoryStream(img_data));
                selectedChannels = new Mat(bitmap.ToIplImage(), true).Split();
              }
              break;
            //case "select":
            //  {
            //    switch (json.JPath("event", "type").ToString_Fair())
            //    {
            //      case "mousemove":
            //        x = ConvertHlp.ToInt(json.JPath("event", "clientX"));
            //        y = ConvertHlp.ToInt(json.JPath("event", "clientY"));
            //        if (x != null && y != null && x >= 0 && y >= 0 && x < img.Width && y < img.Height)
            //          color = img.Get<byte>(y.Value, x.Value);
            //        break;
            //      case "mousedown":
            //        {
            //          var _x = ConvertHlp.ToInt(json.JPath("event", "clientX"));
            //          var _y = ConvertHlp.ToInt(json.JPath("event", "clientY"));
            //          if (_x != null && _y != null && _x >= 0 && _y >= 0 && _x < img.Width && _y < img.Height)
            //          {
            //            var _color = bitmap.GetPixel(_x.Value, _y.Value);
            //            points = new[] { new CheckPoint(_x.Value, _y.Value, (uint)_color.ToArgb()) }.Concat(points).Take(10).ToArray();
            //          }
            //        }
            //        break;
            //    }
            //  }
            //  break;
            default:
              Console.WriteLine(json);
              break;
          }
        }
        catch (Exception exc)
        {
          Console.WriteLine(exc);
        }
      });
    }

    static HElement HView(string[] files, string selectedFile, Bitmap selectedBitmap)//(int? x, int? y, int? color, CheckPoint[] points)
    {
      return h.Html
        (
          h.Head
          (
            h.Desktop_Scripts()           
          ),
          h.Body(
            h.style("font-size:80%;"),
            h.Table(
            h.TBody(
              h.Tr
              (
                h.Td
                (
                  h.style("vertical-align:top;"),
                  h.Table(h.TBody
                  (
                    files.Select(file => 
                      h.Tr
                      (
                        h.style(string.Format("cursor:pointer;{0}", file == selectedFile ? "background-color:lightblue;" : null)),
                        h.Td(
                          h.data("file", file),
                          h.data("command", "select-file"),
                          h.onclick(";"),
                          System.IO.Path.GetFileNameWithoutExtension(file)
                        )
                      )
                    )
                  ))
                ),
                h.Td(
                  h.style("vertical-align:top;"),
                  selectedFile != null
                    ? h.Div 
                      (
                        h.Img(h.Attribute("src", string.Format("img://{0}.h", System.IO.Path.GetFileNameWithoutExtension(selectedFile)))),
                        h.Img(h.Attribute("src", string.Format("img://{0}.s", System.IO.Path.GetFileNameWithoutExtension(selectedFile)))),
                        h.Img(h.Attribute("src", string.Format("img://{0}.v", System.IO.Path.GetFileNameWithoutExtension(selectedFile))))
                      )
                    : null,
                  selectedFile != null 
                    ? h.Img(h.Attribute("src", string.Format("img://{0}.source", System.IO.Path.GetFileNameWithoutExtension(selectedFile))))
                    : null
                )
              )
            ))
            //h.Div(DateTime.UtcNow), 
            //h.Img(h.Attribute("src", "img://0 h"),
            // h.Attribute("onmousedown", ";"),
            // h.Attribute("onmouseup", ";"),
            // h.Attribute("onmousemove", ";"),
            // h.data("command", "select")
            //),
            //h.Div(string.Format("{0}:{1}, {2}", x, y, color)),
            //points.Select(point => h.Div(string.Format("{0}, {1}, 0x{2:x8}", point.Point.X, point.Point.Y, (uint)point.Color.ToArgb())))
          )
        );
    }
    static readonly HBuilder h = null;
  }
}
