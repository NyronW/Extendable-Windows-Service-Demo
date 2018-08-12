using Microsoft.AspNet.SignalR;
using System;

namespace WillCorp.App.Web.Api.Controllers
{
    public abstract class HubApiController<THub, TClient> : BaseController where THub : Hub<TClient> where TClient : class
    {
        private Lazy<IHubContext<TClient>> hub = new Lazy<IHubContext<TClient>>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub,TClient>()
        );

        protected IHubContext<TClient> Hub
        {
            get { return hub.Value; }
        }
    }
}
