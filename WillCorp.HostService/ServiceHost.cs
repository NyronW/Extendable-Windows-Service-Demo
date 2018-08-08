using WillCorp.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WillCorp.App;

namespace WillCorp.HostService
{
    public class ServiceHost
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IServiceModule> _modules;

        public ServiceHost(ILogger logger, IEnumerable<IServiceModule> modules)
        {
            _logger = logger;
            _modules = modules;
        }

        public void Start()
        {
            var moduleCount = 0;

            foreach (var module in _modules)
            {
                var name = module.GetType().Name;

                using (_logger.AddContext("Module", name))
                {
                    try
                    {
                        _logger.Debug($"Starting {name}");

                        if (module.Start())
                        {
                            moduleCount++;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error("Error occured while starting module", e);
                    }
                }
            }

            _logger.Debug("{ModuleCount} of {ModuleTotal} module(s) started sucesfully", moduleCount, _modules.Count());
            _logger.Information("ServiceHost Started");
        }

        public void Stop()
        {
            foreach (var module in _modules.Where(m => m.Status == ServiceModuleStatus.Started))
            {
                var name = module.GetType().Name;

                using (_logger.AddContext("Module", name))
                {
                    try
                    {
                        _logger.Debug($"Stopping {name}");

                        module.Stop();
                    }
                    catch (Exception e)
                    {
                        _logger.Error("Error occured while stopping module", e);
                    }
                }
            }

            _logger.Information("ServiceHost Stopped");
        }

        public void Shutdown()
        {
            _logger.Information("ServiceHost shutting down");
        }

    }
}
