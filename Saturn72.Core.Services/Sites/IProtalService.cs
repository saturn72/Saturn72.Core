using System.Collections.Generic;
using Saturn72.Core.Domain.Sites;

namespace Saturn72.Core.Services.Sites
{
    public interface IProtalService
    {
        IList<Portal> GetAllPortals();
        Portal GetPortalById(object portalId);
    }
}
