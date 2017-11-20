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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using BibaViewEngine.Models;
using System.Collections.Generic;

namespace BibaViewEngine.Router
{
    public class BibaRouter : IBibaRouter
    {
        private readonly Routes _routes;
        private readonly BibaCompiler _compiler;
        private readonly IServiceProvider _provider;
        private readonly RouterData _data;

        public BibaRouter(Routes routes, BibaCompiler compiler, IServiceProvider services, RouterData data)
        {
            _routes = routes;
            _compiler = compiler;
            _provider = services;
            _data = data;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public async Task RouteAsync(RouteContext context)
        {
            _data.SetRouterData((IDictionary<string, object>)context.RouteData.Values);

            if (context.HttpContext.Request.Path == "/app/start")
            {
                await ExecuteStart(context.HttpContext);
            }
            else
            {
                await ExecuteRouter(context);
            }
        }

        private async Task ExecuteStart(HttpContext context)
        {
            var startComponent = _provider.GetRequiredService<Component>();

            var agregate = _routes.Select(x => new
            {
                Path = x.Path,
                Name = x.Component.Name
            });

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Components = agregate,
                Html = StartCompile(startComponent),
            }, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

        private async Task ExecuteRouter(RouteContext context)
        {
            var originalRoute = context.RouteData.Routers.First(x => x is Route) as Route;
            var route = _routes.FirstOrDefault(x => x.Path.Equals(originalRoute.Name, StringComparison.OrdinalIgnoreCase));
            if (route != null)
            {
                var component = (Component)_provider.GetRequiredService(route.Component);

                await context.HttpContext.Response.WriteAsync(StartCompile(component));
            }
            else
            {
                context.HttpContext.Response.StatusCode = 404;
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
