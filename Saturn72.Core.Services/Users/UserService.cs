using System;
using System.Linq;
using Saturn72.Core.Caching;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Events;
using Saturn72.Core.Services.Events;

namespace Saturn72.Core.Services.Users
{
    public class UserService : IUserService
    {
        #region Consts

        private const string UserRolesBySystemnameKey = "Saturn72.customerrole.systemname-{0}";

        #endregion


        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<User> _userRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<UserRole> _userRoleRepository;

        #endregion
        
        #region ctor

        public UserService(IEventPublisher eventPublisher, IRepository<User> userRepository, ICacheManager cacheManager,
            IRepository<UserRole> userRoleRepository)
        {
            _eventPublisher = eventPublisher;
            _userRepository = userRepository;
            _cacheManager = cacheManager;
            _userRoleRepository = userRoleRepository;
        }

        #endregion

        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Update(user);

            //event notification
            _eventPublisher.EntityUpdated(user);
        }

        public virtual User GetUserByGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            return _userRepository.Table
                .Where(u => u.UserGuid == userGuid)
                .OrderBy(u => u.Id)
                .FirstOrDefault();
        }

        public virtual User InsertGuestUser()
        {
            var user = new User
            {
                UserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            //add to 'Guests' role
            var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guests);
            if (guestRole == null)
                throw new Saturn72Exception("'Guests' role could not be loaded");
            user.UserRoles.Add(guestRole);

            _userRepository.Insert(user);

            return user;
        }

        public virtual UserRole GetUserRoleBySystemName(string userRoleSystemName)
        {
            if (string.IsNullOrWhiteSpace(userRoleSystemName))
                return null;

            var key = string.Format(UserRolesBySystemnameKey, userRoleSystemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _userRoleRepository.Table
                    orderby cr.Id
                    where cr.SystemName == userRoleSystemName
                    select cr;
                var customerRole = query.FirstOrDefault();
                return customerRole;
            });
        }

        public User GetUserByEmail(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return null;

            var query = from u in _userRepository.Table
                orderby u.Id
                where u.Email == userEmail
                select u;

            return query.FirstOrDefault();
        }
    }
}