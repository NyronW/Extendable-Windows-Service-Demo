using WillCorp.Logging;
using Serilog.Context;
using Serilog.Core;
using System;
using System.Diagnostics;

namespace WillCorp.Services.Logging
{
    public class SerilogLogger : ILogger, IServicePlugin
    {
        private readonly Logger _logger;

        public SerilogLogger()
        {
            _logger = LoggerFactory.Create();
        }

        public IDisposable AddContext<T>(string id, T value)
        {
            return LogContext.PushProperty(id, value);
        }

        public void Log(LogItem item)
        {
            Write(item);
        }

        private void Write(LogItem item)
        {
            switch (item.LogLevel)
            {
                case LogLevel.Verbose:
                    _logger.Verbose(item.Exception.Demystify(), item.Message, item.FormatParameters);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(item.Exception.Demystify(), item.Message, item.FormatParameters);
                    break;
                case LogLevel.Error:
                    _logger.Error(item.Exception.Demystify(), item.Message, item.FormatParameters);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(item.Exception.Demystify(), item.Message, item.FormatParameters);
                    break;
                case LogLevel.Warning:
                    _logger.Warning(item.Exception.Demystify(), item.Message, item.FormatParameters);
                    break;
                default:
                    _logger.Information(item.Exception.Demystify(), item.Message, item.FormatParameters);
                    break;
            }
        }
    }
}
