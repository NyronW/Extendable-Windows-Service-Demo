using WillCorp.HostService.StructureMap;
using StructureMap;
using Topshelf;
using Topshelf.StructureMap;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using WillCorp.Logging;

namespace WillCorp.HostService
{
    class Program
    {
        public static IContainer Container { get; private set; }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;

            ConfigureContainer();
            InitService();
        }

        private static void InitService()
        {
            var host = HostFactory.New(config =>
            {
                // Replace default factory with StructureMap implementation
                config.UseStructureMap(Container);

                config.Service<ServiceHost>(s =>
                {
                    // Point to use StructureMap
                    s.ConstructUsingStructureMap();
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    s.WhenShutdown(service => service.Shutdown());
                });

                config.RunAsLocalSystem();
                config.StartAutomatically();

                config.EnableShutdown();

                config.EnableServiceRecovery(r =>
                {
                    //you can have up to three of these
                    r.RestartService(0);
                    r.RestartService(0);
                    //the last one will act for all subsequent failures
                    r.RestartService(0);

                    //should this be true for crashed or non-zero exits
                    r.OnCrashOnly();

                    //number of days until the error count resets
                    r.SetResetPeriod(1);
                });

                config.SetServiceName("HostedService");
                config.SetDisplayName("Windows Service Demo");
                config.SetDescription("Demo showing how to create an extensibile windows servce capable of running multiple modules");
            });

            host.Run();
        }

        /// <summary>
        /// Register services here; the Standarf container
        /// will initiate the loading of all plugins and service
        /// modules
        /// </summary>
        private static IContainer ConfigureContainer()
        {
            return Container = new Container(cfg =>
            {
                cfg.AddRegistry<StandardRegistry>();
            }) ?? Container;
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ProcessUnhandledException((Exception)e.ExceptionObject);
        }

        private static void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ProcessUnhandledException(e.Exception);
            e.SetObserved();
        }

        private static void ProcessUnhandledException(Exception ex)
        {
            if (ex is TargetInvocationException)
            {
                ProcessUnhandledException(ex.InnerException);
                return;
            }

            var logger = Container.GetInstance<ILogger>();
            logger?.Fatal(ex);

            if (Debugger.IsAttached)
                Console.Read();
        }
    }
}
