using System;
using System.Collections.Generic;
using System.Linq;

namespace BibaViewEngine.Router
{
    public class Routes : List<BibaRoute>
    {
        public static Dictionary<string, string> CreateRoutes(Routes sourceRoutes, Dictionary<string, string> routes = null, string rootRoute = null)
        {
            if (routes == null)
                routes = new Dictionary<string, string>();

            foreach (var route in sourceRoutes)
            {
                if (string.IsNullOrWhiteSpace(rootRoute))
                    routes.Add(route.Path, "c/" + route.Path);
                else
                    routes.Add(string.Join("/", rootRoute, route.Path), string.Join("/", "c", rootRoute, route.Path));

                if (route.Children != null && route.Children.Count > 0)
                    CreateRoutes(route.Children, routes,
                        string.IsNullOrWhiteSpace(rootRoute) ? route.Path : string.Join("/", rootRoute, route.Path));
            }

            return routes;
        }
    }

    public static class RoutesExtensions
    {
        public static BibaRoute FindRoute(this Routes source, string routePath) =>
            source.FirstOrDefault(x => x.Path.Equals(routePath, StringComparison.CurrentCultureIgnoreCase));
    }
}
