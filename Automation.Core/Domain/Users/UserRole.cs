namespace Automation.Core.Domain.Users
{
    public class UserRole:BaseEntity
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public string SystemName { get; set; }
    }
}