using WillCorp.Logging;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace WillCorp.App.Web.Middleware
{
    /// <summary>
    /// This middle will be placed at the beginning of the OWIN 
    /// execution pipeline and will call the other middleware such as wen api, signalr etc.
    /// If there is any unhandled exceptions from any of the middleware that is invoked after this
    /// it will be caught and handled in the invoke method belowa.
    /// </summary>
    public class GlobalExceptionMiddleware : OwinMiddleware
    {
        public GlobalExceptionMiddleware(OwinMiddleware next) : base(next)
        {

        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                var logger = WebModule.Container.GetInstance<ILogger>();
                logger?.Error("Unhandle error occured in OWN middleware: {middleware}", ex, Next.GetType().FullName);
            }
        }
    }
}
