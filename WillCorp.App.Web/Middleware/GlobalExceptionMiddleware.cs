using WillCorp.Logging;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace WillCorp.App.Web.Middleware
{
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
