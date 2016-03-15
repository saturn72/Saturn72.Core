using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Caching;
using Saturn72.Core.Configuration;
using Saturn72.Core.Data;
using Saturn72.Core.Data.Configuration;

namespace Saturn72.Core.Services.Configuration
{
    public class SettingService : ISettingService
    {
        #region Constants

        /// <summary>
        ///     Key for caching
        /// </summary>
        private const string SettingsAllKey = "Automation.setting.all";

        #endregion

        #region Ctor

        public SettingService(IRepository<Setting> settingRepository, ICacheManager cacheManager)
        {
            _settingRepository = settingRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        public TSettings LoadSetting<TSettings>() where TSettings : ISettings, new()
        {
            var settings = Activator.CreateInstance<TSettings>();

            foreach (var prop in typeof (TSettings).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof (TSettings).Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, loadDefaultValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof (string)))
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = CommonHelper.GetCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }

        /// <summary>
        ///     Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="loadDefaultValueIfNotFound">
        ///     A value indicating whether a shared (for all stores) value should be loaded if
        ///     a value specific for a certain is not found
        /// </param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T),
            bool loadDefaultValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault();

                //load shared value?
                if (setting == null && loadDefaultValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault();

                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }

            return defaultValue;
        }

        #region Utilities

        /// <summary>
        ///     Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            //cache
            var key = string.Format(SettingsAllKey);
            return _cacheManager.Get(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from s in _settingRepository.TableNoTracking
                    orderby s.Name
                    select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public object Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        #endregion

        #region Fields

        private readonly IRepository<Setting> _settingRepository;
        private readonly ICacheManager _cacheManager;

        #endregion
    }
}