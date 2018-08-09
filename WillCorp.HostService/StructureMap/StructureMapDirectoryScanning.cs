using StructureMap;
using StructureMap.Graph;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WillCorp.HostService.StructureMap
{
    public static class StructureMapDirectoryScanning
    {
        public static void ScanPluginDirectory(this IAssemblyScanner scanner, Registry registry)
        {
            try
            {
                var cfg = ConfigurationManager.AppSettings["app:plugin-path"];
                if (string.IsNullOrWhiteSpace(cfg)) return;

                var pluginsDirectory = Path.GetFullPath(cfg);
                if (!Directory.Exists(pluginsDirectory)) return;

                scanner.AssembliesFromPath(pluginsDirectory);
                scanner.LookForRegistries();

                Type structureMapCheck = typeof(IServicePlugin);
                var assemblies = FindAssemblies(pluginsDirectory);

                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetExportedTypes().Where(a => !a.IsAbstract))
                    {
                        if (!structureMapCheck.IsAssignableFrom(type)) continue;

                        var interfaces = type.GetInterfaces();
                        foreach (var item in interfaces)
                        {
                            if (item == structureMapCheck) continue;

                            registry.For(item).Use(type);
                        }

                        var baseType = type.BaseType;
                        if (baseType == null || !baseType.IsAbstract) continue;

                        registry.For(baseType).Use(type);
                    }
                }
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }

        private static IEnumerable<Assembly> FindAssemblies(string assemblyPath)
        {
            var dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories);
            var files = dllFiles;

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.Load(new AssemblyName(name));
                }
                catch (Exception)
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(file);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }
    }
}
