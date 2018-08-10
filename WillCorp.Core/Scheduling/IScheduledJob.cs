namespace WillCorp.Scheduling
{
    /// <summary>
    /// This interface will be implemented by all classes
    /// that will be executed in the scheduler module.
    /// It is basically an abstraction around Quarts.net IJob
    /// interface and it allows changing to a different
    /// scheduler, such as Hangfire, whithout breaking client code
    /// </summary>
    public interface IScheduledJob
    {
        string Id { get; }
        bool HasSchedule { get; }

        string GroupId { get; }
        string Description { get; }
    }
}