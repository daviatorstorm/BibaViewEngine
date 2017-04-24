using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using BibaViewEngine.Middleware;
using BibaViewEngine.Models;
using BibaViewEngine.Router;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BibaViewEngine
{
    public static class EngineExtensions
    {
        const string registeredTags = "['a', 'abbr', 'acronym', 'address', 'applet', 'area', 'base', 'basefont', 'big', 'blink', 'blockquote', 'body', 'br', 'b', 'button', 'caption', 'center', 'cite', 'code', 'col', 'dfn', 'dir', 'div', 'dl', 'dt', 'dd', 'em', 'font', 'form', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'head', 'hr', 'html', 'img', 'input', 'isindex', 'i', 'kbd', 'link', 'li', 'map', 'marquee', 'menu', 'meta', 'ol', 'option', 'param', 'pre', 'p', 'q', 'samp', 'script', 'select', 'small', 'span', 'strikeout', 'strong', 'style', 'sub', 'sup', 'table', 'td', 'textarea', 'th', 'tbody', 'thead', 'tfoot', 'title', 'tr', 'tt', 'ul', 'u', 'var']";
        static BibaViewEngineProperties _props;
        public static IApplicationBuilder UseBibaViewEngine(this IApplicationBuilder app)
        {
            var builder = app;

            app.UseMiddleware<BibaMiddleware>();

            app.Map("/c", (subApp) =>
            {
                subApp.Run((constext) =>
                {
                    return Task.FromResult(constext);
                });
            });

            return builder;
        }

        public static IServiceCollection AddBibaViewEngine(this IServiceCollection services, BibaViewEngineProperties props = null)
        {
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

            _props = props;

            services.AddSingleton(props);
            services.AddSingleton(ass);
            services.AddSingleton(components);
            services.AddSingleton(new RegistesteredTags(tags));
            services.AddTransient<IBibaRouter, BibaRouter>();

            services.AddTransient<BibaCompiler>();

            return services;
        }
    }
}
