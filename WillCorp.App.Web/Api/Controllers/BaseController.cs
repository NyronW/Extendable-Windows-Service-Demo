using System;
using System.Net;
using System.Web.Http;
using WillCorp.App.Web.Api.ActionResults;
using WillCorp.App.Web.Api.Models;

namespace WillCorp.App.Web.Api.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected new IHttpActionResult Ok()
        {
            return base.Ok(Envelope.Ok());
        }

        protected new IHttpActionResult Ok<T>(T result)
        {
            return base.Ok(Envelope.Ok(result));
        }

        protected IHttpActionResult Created<T, I>(string routeNme, I id, T result)
        {
            return base.CreatedAtRoute(routeNme, id, Envelope.Ok(result));
        }

        protected IHttpActionResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        protected IHttpActionResult Error(string errorMessage)
        {
            return new HttpActionResult<Envelope>(HttpStatusCode.BadRequest, Envelope.Error(errorMessage));
        }

        protected new IHttpActionResult InternalServerError(Exception exception)
        {
            return new HttpActionResult<Envelope>(HttpStatusCode.InternalServerError, Envelope.Error(exception.Message));
        }

        protected IHttpActionResult InternalServerError(string errorMessage)
        {
            return new HttpActionResult<Envelope>(HttpStatusCode.InternalServerError, Envelope.Error(errorMessage));
        }
    }
}
