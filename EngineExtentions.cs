using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using BibaViewEngine.Middleware;
using BibaViewEngine.Models;
using BibaViewEngine.Router;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BibaViewEngine.Auth;

namespace BibaViewEngine
{
    public static class EngineExtensions
    {
        const string registeredTags = "['footer', 'a', 'abbr', 'acronym', 'address', 'applet', 'area', 'base', 'basefont', 'big', 'blink', 'blockquote', 'body', 'br', 'b', 'button', 'caption', 'center', 'cite', 'code', 'col', 'dfn', 'dir', 'div', 'dl', 'dt', 'dd', 'em', 'font', 'form', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'head', 'hr', 'html', 'img', 'input', 'isindex', 'i', 'kbd', 'link', 'li', 'map', 'marquee', 'menu', 'meta', 'ol', 'option', 'param', 'pre', 'p', 'q', 'samp', 'script', 'select', 'small', 'span', 'strikeout', 'strong', 'style', 'sub', 'sup', 'table', 'td', 'textarea', 'th', 'tbody','thead', 'tfoot', 'title', 'tr', 'tt', 'ul', 'u', 'var', 'nav']";

        public static IApplicationBuilder UseBibaViewEngine(this IApplicationBuilder app)
        {
            var router = app.ApplicationServices.GetRequiredService<IBibaRouter>();
            var routes = app.ApplicationServices.GetRequiredService<Routes>();
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var engineAss = Assembly.Load(new AssemblyName("BibaViewEngine"));
            var resources = engineAss.GetManifestResourceNames();
            foreach (var item in resources)
            {
                var resourceItem = item.Replace("BibaViewEngine.build.", string.Empty);
                using (var stream = engineAss.GetManifestResourceStream(item))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var file = File.Create(Path.Combine(env.WebRootPath, resourceItem)))
                        stream.CopyTo(file);
                }
            }

            var routerBuilder = new RouteBuilder(app, router)
                .MapRoute("Start", "app/start");

            var _routes = Routes.CreateRoutes(routes);

            foreach (var route in _routes)
                routerBuilder.MapRoute(route.Key, route.Value);

            var builtRouter = routerBuilder.Build();
            app.UseRouter(builtRouter);

            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/c"))
                {
                    context.Response.StatusCode = 404;
                    context.Response.WriteAsync("Unknown route");
                }

                return next();
            });

            app.UseMiddleware<BibaMiddleware>();

            return app;
        }

        public static IServiceCollection AddBibaViewEngine<EntryComponent>(this IServiceCollection services,
            Routes routes = null, Action<BibaViewEngineProperties> propsAct = null) where EntryComponent : Component
        {
            var tags = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<string>>(registeredTags);
            tags = tags.Concat(new string[] { "#text", "#comment" });

            var props = new BibaViewEngineProperties();
            propsAct?.Invoke(props);

            var components = new ComponentTypes(Assembly.Load("BibaViewEngine").GetTypes().Where(x => x.BaseType == typeof(Component))
                .Concat(Assembly.GetEntryAssembly().GetTypes().Where(x => x.BaseType == typeof(Component))));

            InitRoutes(out Routes outRoutes, routes);

            foreach (var component in components)
                services.AddTransient(component);

            services.AddRouting();

            services.AddSingleton(outRoutes);
            services.AddSingleton(props);
            services.AddSingleton<BibaViewEngineProperties>();
            services.AddSingleton(new RegistesteredTags(tags));
            services.AddSingleton(components);
            services.AddSingleton(new RouterData());
            services.AddSingleton(new CurrentRoute { Route = new BibaRoute() });

            services.AddTransient<BibaCompiler>();
            services.AddTransient<IBibaRouter, BibaRouter>();
            services.AddTransient<Component, EntryComponent>();
            services.AddScoped<IAuthorizationHandler, BibaAuthorizationHandler>();

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("BibaScheme", builder =>
                {
                    builder.AddRequirements(new DefaultRequirement());
                    builder.Build();
                });
            });

            return services;
        }

        private static void InitRoutes(out Routes outRoutes, Routes routes)
        {
            outRoutes = new Routes();

            if (routes == null)
                routes = outRoutes;
            if (routes.Any(x => x.Component == null))
                throw new Exception("Component property cannot be null");
            if (routes.Any(x => x.Path == null))
                throw new Exception("Path property cannot be empty");

            outRoutes = routes;
        }
    }

    public class DefaultRequirement : IAuthorizationRequirement
    {
    }
}
