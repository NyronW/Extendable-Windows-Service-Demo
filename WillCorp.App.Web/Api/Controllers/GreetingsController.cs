using System.Web.Http;

namespace WillCorp.App.Web.Api.Controllers
{
    [RoutePrefix("greetings")]
    public class GreetingsController : BaseController
    {
        [Route("")]
        public IHttpActionResult Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return Error("Invalid name");

            return Ok($"Hello {name}!");
        }
    }
}
