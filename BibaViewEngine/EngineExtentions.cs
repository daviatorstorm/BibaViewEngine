using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using BibaViewEngine.Middleware;
using BibaViewEngine.Models;
using BibaViewEngine.Router;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using HtmlAgilityPack;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace BibaViewEngine
{
    public static class EngineExtensions
    {
        const string registeredTags = "['a', 'abbr', 'acronym', 'address', 'applet', 'area', 'base', 'basefont', 'big', 'blink', 'blockquote', 'body', 'br', 'b', 'button', 'caption', 'center', 'cite', 'code', 'col', 'dfn', 'dir', 'div', 'dl', 'dt', 'dd', 'em', 'font', 'form', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'head', 'hr', 'html', 'img', 'input', 'isindex', 'i', 'kbd', 'link', 'li', 'map', 'marquee', 'menu', 'meta', 'ol', 'option', 'param', 'pre', 'p', 'q', 'samp', 'script', 'select', 'small', 'span', 'strikeout', 'strong', 'style', 'sub', 'sup', 'table', 'td', 'textarea', 'th', 'tbody', 'thead', 'tfoot', 'title', 'tr', 'tt', 'ul', 'u', 'var']";

        static IHostingEnvironment _env;
        static BibaViewEngineProperties _props;

        public static IApplicationBuilder UseBibaViewEngine(this IApplicationBuilder app)
        {
            var router = app.ApplicationServices.GetRequiredService<IBibaRouter>();

            var embeddedResource = Assembly.Load(new AssemblyName
            {
                Name = "BibaViewEngine"
            }).GetManifestResourceInfo("BibaViewEngine.Scripts.build.js");

            if (_env.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), _props.OutputPath))
                });
            }
            else
            {
                app.UseStaticFiles();
            }

            var routerBuilder = new RouteBuilder(app, router);

            routerBuilder.MapRoute("Get component", "c/{component}");

            var builtRouter = routerBuilder.Build();

            app.UseRouter(builtRouter);
            app.UseMiddleware<BibaMiddleware>();

            return app;
        }

        public static IServiceCollection AddBibaViewEngine(this IServiceCollection services, BibaViewEngineProperties props = null, IHostingEnvironment env = null)
        {
            _env = env;
            _props = props;

            var ass = Assembly.GetEntryAssembly();
            var engineAss = Assembly.Load(new AssemblyName("BibaViewEngine"));
            var tags = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<string>>(registeredTags);
            tags = tags.Concat(new string[] { "#text", "#comment" });

            var components = new RegisteredComponentsCollection
            {
                components = engineAss.GetTypes().Where(x => x.GetTypeInfo().BaseType == typeof(Component))
            };

            if (props == null)
            {
                props = new BibaViewEngineProperties();
            }

            InitHtmlBuild(props);

            services.AddRouting();

            services.AddSingleton(props);
            services.AddSingleton(ass);
            services.AddSingleton(components);
            services.AddSingleton(new RegistesteredTags(tags));
            services.AddTransient<IBibaRouter, BibaRouter>();

            services.AddTransient<BibaCompiler>();

            return services;
        }

        private static void InitHtmlBuild(BibaViewEngineProperties props)
        {
            var doc = new HtmlDocument();
            doc.Load(File.OpenRead(props.IndexHtml));

            var node = doc.DocumentNode;

            var headNode = node.SelectSingleNode("//head");

            //headNode.InnerHtml = $"<script>{File.ReadAllText(props.RouterBuild)}</script>{headNode.InnerHtml}";

            //File.WriteAllText(props.IndexHtmlBuild, node.OuterHtml);
        }
    }
}
