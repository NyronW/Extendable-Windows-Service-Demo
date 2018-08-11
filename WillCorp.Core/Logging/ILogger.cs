using System;

namespace WillCorp.Logging
{
    /// <summary>
    /// This interface provide a simply contract to implement logging
    /// The AddContext methos is very useful when implement structure 
    /// logging
    /// </summary>
    public interface ILogger
    {
        void Log(LogItem item);
        IDisposable AddContext<T>(string id, T value);
    }
}
