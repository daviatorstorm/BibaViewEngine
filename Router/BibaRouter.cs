using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BibaViewEngine.Router
{
    public class BibaRouter : IBibaRouter
    {
        private readonly Routes _routes;
        private readonly BibaCompiler _compiler;

        public BibaRouter(Routes routes, BibaCompiler compiler)
        {
            _routes = routes;
            _compiler = compiler;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            var routeName = context.RouteData.Values["component"] as string;

            if(routeName == null)
            {
                routeName = string.Empty;
            }

            ExecuteRouter(routeName, context.HttpContext);

            return Task.FromResult(context);
        }

        private void ExecuteRouter(string routeName, HttpContext context)
        {
            var route = _routes.FirstOrDefault(x => x.Path.Equals(routeName, StringComparison.OrdinalIgnoreCase));
            Component component = null;

            if(route != null)
            {
                component = Activator.CreateInstance(route.Component) as Component;

                var doc = new HtmlDocument();
                doc.LoadHtml(component.Template);

                component.HtmlElement = doc.DocumentNode;

                component._compiler = _compiler;

                _compiler.Compile(component);

                context.Response.WriteAsync(component.HtmlElement.InnerHtml);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}
