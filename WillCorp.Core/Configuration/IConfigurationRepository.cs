namespace WillCorp.Configuration
{
    public interface IConfigurationRepository
    {
        T GetConfigurationValue<T>(string key);
        string GetConfigurationValue(string key);
        T GetConfigurationValue<T>(string key, T defaultValue);
        string GetConfigurationValue(string key, string defaultValue);
    }

    public static class ConfigurationKeys
    {
        public static string ImportDirectory = "import:pickup-directory";
        public static string WebEndpoint = "web:endpoint";
    }
}
