using System.Collections.Generic;
using Automation.Core.Domain.Sites;

namespace Automation.Core.Services.Sites
{
    public interface IProtalService
    {
        IList<Portal> GetAllPortals();
        Portal GetPortalById(object portalId);
    }
}
