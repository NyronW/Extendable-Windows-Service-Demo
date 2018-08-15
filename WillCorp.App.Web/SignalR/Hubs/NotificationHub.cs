using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using WillCorp.App.Web.Model;
using WillCorp.Core.Entity;
using WillCorp.Logging;

namespace WillCorp.App.Web.SignalR.Hubs
{
    public class NotificationHub: Hub<INotificationClient>
    {
        protected readonly ILogger _logger;
        private readonly TodoDataStore _store;

        public NotificationHub(ILogger logger, TodoDataStore store)
        {
            _logger = logger;
            _store = store;
        }

        public override Task OnConnected()
        {
            _logger.Information("Client '{ConnectionId}' connected.", Context.ConnectionId);

            Clients.Client(Context.ConnectionId).notifyUser($"Connection suceeded, your connection id is: {Context.ConnectionId}");

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _logger.Information("Client '{ConnectionId}' disconnected.", Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public void MarkAsDone(TodoModel model)
        {
           var result =  _store.Update(model);

            if(result.Success)
            {
                Clients.Others.markAsDone(model.Id, model.Completed);
            }
        }

        public void MarkAllAsDone()
        {
            var result = _store.SetAllToCompleted();

            if (result.Success)
            {
                Clients.Others.markAllAsDone();
            }
        }
    }

    public interface INotificationClient
    {
        Task addTodo(Todo todo);
        Task markAsDone(string todoId, bool completed);
        Task notifyUser(string message);
        Task markAllAsDone();
    }
}
