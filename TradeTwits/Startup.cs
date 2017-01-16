using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Owin;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(TradeTwits.Startup))]
namespace TradeTwits
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //Registering custom HubActivator to make Dependency Injection working with WebApi
            var unityHubActivator = new MvcHubActivator();
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => unityHubActivator);
            app.MapSignalR();
        }

        public class MvcHubActivator : IHubActivator
        {
            public IHub Create(HubDescriptor descriptor)
            {
                return (IHub)DependencyResolver.Current.GetService(descriptor.HubType);
            }
        }
    }
}
