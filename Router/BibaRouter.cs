using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BibaViewEngine.Router
{
    public class BibaRouter : IBibaRouter
    {
        private readonly Routes _routes;
        private readonly BibaCompiler _compiler;
        private readonly IServiceProvider _provider;

        public BibaRouter(Routes routes, BibaCompiler compiler, IServiceProvider services)
        {
            _routes = routes;
            _compiler = compiler;
            _provider = services;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            var routeName = context.RouteData.Values["component"] as string;

            if (routeName == null)
            {
                routeName = string.Empty;
            }

            ExecuteRouter(routeName, context.HttpContext);

            return Task.FromResult(context);
        }

        private void ExecuteRouter(string routeName, HttpContext context)
        {
            var route = _routes.FirstOrDefault(x => x.Path.Equals(routeName, StringComparison.OrdinalIgnoreCase));

            if (route != null)
            {
                var component = (Component)_provider.GetRequiredService(route.Component);

                var doc = new HtmlDocument();

                component.HtmlElement = doc.DocumentNode;
                component.HtmlElement.InnerHtml = component.Template;

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
