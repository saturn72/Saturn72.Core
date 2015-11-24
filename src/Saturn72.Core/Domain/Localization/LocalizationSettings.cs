using Saturn72.Core.Configuration;

namespace Saturn72.Core.Domain.Localization
{
    public class LocalizationSettings : ISettings
    {
        public bool LoadAllLocaleRecordsOnStartup { get; set; }
    }
}
