using WillCorp.Scheduling;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using StructureMap;
using System.Collections.Specialized;
using WillCorp.Logging;

namespace WillCorp.App.Scheduler.StructureMap
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
                x.AssembliesFromApplicationBaseDirectory(
                    a => a.FullName.StartsWith("WillCorp"));

                x.AddAllTypesOf(typeof(ITriggerFactory<ITrigger>));
                x.AddAllTypesOf<IScheduledJob>();
            });

            For<IJobFactory>().Use(c => new StructureMapJobFactory(c.GetInstance<ILogger>(), c.GetInstance<IContainer>())).Singleton();
            For<ISchedulerFactory>().Use(c => BuildFactory(c)).Singleton();
            For<IScheduler>().Use(c => BuildScheduler(c)).Singleton();
        }

        internal ISchedulerFactory BuildFactory(IContext ctx)
        {
            // Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            return factory;
        }

        internal IScheduler BuildScheduler(IContext ctx)
        {
            var factory = ctx.GetInstance<ISchedulerFactory>();
            IScheduler scheduler = factory.GetScheduler().Result;
            scheduler.JobFactory = ctx.GetInstance<IJobFactory>();

            return scheduler;
        }
    }
}
