using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR.Hubs;
using System.Text.RegularExpressions;

namespace TradeTwits.Hubs
{
    public class AjaxMinMinifier : IJavaScriptMinifier
    {
        public string Minify(string source)
        {
            Regex r = new Regex(@"/\*(.*?)\*/", RegexOptions.Singleline);
            source = r.Replace(source, "");
            return new Minifier().MinifyJavaScript(source);
        }
    }
}