namespace WillCorp.App
{
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
