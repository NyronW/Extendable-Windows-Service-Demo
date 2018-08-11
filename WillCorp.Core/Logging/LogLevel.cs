namespace WillCorp.Logging
{
    /// <summary>
    /// CHOOSING THE RIGHT LEVEL
    /// Verbose is the noisiest level, generally used only as a last resort when debugging a difficult problem, and rarely (if ever) enabled for a production app. For example, local variables within an algorithm implementation might be logged at this level.
    /// Debug is used for internal system events that are not necessarily observable from the outside, but useful when determining how something happened. For example, the details of requests and responses made to and from external integration points would often be logged at this level.
    /// Information events describe things happening in the system that correspond to its responsibilities and functions. Generally these are the observable actions the system can perform. For example, processing a payment or updating a user's details will be logged at this level.
    /// When service is degraded, endangered, or may be behaving outside of its expected parameters, Warning level events are used. A warning event should only be emitted when the condition is either transient or can be investigated and fixed - use restraint to avoid polluting the log with spurious warnings. For example, slow response times from a critical database would be logged as warnings.
    /// When functionality is unavailable or expectations broken, an Error event is used. For example, receiving an exception when trying to commit a database transaction is an event at this level.
    /// The most critical level, fatal events demand immediate attention. For example, an application failing during startup will log a fatal event. The system should send some sort of notification to technical support when one of these occurs.
    /// </summary>
    public enum LogLevel
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }
}
