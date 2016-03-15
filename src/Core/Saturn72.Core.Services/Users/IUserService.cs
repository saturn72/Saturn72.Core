using System;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.Users
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