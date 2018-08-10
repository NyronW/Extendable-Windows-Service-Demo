using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using WillCorp.Logging;

namespace WillCorp.App.Web.SignalR.Hubs
{
    public abstract class HubBase: Hub
    {
        protected readonly ILogger logger;

        public HubBase(ILogger logger)
        {
            this.logger = logger;
        }

        public override Task OnConnected()
        {
            logger.Information("[{0}] Client '{1}' connected.", DateTime.Now.ToString("dd-mm-yyyy hh:MM:ss"), Context.ConnectionId);

            return base.OnConnected();
        }
    }
}
