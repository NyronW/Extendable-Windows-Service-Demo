using System;
using System.IO;
using System.Threading.Tasks;
using Quartz;
using WillCorp.Configuration;
using WillCorp.Logging;
using WillCorp.Scheduling;

namespace WillCorp.Services.Scheduling.Jobs
{
    public class PurgeImportFolderJob : JobBase
    {
        private readonly IConfigurationRepository _configuration;

        protected PurgeImportFolderJob(ITriggerFactory<ITrigger> triggerFactory, ILogger logger, IConfigurationRepository configuration) : base(triggerFactory, logger)
        {
            _configuration = configuration;
        }

        public override string Id => nameof(PurgeImportFolderJob);

        public override string Description => "Deletes all files in the import directory";

        protected override Task DoWorkAsync(IJobExecutionContext context)
        {
            var directory = _configuration.GetConfigurationValue(ConfigurationKeys.ImportDirectory, string.Empty);
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) return Task.CompletedTask;

            var files = Directory.GetFiles(directory, $"*.txt");

            try
            {
                var today = DateTime.Now;

                foreach (var file in files)
                {
                    //only delete files older than 20 minutes
                    if ((File.GetLastWriteTime(file) - today).TotalMinutes > 5) continue;
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error deleting temp file", e);
            }

            return Task.CompletedTask;
        }
    }
}
