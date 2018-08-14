using System;
using System.Threading.Tasks;
using System.Web.Http;
using WillCorp.App.Web.Model;
using WillCorp.App.Web.SignalR.Hubs;
using WillCorp.Logging;

namespace WillCorp.App.Web.Api.Controllers
{
    [RoutePrefix("todos")]
    public class TodosController : HubApiController<NotificationHub, INotificationClient>
    {
        private readonly ILogger _logger;
        private readonly TodoDataStore _store;

        public TodosController(ILogger logger, TodoDataStore store)
        {
            _logger = logger;
            _store = store;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_store.Get());
        }

        [Route("{id}", Name = "GetTodoById")]
        public IHttpActionResult GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) Error("Id is required");

            var todo = _store.GetById(id);
            if(todo == null) return NotFound();

            return Ok(todo);
        }

        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]string description)
        {
            if (string.IsNullOrEmpty(description)) return Error("Description is required");

            try
            {
                var result = _store.Add(description);
                
                if (result.Success)
                {
                    var todo = result.Value;

                    await Hub.Clients.All.addTodo(todo);
                    return Created("GetTodoById", new { id = todo.Id }, todo);
                }
                else
                    return Error(result.Error);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return InternalServerError("File not create");
            }
        }
    }
}
