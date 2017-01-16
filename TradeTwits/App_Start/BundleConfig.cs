using System.Web;
using System.Web.Optimization;

namespace TradeTwits
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-route.js",
                      "~/Scripts/angular-resource.js",
                      "~/Scripts/ui-bootstrap-tpls-0.11.0.min.js",
                      "~/Scripts/app.js",
                      "~/Scripts/Controllers/*.js",
                      "~/Scripts/Services/*.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/angularjs-file-upload").Include(
                        "~/Scripts/es5-shim.min.min.js",
                        "~/Scripts/es5-sham.min.min.js",
                        "~/Scripts/angular-file-upload.min.js",
                        "~/Scripts/bootstrap-filestyle.min.js"                   
                      ));

            bundles.Add(new ScriptBundle("~/bundles/utils").Include(
                      "~/Scripts/moment.min.js",
                      "~/Scripts/jquery.signalR-2.0.3.js",
                      "~/Scripts/jquery.textcomplete.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/TradeTwits.css",
                      "~/Content/jquery.textcomplete.css"));
        }
    }
}
