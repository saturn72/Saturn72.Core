using System;
using System.Linq;
using Automation.Extensions;

namespace Automation.Core.Domain.Users
{
    public static class UserExtensions
    {
        public static bool IsInUserRole(this User user, string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            Guard.NotNull(user, () => { throw new ArgumentNullException("user"); });

            Guard.MustFollow(userRoleSystemName.HaveValue(),
                () => { throw new ArgumentNullException("userRoleSystemName"); });

            var result = user.UserRoles
                .FirstOrDefault(ur => (!onlyActiveUserRoles || ur.Active) && (ur.SystemName == userRoleSystemName)) !=
                         null;
            return result;
        }

        public static bool IsRegistered(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Registered, onlyActiveUserRoles);
        }
    }
}