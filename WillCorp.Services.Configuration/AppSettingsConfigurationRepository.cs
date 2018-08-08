using WillCorp.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace WillCorp.Services.Configuration
{
    public class AppSettingsConfigurationRepository : IConfigurationRepository, IServicePlugin
    {
        public string Id => nameof(AppSettingsConfigurationRepository);

        public T GetConfigurationValue<T>(string key)
        {
            return GetConfigurationValue(key, default(T), true);
        }

        public string GetConfigurationValue(string key)
        {
            return GetConfigurationValue<string>(key);
        }

        public T GetConfigurationValue<T>(string key, T defaultValue)
        {
            return GetConfigurationValue(key, defaultValue, false);
        }

        public string GetConfigurationValue(string key, string defaultValue)
        {
            return GetConfigurationValue<string>(key, defaultValue);
        }

        private T GetConfigurationValue<T>(string key, T defaultValue, bool throwException)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                if (throwException)
                    throw new KeyNotFoundException("AppSettings key " + key + " not found in app.settings file.");
                return defaultValue;
            }
            try
            {
                if (typeof(Enum).IsAssignableFrom(typeof(T)))
                    return (T)Enum.Parse(typeof(T), value);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                if (throwException)
                    throw ex;
                return defaultValue;
            }
        }
    }
}
