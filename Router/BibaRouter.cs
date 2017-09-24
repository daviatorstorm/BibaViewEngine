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

        public async Task RouteAsync(RouteContext context)
        {
            var routeName = context.RouteData.Values["component"] as string;

            if (routeName == null)
            {
                routeName = string.Empty;
            }

            if (context.HttpContext.Request.Path == "/app/start")
            {
                await ExecuteStart(context.HttpContext);
            }
            else
            {
                await ExecuteRouter(routeName, context.HttpContext);
            }
        }

        private async Task ExecuteStart(HttpContext context)
        {
            var startComponent = _provider.GetRequiredService<Component>();

            await context.Response.WriteAsync(StartCompile(startComponent));
        }

        private async Task ExecuteRouter(string routeName, HttpContext context)
        {
            var route = _routes.FirstOrDefault(x => x.Path.Equals(routeName, StringComparison.OrdinalIgnoreCase));
            if (route != null)
            {
                var component = (Component)_provider.GetRequiredService(route.Component);

                await context.Response.WriteAsync(StartCompile(component));
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }

        private string StartCompile(Component component)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(component.Template);

            component.HtmlElement = doc.DocumentNode;
            return _compiler.PassValues(component);
        }
    }
}
