namespace WillCorp.App
{
    public interface IServiceModule
    {
        bool Start();
        bool Stop();
        //bool Pause();
        //bool Continue();

        ServiceModuleStatus Status { get; }
    }
}
