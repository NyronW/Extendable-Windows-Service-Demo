namespace WillCorp.Scheduling
{
    public interface IScheduledJob
    {
        string Id { get; }
        bool HasSchedule { get; }

        string GroupId { get; }
        string Description { get; }
    }
}