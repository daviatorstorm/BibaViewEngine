using System;
using System.Collections.Generic;
using System.Linq;

namespace BibaViewEngine.Router
{
    public class Routes : List<BibaRoute>
    {
    }

    public static class RoutesExtensions
    {
        public static BibaRoute FindRoute(this Routes source, string routePath) =>
            source.First(x => x.Path.Equals(routePath, StringComparison.CurrentCultureIgnoreCase));
    }
}
