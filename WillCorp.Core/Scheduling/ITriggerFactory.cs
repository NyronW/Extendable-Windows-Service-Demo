namespace WillCorp.Scheduling
{
    public interface ITriggerFactory<TTrigger>
    {
        Result<TTrigger> Create(string jobId, string groupId = "DEFAULT");
        bool IsSchedule(string jobId);
    }
}