using Microsoft.AspNet.SignalR;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;

namespace WillCorp.App.Web.StructureMap
{
    public static class Extensions
    {
        #region StructureMap integration with Web Api 
        public static void UseStructureMap(this HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new StructureMapWebApiDependencyResolver(container);
        }

        public static T GetService<T>(this IDependencyScope scope)
        {
            return (T)scope.GetService(typeof(T));
        }

        public static T GetService<T>(this HttpRequestMessage message)
        {
            return message.GetDependencyScope().GetService<T>();
        }

        public static T GetService<T>(this HttpActionExecutedContext context)
        {
            return context.Request.GetDependencyScope().GetService<T>(context,
                context.ActionContext, context.Response);
        }

        public static T GetService<T>(this HttpActionContext context)
        {
            return context.Request.GetDependencyScope().GetService<T>(context,
                context.ActionDescriptor, context.ControllerContext,
                context.ModelState);
        }

        private static T GetService<T>(this IDependencyScope scope, params object[] arguments)
        {
            var container = scope.GetService<IContainer>();
            var explicitArguments = new ExplicitArguments();
            arguments.ForEach(x => explicitArguments.Set(x.GetType(), x));
            return container.GetInstance<T>(explicitArguments);
        }

        private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
        }
        #endregion

        #region StructuteMap integration with SignalR
        public static void UseStructureMap(this HubConfiguration config, IContainer container)
        {
            config.Resolver = new SignalRStructureMapResolver(container);
        }
        #endregion
    }
}
