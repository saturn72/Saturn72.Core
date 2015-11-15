using System.Collections.Generic;

namespace Saturn72.Core.Configuration
{
    public abstract class SettingsBase:ISettings
    {
        protected SettingsBase()
        {
            RawSettings = new Dictionary<string, string>();
        }
        public IDictionary<string, string> RawSettings { get; protected set; }
    }
}
