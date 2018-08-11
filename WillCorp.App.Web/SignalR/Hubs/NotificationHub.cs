using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using WillCorp.Logging;

namespace WillCorp.App.Web.SignalR.Hubs
{
    public class NotificationHub: Hub<IFileNotificationClient>
    {
        protected readonly ILogger logger;

        public NotificationHub(ILogger logger)
        {
            this.logger = logger;
        }

        public override Task OnConnected()
        {
            logger.Information("[{0}] Client '{1}' connected.", DateTime.Now.ToString("dd-mm-yyyy hh:MM:ss"), Context.ConnectionId);

            return base.OnConnected();
        }

        public Task Notify(FileNotification notification)
        {
            return Clients.All.Notify(notification);
        }
    }

    public interface IFileNotificationClient
    {
        Task Notify(FileNotification notification);
    }

    public class FileNotification
    {
        public string FileName { get; set; }
        public string Message { get; set; }
    }
}
