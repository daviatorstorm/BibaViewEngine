using BibaViewEngine.Compiler;
using BibaViewEngine.Exceptions;
using BibaViewEngine.Extensions;
using BibaViewEngine.Interfaces;
using BibaViewEngine.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BibaViewEngine.Router
{
    public class BibaRouter : IBibaRouter
    {
        private readonly Routes _routes;
        private readonly BibaCompiler _compiler;
        private readonly IServiceProvider _provider;
        private readonly RouterData _data;
        private readonly CurrentRoute _currentRoute;
        private readonly CamelCasePropertyNamesContractResolver _contractResolver;

        public BibaRouter(Routes routes, BibaCompiler compiler, IServiceProvider provider,
            RouterData data, CurrentRoute currentRoute)
        {
            _routes = routes;
            _compiler = compiler;
            _provider = provider;
            _data = data;
            _currentRoute = currentRoute;
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
                await ExecuteStart(context.HttpContext);
            else
                await ExecuteRouter(context);
        }

        private async Task ExecuteStart(HttpContext context)
        {
            var component = _provider.CreateComponent<Component>();

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Html = StartCompile(component),
                component.Scope
            }, new JsonSerializerSettings { ContractResolver = _contractResolver }));
        }

        private async Task ExecuteRouter(RouteContext context)
        {
            context.HttpContext.Request.Path = context.HttpContext.Request.Path.Value.Replace("/c/", "/");

            var routeTree = BuildRouteTree(TemplateParser.Parse(context.HttpContext.Request.Path.Value.Remove(0, 1)).Segments,
                TemplateParser.Parse(PathString.FromUriComponent(
                    new Uri(context.HttpContext.Request.Headers["Referer"].ToString(), UriKind.Absolute)).Value.Remove(0, 1)).Segments);

            var completeTemplate = new RouterResult();
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

        private async Task<RouterResult> CompileRoutes(RouteTree routeTree, RouteContext context, HtmlNode node = null, Component parent = null)
        {
            var component = _provider.CreateComponent(routeTree.Route.Component);
            var _authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

            if (routeTree.Route.Handler != null)
            {
                _currentRoute.Route = routeTree.Route;
                var authResult = await _authorizationService.AuthorizeAsync(context.HttpContext.User, new object { }, "BibaScheme");
                if (!authResult.Succeeded)
                    throw new UnauthorizedAccessException();
            }

            if (!routeTree.Skip)
            {
                if (node == null)
                {
                    node = new HtmlNode(HtmlNodeType.Document, new HtmlDocument(), 0);
                    node.InnerHtml = _compiler.PassValues(component);
                }
                else
                    node.Descendants().FirstOrDefault(
                        x => x.Attributes.Any(a => a.Name.Equals("router-container"))
                    ).InnerHtml = _compiler.PassValues(component);
            }

            if (routeTree.NestedRoute != null)
                return await CompileRoutes(routeTree.NestedRoute, context, node, parent);

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

        private RouteTree BuildRouteTree(IList<TemplateSegment> toSegments, IList<TemplateSegment> fromSegments, RouteTree route = null, Routes routes = null)
        {
            if (fromSegments.Count >= toSegments.Count)
                fromSegments.Clear();

            if (routes == null)
                routes = _routes;

            if (route == null)
            {
                route = new RouteTree();

                if (toSegments.Count() == 0)
                {
                    route.RouteName = "";
                    route.Route = routes.FindRoute(route.RouteName);

                    return route;
                }
            }

            if (toSegments.Count > 0 && fromSegments.Count > 0)
                if (toSegments[0].Parts[0].Text == fromSegments[0].Parts[0].Text)
                    route.Skip = true;
                else
                    fromSegments.Clear();
            else
                fromSegments.Clear();

            try
            {
                route.RouteName = toSegments.First().Parts.First().Text;
                route.Route = routes.FindRoute(route.RouteName);

                toSegments.RemoveAt(0);
                if (fromSegments.Count > 0)
                    fromSegments.RemoveAt(0);
            }
            catch
            {
                throw new RouteNotExistsException(route.RouteName);
            }

            if (toSegments.Count > 0)
            {
                route.NestedRoute = new RouteTree();
                BuildRouteTree(toSegments, fromSegments, route.NestedRoute, route.Route.Children);
            }

            return route;
        }
    }
}
