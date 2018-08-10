namespace WillCorp.App
{
    /// <summary>
    /// This must be implemented by allow application modules
    /// which will be hosted by the windows service.
    /// </summary>
    public interface IServiceModule
    {
        bool Start();
        bool Stop();
        //bool Pause();
        //bool Continue();

        ServiceModuleStatus Status { get; }
    }
}
