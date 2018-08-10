namespace WillCorp.App
{
    /// <summary>
    /// Provides a basic implememtation of the IServiceModule interface
    /// so that inheriting classes don't need to include the code to update
    /// its' status propery explicity when they are started or stopped
    /// </summary>
    public abstract class ServiceModuleBase : IServiceModule
    {
        protected ServiceModuleStatus _status = ServiceModuleStatus.Stopped;

        public virtual ServiceModuleStatus Status => _status;
        protected abstract bool OnStart();
        protected abstract bool OnStop();

        public bool Start()
        {
            var success = OnStart();

            if (success)
                _status = ServiceModuleStatus.Started;

            return success;
        }


        public bool Stop()
        {
            var success = OnStop();

            if (success)
                _status = ServiceModuleStatus.Stopped;

            return success;
        }
    }
}
