using Quartz;
using System.Threading.Tasks;

namespace WillCorp.App.Scheduler.StructureMap
{
    public class StructureMapJobDecorator : IJob
    {
        private readonly IJob job;

        public StructureMapJobDecorator(IJob job)
        {
            this.job = job;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return job.Execute(context);
        }
    }
}
