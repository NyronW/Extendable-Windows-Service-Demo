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
            if (serviceType == null) return null;

            var service = base.GetService(serviceType);
            if (service != null) return service;

            return (!serviceType.IsAbstract && !serviceType.IsInterface && serviceType.IsClass) ? 
                _container.GetInstance(serviceType) : _container.TryGetInstance(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = _container.GetAllInstances(serviceType).Cast<object>();
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}
