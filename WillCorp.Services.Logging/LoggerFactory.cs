using Serilog;
using Serilog.Core;

namespace WillCorp.Services.Logging
{
    public static class LoggerFactory
    {
        private static readonly LoggerConfiguration Configuration;
        private static Logger _logger;

        static LoggerFactory()
        {
            Configuration = new LoggerConfiguration()
                                .ReadFrom.AppSettings()
                                .Enrich.WithProcessId()
                                .Enrich.WithProcessName()
                                .Enrich.FromLogContext();
        }

        public static Logger Create()
        {
            return _logger ?? (_logger = Configuration.CreateLogger());
        }
    }
}
