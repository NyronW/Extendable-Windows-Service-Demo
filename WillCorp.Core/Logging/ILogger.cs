using System;

namespace WillCorp.Logging
{
    public interface ILogger
    {
        void Log(LogItem item);
        IDisposable AddContext<T>(string id, T value);
    }
}
