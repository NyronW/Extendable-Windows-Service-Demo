using System;
using WillCorp.Logging;
using Quartz.Logging;
using StructureMap;

namespace WillCorp.App.Scheduler
{
    public class CustomLogProvider : ILogProvider
    {
        private readonly IContainer context;

        public CustomLogProvider(IContainer context)
        {
            this.context = context;
        }

        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (func != null)
                {
                    var logger = context.GetInstance<ILogger>();
                    if (logger == null) return false;
                    switch (level)
                    {
                        case Quartz.Logging.LogLevel.Trace:
                            logger.Verbose(func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Debug:
                            logger.Debug(func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Info:
                            logger.Information(func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Warn:
                            logger.Warn(func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Error:
                            logger.Error(func(), exception, parameters);
                            break;
                        case Quartz.Logging.LogLevel.Fatal:
                            logger.Fatal(func(), exception, parameters);
                            break;
                        default:
                            break;
                    }
                }
                return true;
            };
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
}
