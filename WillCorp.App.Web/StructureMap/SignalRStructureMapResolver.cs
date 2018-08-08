using Microsoft.AspNet.SignalR;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WillCorp.App.Web.StructureMap
{
    public class SignalRStructureMapResolver : DefaultDependencyResolver
    {
        private readonly IContainer _container;

        public SignalRStructureMapResolver(IContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            object service;
            if (!serviceType.IsAbstract && !serviceType.IsInterface && serviceType.IsClass)
            {
                // Concrete type resolution
                service = _container.GetInstance(serviceType);
            }
            else
            {
                // Other type resolution with base fallback
                service = _container.TryGetInstance(serviceType) ?? base.GetService(serviceType);
            }
            return service;
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = _container.GetAllInstances(serviceType).Cast<object>();
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}
