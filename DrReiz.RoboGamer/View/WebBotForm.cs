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

namespace Gamering
{
  public partial class WebBotForm : Form
  {
    public WebBotForm()
    {
      InitializeComponent();

      var appName = System.IO.Path.GetFileName(Application.ExecutablePath);

      Microsoft.Win32.Registry
        .CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true)
        .SetValue(appName, 11000, Microsoft.Win32.RegistryValueKind.DWord);

      this.webBrowser1.ScriptErrorsSuppressed = true;

      this.webBrowser1.Navigate("http://fallenlondon.storynexus.com");
    }

    private void execute_button_Click(object sender, EventArgs e)
    {
      Execute();
    }

    public void Execute()
    {
      System.IO.File.WriteAllText("q.html", ToHNode(this.webBrowser1.Document.Body.DomElement as mshtml.IHTMLDOMNode).ToHtmlText());
    }
    static HObject ToHNode(mshtml.IHTMLDOMNode node)
    {
      if (node == null)
        return null;
      //dynamic x = element;
      //Console.WriteLine(MetaTech.Library.ReflectionExtension.ReflectionHelper._P<object>(element.DomElement, "textContent"));
      //var span = element.DomElement as mshtml.IHTMLDOMNode;
      switch (node.nodeType)
      {
        case 3:
          return new HText(node.nodeValue);
        case 1:
          {
            var element = node as mshtml.IHTMLElement;
            var childs = node.childNodes as System.Collections.IEnumerable;
            return new HElement(element.tagName,
              ToHAttribute("id", element.id),
              ToHAttribute("class", element.className),
              ToHAttribute("src", element.getAttribute("src")),
              //ToHAttribute("style", element.getAttribute("style")),
              childs != null ? childs.OfType<mshtml.IHTMLDOMNode>().Select(child => ToHNode(child)) : null
            );
          }
        default:
          return null;

      }
      ////Console.WriteLine(x.textContent);
      ////mshtml.HTMLTextAreaElement
      //return new HElement(element.TagName, 
      //  ToHAttribute("id", element.Id),
      //  ToHAttribute("name", element.Name),
      //  ToHAttribute("class", element.GetAttribute("class")),
      //  ToHAttribute("style", element.Style),
      //  element.Children.OfType<HtmlElement>().Select(e => ToHElement(e)),
      //  ToHAttribute("type", element.DomElement._f(_=>_.GetType().FullName))
      //);


    }
    static HAttribute ToHAttribute(string name, object value)
    {
      if (value == null || value.ToString_Fair() == "")
        return null;
      return new HAttribute(name, value);
    }
  }
}
