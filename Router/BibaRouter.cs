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
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;

namespace BibaViewEngine.Router
{
    public class BibaRouter : IBibaRouter
    {
        private readonly Routes _routes;
        private readonly BibaCompiler _compiler;
        private readonly IServiceProvider _provider;
        private readonly RouterData _data;
        private readonly IAuthorizationService _authorizationService;
        private readonly CamelCasePropertyNamesContractResolver _contractResolver;

        public BibaRouter(Routes routes, BibaCompiler compiler, IServiceProvider provider, RouterData data, IAuthorizationService authorizationService)
        {
            _routes = routes;
            _compiler = compiler;
            _provider = provider;
            _data = data;
            _authorizationService = authorizationService;
            _contractResolver = new CamelCasePropertyNamesContractResolver();
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
            var component = _provider.GetRequiredService<Component>();

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Html = StartCompile(component),
                Scope = component.Scope
            }, new JsonSerializerSettings { ContractResolver = _contractResolver }));
        }

        private async Task ExecuteRouter(RouteContext context)
        {
            var originalRoute = context.RouteData.Routers.First(x => x is Route) as Route;
            var route = _routes.FirstOrDefault(x => x.Path.Equals(originalRoute.Name, StringComparison.OrdinalIgnoreCase));
            if (route != null)
            {
                var component = (Component)_provider.GetRequiredService(route.Component);

                if (route.Handler != null)
                {
                    var authResult = await _authorizationService.AuthorizeAsync(context.HttpContext.User, new object { }, "BibaScheme");
                    if (!authResult.Succeeded)
                    {
                        context.HttpContext.Response.StatusCode = 401;
                        await context.HttpContext.Response.WriteAsync("Unauthorize");
                        return;
                    }
                }

                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Html = StartCompile(component),
                    Scope = component.Scope
                }, new JsonSerializerSettings { ContractResolver = _contractResolver }));
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
