using System.Collections.Generic;
using System.Linq;
using Automation.Core.Caching;
using Automation.Core.Data;
using Automation.Core.Domain.Localization;
using Automation.Core.Domain.Logging;

namespace Automation.Core.Services.Localization
{
    public interface ILocalizationService
    {
        string GetResource(string resourceKey, bool logIfNotFound = true, string defaultValue = "",
            bool returnEmptyIfNotFound = false);

        Dictionary<string, string> GetAllResourceValues();
    }

    public class LocalizationService : ILocalizationService
    {
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.lsr.all";
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "Nop.lsr.{0}";
        private readonly ICacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILogger _logger;
        private readonly IRepository<LocaleStringResource> _lsrRepository;

        public LocalizationService(LocalizationSettings localizationSettings, ICacheManager cacheManager, ILogger logger,
            IRepository<LocaleStringResource> lsrRepository)
        {
            _localizationSettings = localizationSettings;
            _cacheManager = cacheManager;
            _logger = logger;
            _lsrRepository = lsrRepository;
        }

        public virtual string GetResource(string resourceKey, bool logIfNotFound = true, string defaultValue = "",
            bool returnEmptyIfNotFound = false)
        {
            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records (we know they are cached)
                var resources = GetAllResourceValues();
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey];
                }
            }
            else
            {
                //gradual loading
                var key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, resourceKey);
                var lsr = _cacheManager.Get(key, () =>
                {
                    var query = from l in _lsrRepository.Table
                        where l.ResourceName == resourceKey
                        select l.ResourceValue;
                    return query.FirstOrDefault();
                });

                if (lsr != null)
                    result = lsr;
            }
            if (string.IsNullOrEmpty(result))
            {
                if (logIfNotFound)
                    _logger.Warning(string.Format("Resource string ({0}) is not found.", resourceKey));

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    result = defaultValue;
                }
                else
                {
                    if (!returnEmptyIfNotFound)
                        result = resourceKey;
                }
            }
            return result;
        }

        public virtual Dictionary<string, string> GetAllResourceValues()
        {
            var key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from l in _lsrRepository.TableNoTracking
                    orderby l.ResourceName
                    select l;
                var locales = query.ToList();
                //format: <name, <id, value>>
                var dictionary = new Dictionary<string, string>();
                foreach (var locale in locales)
                {
                    var resourceName = locale.ResourceName.ToLowerInvariant();
                    if (!dictionary.ContainsKey(resourceName))
                        dictionary.Add(resourceName, locale.ResourceValue);
                }
                return dictionary;
            });
        }
    }
}