using System;
using System.Web;
using System.Web.Security;
using Automation.Core.Domain.Users;
using Automation.Core.Services.Users;
using Automation.Extensions;

namespace Automation.Core.Services.Authentication
{
    public interface IAuthenticationService
    {
        User GetAuthenticatedUser();
        User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket);
    }

    public class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private User _cachedUser;

        public FormsAuthenticationService(HttpContextBase httpContext, IUserService userService)
        {
            _httpContext = httpContext;
            _userService = userService;
        }

        public virtual User GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity) _httpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null && user.Active && !user.Deleted && user.IsRegistered())
                _cachedUser = user;
            return _cachedUser;
        }

        public virtual User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket)
        {
            Guard.NotNull(ticket, () => { throw new ArgumentNullException("ticket"); });

            var userEmail = ticket.UserData;

            return userEmail.HaveValue()
                ? _userService.GetUserByEmail(userEmail)
                : null;
        }
    }
}