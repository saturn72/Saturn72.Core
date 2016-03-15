using System.Web.Routing;

namespace Saturn72.Core.Web.Routes
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}