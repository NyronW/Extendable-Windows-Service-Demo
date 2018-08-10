using System.Collections.Generic;
using WillCorp.Configuration;
using WillCorp.Core.FileSystem;
using WillCorp.Logging;

namespace WillCorp.App.Importer
{
    public class ImporterModule : ServiceModuleBase, IServicePlugin
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRepository _configuration;
        private readonly DirectoryMonitor _directoryMonitor;

        private string _directory;

        public ImporterModule(ILogger logger, IConfigurationRepository configuration, IEnumerable<FileTransformationBase> transformations)
        {
            _logger = logger;
            _configuration = configuration;

            _directory = configuration.GetConfigurationValue(ConfigurationKeys.ImportDirectory, string.Empty);
            _directoryMonitor = new DirectoryMonitor(_logger, _configuration, transformations, _directory);
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
