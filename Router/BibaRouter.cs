using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using BibaViewEngine.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;
using Microsoft.AspNetCore.Routing.Template;
using BibaViewEngine.Exceptions;
using Microsoft.Extensions.Primitives;

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
            var refererHeader = PathString.FromUriComponent(
                new Uri((context.HttpContext.Request.Headers["Referer"].ToString()), UriKind.Absolute));
            var originalRoute = context.RouteData.Routers.First(x => x is Route) as Route;
            var routeTree = BuildRouteTree(originalRoute.ParsedTemplate.Segments.Skip(1).ToList());
            context.HttpContext.Request.Path = context.HttpContext.Request.Path.Value.Replace("/c/", "/");

            if (!string.IsNullOrWhiteSpace(refererHeader) &&
                context.HttpContext.Request.Path.StartsWithSegments(refererHeader,
                StringComparison.CurrentCultureIgnoreCase, out PathString newPath))
            {
                context.HttpContext.Request.Path = newPath;
            }

            RouterResult completeTemplate = new RouterResult();
            try
            {
                completeTemplate = await CompileRoutes(routeTree, context);

            }
            catch (UnauthorizedAccessException)
            {
                context.HttpContext.Response.StatusCode = 401;
                await context.HttpContext.Response.WriteAsync("Unauthorize");
            }

            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(completeTemplate,
                new JsonSerializerSettings { ContractResolver = _contractResolver }));
        }

        private async Task<RouterResult> CompileRoutes(RouteTree routeTree, RouteContext context, HtmlNode node = null)
        {
            var component = (Component)_provider.GetRequiredService(routeTree.Route.Component);

            if (routeTree.Route.Handler != null)
            {
                var authResult = await _authorizationService.AuthorizeAsync(context.HttpContext.User, new object { }, "BibaScheme");
                if (!authResult.Succeeded)
                {

                    throw new UnauthorizedAccessException();
                }
            }

            if (node == null)
            {
                node = new HtmlNode(HtmlNodeType.Document, new HtmlDocument(), 0);
                node.InnerHtml = _compiler.PassValues(component);
            }
            else
            {
                var attrName = "router-container";
                var routeContainer = node.Descendants().FirstOrDefault(
                    x => x.Attributes.Any(a => a.Name.Equals(attrName)));
                routeContainer.InnerHtml = _compiler.PassValues(component);
                routeContainer.Attributes.Remove(attrName);
            }

            if (routeTree.NestedRoute != null)
            {
                await CompileRoutes(routeTree.NestedRoute, context, node);
            }

            return new RouterResult
            {
                Html = node.OuterHtml,
                Scope = component.Scope
            };
        }

        private string StartCompile(Component component)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(component.Template);

            component.HtmlElement = doc.DocumentNode;
            return _compiler.PassValues(component);
        }

        private RouteTree BuildRouteTree(IList<TemplateSegment> segments, RouteTree route = null, Routes routes = null)
        {
            if (routes == null)
            {
                routes = _routes;
            }

            if (route == null)
            {
                route = new RouteTree();

                if (segments.Count() == 0)
                {
                    route.RouteName = "";
                    route.Route = routes.FindRoute(route.RouteName);

                    return route;
                }
            }

            try
            {
                route.RouteName = segments.First().Parts.First().Text;
                route.Route = routes.FindRoute(route.RouteName);

                segments.RemoveAt(0);
            }
            catch
            {
                throw new RouteNotExistsException(route.RouteName);
            }

            if (segments.Count > 0)
            {
                route.NestedRoute = new RouteTree();
                BuildRouteTree(segments, route.NestedRoute, route.Route.Children);
            }

            return route;
        }
    }
}
