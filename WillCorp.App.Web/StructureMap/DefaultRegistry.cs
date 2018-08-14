using Microsoft.AspNet.SignalR;
using StructureMap;
using WillCorp.App.Web.Model;

namespace WillCorp.App.Web.StructureMap
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();

                x.AddAllTypesOf<IDependencyResolver>();
            });

            For<IDependencyResolver>().Use<SignalRStructureMapResolver>();
            For<TodoDataStore>().Singleton();
        }
    }
}
