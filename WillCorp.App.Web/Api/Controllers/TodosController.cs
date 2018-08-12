using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Web.Http;
using WillCorp.App.Web.SignalR.Hubs;
using WillCorp.Core.Entity;
using WillCorp.Logging;

namespace WillCorp.App.Web.Api.Controllers
{
    [RoutePrefix("todos")]
    public class TodosController : HubApiController<NotificationHub, INotificationClient>
    {
        static ConcurrentDictionary<string, Todo> todoItems = new ConcurrentDictionary<string, Todo>();

        private readonly ILogger _logger;

        public TodosController(ILogger logger)
        {
            _logger = logger;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(todoItems.Values);
        }

        [Route("{id}", Name = "GetTodoById")]
        public IHttpActionResult GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) Error("Id is required");

            if (todoItems.TryGetValue(id, out Todo todo))
                return Ok(todo);
            else
                return NotFound();
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(string description)
        {
            if (string.IsNullOrEmpty(description)) return Error("Description is required");

            try
            {
                var todo = new Todo(description);

                if (todoItems.TryAdd(todo.Id, todo))
                {
                    await Hub.Clients.All.Notify(new Notification(todo, $"New task: {todo.Id} created"));
                    return Created("GetTodoById", todo.Id, todo);
                }
                else
                    return Error("Item not created");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return InternalServerError("File not create");
            }
        }

        [Route("")]
        [HttpPut]
        public async Task<IHttpActionResult> Completed(string id, string userName)
        {
            if (string.IsNullOrEmpty(id)) return Error("Id is required");
            if (string.IsNullOrEmpty(userName)) return Error("Username is required");
            if (!todoItems.TryGetValue(id, out Todo todo)) return NotFound();

            var result = todo.MarkAsCompleted(userName);
            if (result.Failure) return InternalServerError(result.Error);

            if (todoItems.TryAdd(todo.Id, todo))
            {
                await Hub.Clients.All.Notify(new Notification(todo, $"Task: {todo.Id} completed"));

                return Ok(todo);
            }
            else
                return Error("Item not created");
        }
    }
}
