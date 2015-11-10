using Automation.Core.Configuration;

namespace Automation.Core.Domain.Localization
{
    public class LocalizationSettings : ISettings
    {
        public bool LoadAllLocaleRecordsOnStartup { get; set; }
    }
}
