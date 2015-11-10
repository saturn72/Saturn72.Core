using Automation.Core.Configuration;

namespace Automation.Core.Domain.Users
{
    public class UserSettings:ISettings
    {
        public bool ShoulsStoreLastVisitedPage { get; set; }
    }
}
