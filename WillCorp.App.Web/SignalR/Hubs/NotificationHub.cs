using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using WillCorp.Core.Entity;
using WillCorp.Logging;

namespace WillCorp.App.Web.SignalR.Hubs
{
    public class NotificationHub: Hub<INotificationClient>
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

        public Task Notify(Notification notification)
        {
            return Clients.All.Notify(notification);
        }
    }

    public interface INotificationClient
    {
        Task Notify(Notification notification);
    }

    public class Notification
    {
        public Notification(Todo todo,string message)
        {
            Todo = todo;
            Message = message;
        }

        public Todo Todo { get; }
        public string Message { get; set; }
    }
}
