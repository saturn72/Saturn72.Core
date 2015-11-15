using Saturn72.Core.Configuration;

namespace Saturn72.Core.Domain.Users
{
    public class UserSettings:ISettings
    {
        public bool ShoulsStoreLastVisitedPage { get; set; }
    }
}
