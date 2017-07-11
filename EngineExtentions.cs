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
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace BibaViewEngine
{
    public static class EngineExtensions
    {
        const string registeredTags = "['a', 'abbr', 'acronym', 'address', 'applet', 'area', 'base', 'basefont', 'big', 'blink', 'blockquote', 'body', 'br', 'b', 'button', 'caption', 'center', 'cite', 'code', 'col', 'dfn', 'dir', 'div', 'dl', 'dt', 'dd', 'em', 'font', 'form', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'head', 'hr', 'html', 'img', 'input', 'isindex', 'i', 'kbd', 'link', 'li', 'map', 'marquee', 'menu', 'meta', 'ol', 'option', 'param', 'pre', 'p', 'q', 'samp', 'script', 'select', 'small', 'span', 'strikeout', 'strong', 'style', 'sub', 'sup', 'table', 'td', 'textarea', 'th', 'tbody', 'thead', 'tfoot', 'title', 'tr', 'tt', 'ul', 'u', 'var']";

        public static IApplicationBuilder UseBibaViewEngine(this IApplicationBuilder app)
        {
            var router = app.ApplicationServices.GetRequiredService<IBibaRouter>();
            var props = app.ApplicationServices.GetRequiredService<BibaViewEngineProperties>();
            var engineAss = Assembly.Load(new AssemblyName("BibaViewEngine"));

            InitHtmlBuild(engineAss, props);

            var routerBuilder = new RouteBuilder(app, router);

            routerBuilder.MapRoute("Get component", "c/{component?}");

            var builtRouter = routerBuilder.Build();

            app.UseRouter(builtRouter);
            app.UseMiddleware<BibaMiddleware>();

            CSharpScript.EvaluateAsync("1 + 2");

            return app;
        }

        public static IServiceCollection AddBibaViewEngine(this IServiceCollection services, Routes routes = null, BibaViewEngineProperties props = null)
        {
            var engineAss = Assembly.Load(new AssemblyName("BibaViewEngine"));
            ResourceManager rm = new ResourceManager("Component", engineAss);

            var tags = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<string>>(registeredTags);
            tags = tags.Concat(new string[] { "#text", "#comment" });
            Routes outRoutes;

            var components = new RegisteredComponentsCollection
            {
                components = engineAss.GetTypes().Where(x => x.GetTypeInfo().BaseType == typeof(Component))
            };

            if (props == null)
            {
                props = new BibaViewEngineProperties();
            }

            InitRoutes(out outRoutes, routes);

            services.AddRouting();

            services.AddSingleton(components);
            services.AddSingleton(outRoutes);
            services.AddSingleton(props);
            services.AddSingleton<BibaViewEngineProperties>();
            services.AddSingleton(new RegistesteredTags(tags));
            services.AddTransient<IBibaRouter, BibaRouter>();
            services.AddTransient<BibaCompiler>();

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
            else if (routes.Any(x => x.Path == null))
            {
                throw new Exception("Path property cannot be empty");
            }

            outRoutes = routes;
        }

        private static void InitHtmlBuild(Assembly ass, BibaViewEngineProperties props)
        {
            var buildFileStream = ass.GetManifestResourceStream(ass.GetManifestResourceNames()[0]);

            var doc = new HtmlDocument();
            doc.Load(File.OpenRead(props.IndexHtml));

            var node = doc.DocumentNode;
            var headNode = node.SelectSingleNode("//head");

            headNode.InnerHtml = $"{Environment.NewLine}<script>{Environment.NewLine}{new StreamReader(buildFileStream).ReadToEnd()}</script>{Environment.NewLine}{headNode.InnerHtml}";

            File.WriteAllText(props.IndexHtmlBuild, node.OuterHtml);
        }
    }
}
