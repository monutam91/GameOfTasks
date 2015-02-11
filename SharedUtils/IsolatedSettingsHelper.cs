using System;
using System.Collections.Generic;
using Windows.Storage;

namespace GameOfTasks
{
    /// <summary>
    /// A helper class to IsolatedStorageSettings
    /// </summary>
    public static class IsolatedSettingsHelper
    {
        /// <summary>
        /// Get the value of an IsolatedStorageSetting
        /// If the key isn't exists, then it creates it andd add it a default value
        /// <remarks>
        /// The type you using is have to be registered otherwise an exception will be thrown!
        /// </remarks>
        /// </summary>
        /// <typeparam name="TReturnType">The type which we try to convert the retrieved value</typeparam>
        /// <param name="key">
        /// The key of the setting
        /// <remarks>
        /// All entry keys should be registered in Settings class
        /// </remarks>
        /// </param>
        /// <returns>The saved value of the settings entry or the default value if the entry doesn't exists!</returns>
        public static TReturnType GetSettingsValue<TReturnType>(string key)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey(key)) 
                return (TReturnType) roamingSettings.Values[key];
            if (RegisteredTypesAndDefaults.ContainsKey(typeof(TReturnType)))
            {
                roamingSettings.Values.Add(new KeyValuePair<string, object>(key, RegisteredTypesAndDefaults[typeof(TReturnType)]));
            }
            else
            {
                throw new InvalidOperationException("The type you asking is not Registered!");
            }
            return (TReturnType) RegisteredTypesAndDefaults[typeof (TReturnType)];
        }

        /// <summary>
        /// Set or add a value in IsolatedSetting
        /// <remarks>
        /// The type of the value and also its default value should have registered before used!
        /// Also the key should be registered as a constant in the Settings class!
        /// </remarks>
        /// </summary>
        /// <param name="key">The key of the Setting value</param>
        /// <param name="value">The value which will be setted</param>
        public static void SetSettingsValue(string key, object value)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey(key))
            {
                roamingSettings.Values[key] = value;
            }
            else
            {
                roamingSettings.Values.Add(key, value);
            }
        }

        private static readonly Dictionary<Type, object> RegisteredTypesAndDefaults = new Dictionary<Type, object>
        {
            {typeof (TimeSpan), TimeSpan.FromHours(1)},
            {typeof(bool), false}
        };
    }

    /// <summary>
    /// The Key values that are contained by IsolatedSettings
    /// </summary>
    public class Settings
    {
        public const string EarlyNotification = "EarlyNotification";
        public const string EnableEarlyNotification = "EnableEarlyNotification";
    }
}
