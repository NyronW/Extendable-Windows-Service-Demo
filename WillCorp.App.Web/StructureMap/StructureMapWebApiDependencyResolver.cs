using StructureMap;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;

namespace WillCorp.App.Web.StructureMap
{
    public class StructureMapWebApiDependencyResolver : IDependencyResolver
    {
        private IContainer _container;
        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapWebApiDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public StructureMapWebApiDependencyResolver(IContainer container)
        {
            _container = container;

            // Register the HttpControllerActivatorProxy so we can 
            // inject request scoped objects into the nested container
            // before the controller object graph is built.
            _container.Configure(x => x.For<IHttpControllerActivator>()
                .Use<HttpControllerActivatorProxy>());
        }

        /// <summary>
        /// The begin scope.
        /// </summary>
        /// <returns>
        /// The System.Web.Http.Dependencies.IDependencyScope.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            IContainer child = _container.GetNestedContainer();
            return new StructureMapWebApiDependencyScope(child);
        }

        public object GetService(System.Type serviceType)
        {
            if (serviceType == null)
            {
                return null;
            }
            try
            {
                if (serviceType.IsAbstract || serviceType.IsInterface)
                    return _container.TryGetInstance(serviceType);

                return _container.GetInstance(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(System.Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
