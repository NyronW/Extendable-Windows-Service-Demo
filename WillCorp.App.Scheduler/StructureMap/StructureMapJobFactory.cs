using Quartz;
using Quartz.Spi;
using StructureMap;
using System;
using WillCorp.Logging;

namespace WillCorp.App.Scheduler.StructureMap
{
    public class StructureMapJobFactory : IJobFactory
    {
        private readonly IContainer _container;
        private readonly ILogger _logger;

        public StructureMapJobFactory(ILogger logger, IContainer container)
        {
            _container = container;
            _logger = logger;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;

            try
            {
                _logger.Verbose("Creating instance of Job '{0}', class={1}", jobDetail.Key, jobType.FullName);

                var job = (IJob)_container.GetInstance(bundle.JobDetail.JobType);
                return new StructureMapJobDecorator(job);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Problem instantiating class {0}", jobType.FullName), ex);
                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
