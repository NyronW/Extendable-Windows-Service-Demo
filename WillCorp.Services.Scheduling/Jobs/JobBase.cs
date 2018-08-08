using WillCorp.Logging;
using WillCorp.Scheduling;
using Quartz;
using System.Threading.Tasks;

namespace WillCorp.Services.Scheduling.Jobs
{
    public abstract class JobBase : IScheduledJob, IJob
    {
        private readonly ITriggerFactory<ITrigger> _triggerFactory;
        protected readonly ILogger _logger;

        protected JobBase(ITriggerFactory<ITrigger> triggerFactory, ILogger logger)
        {
            _triggerFactory = triggerFactory;
            _logger = logger;
        }

        public bool HasSchedule => _triggerFactory.IsSchedule(Id);

        public async Task Execute(IJobExecutionContext context)
        {
            using (_logger.AddContext("Job", Id))
            {
                _logger.Information("Executing Job");
                try
                {
                    await DoWorkAsync(context);
                    _logger.Information("Job executed");
                }
                catch (System.Exception e)
                {
                    _logger.Error(e);
                }
            }
        }

        protected abstract Task DoWorkAsync(IJobExecutionContext context);

        public abstract string Id { get; }

        public virtual string GroupId => "DEFAULT";

        public abstract string Description { get; }
    }
}