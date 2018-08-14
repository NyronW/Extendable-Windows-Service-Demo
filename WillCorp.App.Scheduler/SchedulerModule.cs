using WillCorp.Logging;
using WillCorp.Scheduling;
using Quartz;
using Quartz.Logging;
using StructureMap;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WillCorp.App.Scheduler
{
    public class SchedulerModule : ServiceModuleBase, IServicePlugin
    {
        private readonly CancellationTokenSource _cancel;
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly IEnumerable<IScheduledJob> _jobs;
        private readonly ITriggerFactory<ITrigger> _factory;

        private static IContainer _container { get; set; }
        public static IContainer Container => _container;

        public SchedulerModule(ILogger logger, IScheduler scheduler, IEnumerable<IScheduledJob> jobs, ITriggerFactory<ITrigger> factory, IContainer container)
        {
            _logger = logger;
            _scheduler = scheduler;
            _jobs = jobs;
            _factory = factory;
            _container = container;

            _cancel = new CancellationTokenSource();
            LogProvider.SetCurrentLogProvider(new CustomLogProvider(container));
        }


        protected override bool OnStart()
        {
            if(!_jobs.Any())
            {
                _logger.Warn("No sceduled jobs found, scheduler module will not be started");
                return false;
            }

            Task.Run(() => ExecuteAsync(_cancel.Token));
            _logger.Verbose("Scheduler module running");

            return true;
        }

        protected override bool OnStop()
        {
            _cancel.Cancel();
            return true;
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_scheduler.IsStarted) continue;

                var jobCount = 0;

                try
                {
                    await _scheduler.Start();

                    foreach (var job in _jobs)
                    {
                        using (_logger.AddContext("JobId", job.Id))
                        {
                            if (await _scheduler.CheckExists(new JobKey(job.Id))) continue;

                            var triggerOkorFail = _factory.Create(job.Id, job.GroupId);
                            if (!triggerOkorFail.Success)
                            {
                                _logger.Warn(string.Join(" ", triggerOkorFail.Error));
                                continue;
                            }

                            IJobDetail dtl = JobBuilder.Create(job.GetType())
                                .WithIdentity(job.Id, job.GroupId)
                                .WithDescription(job.Description)
                                .Build();

                            _logger.Verbose($"Scheduling job: {job.Id}");

                            await _scheduler.ScheduleJob(dtl, triggerOkorFail.Value);
                            jobCount++;
                        }
                    }

                    _logger.Information("{JobCount} of {JobTotal} job(s) scheduled successfully", jobCount, _jobs.Count());
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                if (!_scheduler.IsShutdown)
                    await _scheduler.Shutdown(waitForJobsToComplete: true);
            }
        }
    }
}
