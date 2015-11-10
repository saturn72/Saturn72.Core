using Automation.Core.Domain.Sites;

namespace Automation.Core
{
    public interface IPortalContext
    {
        Portal CurrentPortal { get; set; }
    }
}
