using WillCorp.Configuration;
using WillCorp.Logging;
using WillCorp.Scheduling;
using Quartz;
using System.Text.RegularExpressions;

namespace WillCorp.Services.Scheduling
{
    public class CronTriggerFactory : ITriggerFactory<ITrigger>, IServicePlugin
    {
        private const string regex = @"^\s*($|#|\w+\s*=|(\?|\*|(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?(?:,(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?)*)\s+(\?|\*|(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?(?:,(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?)*)\s+(\?|\*|(?:[01]?\d|2[0-3])(?:(?:-|\/|\,)(?:[01]?\d|2[0-3]))?(?:,(?:[01]?\d|2[0-3])(?:(?:-|\/|\,)(?:[01]?\d|2[0-3]))?)*)\s+(\?|\*|(?:0?[1-9]|[12]\d|3[01])(?:(?:-|\/|\,)(?:0?[1-9]|[12]\d|3[01]))?(?:,(?:0?[1-9]|[12]\d|3[01])(?:(?:-|\/|\,)(?:0?[1-9]|[12]\d|3[01]))?)*)\s+(\?|\*|(?:[1-9]|1[012])(?:(?:-|\/|\,)(?:[1-9]|1[012]))?(?:L|W)?(?:,(?:[1-9]|1[012])(?:(?:-|\/|\,)(?:[1-9]|1[012]))?(?:L|W)?)*|\?|\*|(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?(?:,(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?)*)\s+(\?|\*|(?:[0-6])(?:(?:-|\/|\,|#)(?:[0-6]))?(?:L)?(?:,(?:[0-6])(?:(?:-|\/|\,|#)(?:[0-6]))?(?:L)?)*|\?|\*|(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?(?:,(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?)*)(|\s)+(\?|\*|(?:|\d{4})(?:(?:-|\/|\,)(?:|\d{4}))?(?:,(?:|\d{4})(?:(?:-|\/|\,)(?:|\d{4}))?)*))$";
        private readonly IConfigurationRepository _configuration;
        private readonly ILogger _logger;

        public CronTriggerFactory(IConfigurationRepository configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Result<ITrigger> Create(string jobId, string groupId = "DEFAULT")
        {
            var key = $"scheduler:cron-expression-{jobId.ToLower()}";
            var cronExpression = _configuration.GetConfigurationValue(key, "");

            if (string.IsNullOrEmpty(cronExpression))
            {
                return Result.Fail<ITrigger>($"Missing configuration (appSettings) entry: {key}");
            }

            if (!Regex.IsMatch(cronExpression, regex))
            {
                return Result.Fail<ITrigger>($"Invalid configuration (appSettings) value for key: {key}. Value is not a valid cron expression");
            }

            var trigger = TriggerBuilder.Create()
                .ForJob(jobId, groupId)
                .WithIdentity(jobId, groupId)
                .WithCronSchedule(cronExpression.Trim(), x => x
                    .WithMisfireHandlingInstructionFireAndProceed())
                .Build();

            return Result<ITrigger>.Ok(trigger);
        }

        public bool IsSchedule(string jobId)
        {
            var key = $"scheduler:cron-expression-{jobId.ToLower()}";
            var cronExpression = _configuration.GetConfigurationValue(key, "");

            if (string.IsNullOrEmpty(cronExpression)) return false;

            return Regex.IsMatch(cronExpression, regex);
        }
    }
}
