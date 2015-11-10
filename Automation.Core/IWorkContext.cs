using Automation.Core.Domain.Users;

namespace Automation.Core
{
    /// <summary>
    ///     Work context
    /// </summary>
    public interface IWorkContext
    {
        User CurrentUser { get; set; }
        bool IsAdmin { get; set; }
    }
}