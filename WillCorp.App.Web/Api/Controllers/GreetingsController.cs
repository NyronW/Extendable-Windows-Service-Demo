using System.Web.Http;
using WillCorp.Logging;

namespace WillCorp.App.Web.Api.Controllers
{
    [RoutePrefix("greetings")]
    public class GreetingsController : BaseController
    {
        private readonly ILogger _logger;

        public GreetingsController(ILogger logger)
        {
            _logger = logger;
        }

        [Route("")]
        public IHttpActionResult Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return Error("Invalid name");

            return Ok($"Hello {name}!");
        }
    }
}
