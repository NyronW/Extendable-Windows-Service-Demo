using WillCorp.Configuration;
using WillCorp.Logging;

namespace WillCorp.App.Importer
{
    public class ImporterModule : ServiceModuleBase, IServicePlugin
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRepository _configuration;
        private readonly DirectoryMonitor _directoryMonitor;

        private string _directory;
        private string _endpoint;

        public ImporterModule(ILogger logger, IConfigurationRepository configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _directory = configuration.GetConfigurationValue(ConfigurationKeys.ImportDirectory, string.Empty);
            _endpoint = configuration.GetConfigurationValue(ConfigurationKeys.WebEndpoint, string.Empty);

            _directoryMonitor = new DirectoryMonitor(_logger, _endpoint, _directory);
        }

        protected override bool OnStart()
        {
            if (string.IsNullOrWhiteSpace(_directory))
            {
                _logger.Warn("File pickup directory not configured, module will not be started");
                return false;
            }
            _directoryMonitor.Start();
            _logger.Verbose("Importer module running");

            return true;
        }

        protected override bool OnStop()
        {
            _directoryMonitor?.Stop();
            return true;
        }
    }
}
