using Saturn72.Core.Domain.Users;

namespace Saturn72.Core
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