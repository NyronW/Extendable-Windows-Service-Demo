using Quartz;
using Quartz.Spi;
using StructureMap;

namespace WillCorp.App.Scheduler.StructureMap
{
    public class StructureMapJobFactory : IJobFactory
    {
        private readonly IContext context;

        public StructureMapJobFactory(IContext context)
        {
            this.context = context;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = (IJob)context.GetInstance(bundle.JobDetail.JobType);
            return new StructureMapJobDecorator(job);
        }

        public void ReturnJob(IJob job)
        {
            // do nothing
        }
    }
}
