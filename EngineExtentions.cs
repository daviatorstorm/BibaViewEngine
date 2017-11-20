using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using BibaViewEngine.Middleware;
using BibaViewEngine.Models;
using BibaViewEngine.Router;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace BibaViewEngine
{
    public static class EngineExtensions
    {
        const string registeredTags = "['footer', 'a', 'abbr', 'acronym', 'address', 'applet', 'area', 'base', 'basefont', 'big', 'blink', 'blockquote', 'body', 'br', 'b', 'button', 'caption', 'center', 'cite', 'code', 'col', 'dfn', 'dir', 'div', 'dl', 'dt', 'dd', 'em', 'font', 'form', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'head', 'hr', 'html', 'img', 'input', 'isindex', 'i', 'kbd', 'link', 'li', 'map', 'marquee', 'menu', 'meta', 'ol', 'option', 'param', 'pre', 'p', 'q', 'samp', 'script', 'select', 'small', 'span', 'strikeout', 'strong', 'style', 'sub', 'sup', 'table', 'td', 'textarea', 'th', 'tbody', 'thead', 'tfoot', 'title', 'tr', 'tt', 'ul', 'u', 'var', 'nav']";

        public static IApplicationBuilder UseBibaViewEngine(this IApplicationBuilder app)
        {
            var router = app.ApplicationServices.GetRequiredService<IBibaRouter>();
            var routes = app.ApplicationServices.GetRequiredService<Routes>();
            var props = app.ApplicationServices.GetRequiredService<BibaViewEngineProperties>();
            var engineAss = Assembly.Load(new AssemblyName("BibaViewEngine"));
            var applicationRoot = Directory.GetCurrentDirectory();
            var resources = engineAss.GetManifestResourceNames();
            foreach (var item in resources)
            {
                var resourceItem = item.Replace("BibaViewEngine.build.", string.Empty);
                using (var stream = engineAss.GetManifestResourceStream(item))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var file = File.Create(Path.Combine(props.LibRoot, resourceItem)))
                    {
                        stream.CopyTo(file);
                    }
                }
            }

            var routerBuilder = new RouteBuilder(app, router)
                .MapRoute("Start", "app/start");

            foreach (var route in routes)
            {
                routerBuilder.MapRoute(route.Path, "c/" + route.Path);
            }

            var builtRouter = routerBuilder.Build();

            app.UseRouter(builtRouter);

            app.UseMiddleware<BibaMiddleware>();

            return app;
        }

        public static IServiceCollection AddBibaViewEngine<EntryComponent>(this IServiceCollection services,
            Routes routes = null, BibaViewEngineProperties props = null) where EntryComponent : Component
        {
            var tags = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<string>>(registeredTags);
            tags = tags.Concat(new string[] { "#text", "#comment" });
            Routes outRoutes;

            if (props == null)
            {
                props = new BibaViewEngineProperties();
            }

            var components = new ComponentTypes(Assembly.Load("BibaViewEngine").GetTypes().Where(x => x.BaseType == typeof(Component))
                .Concat(Assembly.GetEntryAssembly().GetTypes().Where(x => x.BaseType == typeof(Component))));

            InitRoutes(out outRoutes, routes);

            foreach (var component in components)
            {
                services.AddTransient(component);
            }

            services.AddRouting();

            services.AddSingleton(outRoutes);
            services.AddSingleton(props);
            services.AddSingleton<BibaViewEngineProperties>();
            services.AddSingleton(new RegistesteredTags(tags));
            services.AddSingleton(components);
            services.AddSingleton(new RouterData());

            services.AddTransient<BibaCompiler>();
            services.AddTransient<IBibaRouter, BibaRouter>();
            services.AddTransient<Component, EntryComponent>();

            return services;
        }

        private static void InitRoutes(out Routes outRoutes, Routes routes)
        {
            outRoutes = new Routes();

            if (routes == null)
            {
                routes = outRoutes;
            }
            if (routes.Any(x => x.Component == null))
            {
                throw new Exception("Component property cannot be null");
            }
            if (routes.Any(x => x.Path == null))
            {
                throw new Exception("Path property cannot be empty");
            }

            outRoutes = routes;
        }
    }
}
