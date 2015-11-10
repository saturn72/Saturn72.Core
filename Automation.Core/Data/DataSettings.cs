using Automation.Core.Configuration;

namespace Automation.Core.Data
{
    public class DataSettings : SettingsBase
    {
        /// <summary>
        ///     Database name
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        ///     Data provider
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        ///     Connection string
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        ///     A value indicating whether entered information is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(DataProvider) && !string.IsNullOrEmpty(DataConnectionString);
        }
    }
}