using System.Web.Http;

namespace TradeTwits
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute("Search", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional }, new { controller = "Search" });

            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });



            //configuration.MapHttpAttributeRoutes();
            //configuration.EnsureInitialized();
        }
    }
}