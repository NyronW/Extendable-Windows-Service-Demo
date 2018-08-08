using WillCorp.App.Web.Api.ActionResults;
using WillCorp.App.Web.Api.Models;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace WillCorp.App.Web.Api.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public virtual Task HandleAsync(ExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            if (!ShouldHandle(context))
            {
                return Task.FromResult(0);
            }

            return HandleAsyncCore(context, cancellationToken);
        }

        public virtual Task HandleAsyncCore(ExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            HandleCore(context);
            return Task.FromResult(0);
        }

        public virtual void HandleCore(ExceptionHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Result = new HttpActionResult<Envelope>(HttpStatusCode.InternalServerError, Envelope.Error(context.Exception.Message));
        }

        public virtual bool ShouldHandle(ExceptionHandlerContext context)
        {
            return context.ExceptionContext.CatchBlock.IsTopLevel;
        }
    }
}
