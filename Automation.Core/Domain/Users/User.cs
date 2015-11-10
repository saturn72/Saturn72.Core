using System;
using System.Collections.Generic;

namespace Automation.Core.Domain.Users
{
    public class User : BaseEntity
    {
        private ICollection<UserRole> _userRoles;
        public string LastIpAddress { get; set; }
        public DateTime LastActivityDateUtc { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            protected set { _userRoles = value; }
        }
    }
}