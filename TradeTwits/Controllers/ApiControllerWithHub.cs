using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Web.Http;

namespace TradeTwits.Controllers
{
    public abstract class ApiControllerWithHub<THub> : ApiController
        where THub : IHub
    {
        Lazy<IHubContext> hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
        );

        protected IHubContext Hub
        {
            get { return hub.Value; }
        }
    }
}