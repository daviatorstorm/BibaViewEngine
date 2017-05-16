using System;
using System.Threading.Tasks;
using BibaViewEngine.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BibaViewEngine.Router
{
    public class BibaRouter : IBibaRouter
    {
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            var routeName = context.RouteData.Values["component"] as string;
            
            ExecuteRouter(routeName, context.HttpContext);

            return Task.FromResult(context);
        }

        private void ExecuteRouter(string routeName, HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
