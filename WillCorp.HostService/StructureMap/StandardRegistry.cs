using StructureMap;

namespace WillCorp.HostService.StructureMap
{
    public class StandardRegistry : Registry
    {
        public StandardRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
                x.AssembliesFromApplicationBaseDirectory(
                    a => a.FullName.StartsWith("WillCorp"));

                x.ScanPluginDirectory(this);
            });

            For<IContainer>().Use(c => Program.Container);
        }
    }
}