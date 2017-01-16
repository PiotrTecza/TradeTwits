using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using Google.Apis.Services;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TradeTwits.Utils
{
    public class UrlShortener
    {
        private static UrlshortenerService service;
        public static string ShortenUrl(string url)
        {
            service = new UrlshortenerService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyBe36J3NJ1hRLH0pxaIQISm-UDR94zqIt8",
                ApplicationName = "TradeTwits"
            });

            return ParseUrlFromMessage(url); ;
        }

        private static string CreateShortURL(string urlToShorten)
        {

            // Shorten URL
            Url response = service.Url.Insert(new Url { LongUrl = urlToShorten }).Execute();

            return response.Id;
        }

        private static string ParseUrlFromMessage(string message)
        {
            List<string> list = new List<string>();

            // regex may need adjustments
            Regex urlRx = new Regex(@"(http|ftp|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase);
            
            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {
                string shortedUrl = CreateShortURL(match.Value);
                message = message.Replace(match.Value, "<a href='" + shortedUrl + "'>" + shortedUrl + "</a>");
            }
            return message;
        }

        private async Task<string> ResolveShortURL(string urlToResolve)
        {
            // Request input
            urlToResolve = "http://goo.gl/hcEg7";

            // Resolve URL
            Url response = await service.Url.Get(urlToResolve).ExecuteAsync();

            return response.LongUrl;
        }
    }
}