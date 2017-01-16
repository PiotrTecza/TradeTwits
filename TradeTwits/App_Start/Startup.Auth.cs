using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Owin.Security.Providers.GooglePlus;

namespace TradeTwits
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            app.UseFacebookAuthentication(
               appId: "641640445932136",
               appSecret: "6a346a86f9157d489d28362dbbeb43c8");

            app.UseGooglePlusAuthentication(
                clientId: "128991782224-uftnrb5lqrb5vv7o8off1v39hdtvbi3l.apps.googleusercontent.com",
                clientSecret: "Cx0zvJcpTxJ9OUNxFnkpwSJl");

            app.UseTwitterAuthentication(
               consumerKey: "wjTcQyCxvOotSrj9RSX1oSBGi",
               consumerSecret: "eNpbRfJP8z1mwzOQerFdIBc44fzpHJPffMtb0gbLiuJEPOYYHf");
        }
    }
}