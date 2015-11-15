using Saturn72.Core.Domain.Sites;

namespace Saturn72.Core
{
    public interface IPortalContext
    {
        Portal CurrentPortal { get; set; }
    }
}
