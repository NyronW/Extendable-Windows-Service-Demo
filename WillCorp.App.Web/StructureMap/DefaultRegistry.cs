using StructureMap;

namespace WillCorp.App.Web.StructureMap
{
    public class DefaultRegistry: Registry
    {
        public DefaultRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
                x.AssembliesFromApplicationBaseDirectory(
                    a => a.FullName.StartsWith("WillCorp"));
            });
        }
    }
}
