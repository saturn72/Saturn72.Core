using System;
using Automation.Core.Domain.Users;

namespace Automation.Core.Services.Users
{
    public interface IUserService
    {
        void UpdateUser(User user);
        User GetUserByGuid(Guid userGuid);
        User InsertGuestUser();
        UserRole GetUserRoleBySystemName(string userRoleSystemName);
        User GetUserByEmail(string userEmail);
    }
}