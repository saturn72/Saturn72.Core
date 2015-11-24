using Saturn72.Core.Configuration;

namespace Saturn72.Core.Services.Configuration
{
    public interface ISettingService
    {
        /// <summary>
        /// Loads settings
        /// </summary>
        /// <typeparam name="TSettings">Settings Type</typeparam>
        /// <returns></returns>
        TSettings LoadSetting<TSettings>() where TSettings : ISettings, new();

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="loadDefaultValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting value</returns>
        T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadDefaultValueIfNotFound = false);
    }
}
