using WillCorp.Configuration;
using WillCorp.Logging;
using Microsoft.Owin.Hosting;
using StructureMap;
using System;

namespace WillCorp.App.Web
{
    public class WebModule : ServiceModuleBase, IServicePlugin
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRepository _configuration;

        private IDisposable _server;
        private string _baseAddress;
        private static IContainer _container { get; set; }

        public WebModule(ILogger logger, IContainer container, IConfigurationRepository configuration)
        {
            _logger = logger;
            _container = container;
            _configuration = configuration;

            _baseAddress = _configuration.GetConfigurationValue("web:endpoint", "");
        }

        public static IContainer Container => _container;

        protected override bool OnStart()
        {
            if (string.IsNullOrWhiteSpace(_baseAddress))
            {
                _logger.Warn("Webhost endpoint not set, web module will not be started");
                return false;
            }

            _server = WebApp.Start<Startup>(url: _baseAddress);
            _logger.Verbose("Webhost running at {endpoint}", _baseAddress);

            return true;
        }

        protected override bool OnStop()
        {
            _server?.Dispose();
            return true;
        }
    }
}
