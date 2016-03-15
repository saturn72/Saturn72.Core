using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Saturn72.Core.Web.Routes
{
    /// <summary>
    /// This object should handle laocalized route mapping.
    /// this is not implemented at this point
    /// </summary>
    public static class RouteExtensions
    {
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults,
            object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var route = new Route(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens["Namespaces"] = namespaces;
            }

            routes.Add(name, route);

            return route;
        
        }

        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults,
            string[] namespaces)
        {
            return MapLocalizedRoute(routes, name, url, defaults, null /* constraints */, namespaces);
        }
    }
}