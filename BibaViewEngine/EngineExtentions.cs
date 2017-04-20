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

namespace BibaViewEngine
{
    public static class EngineExtensions
    {
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
            services.AddTransient<IBibaRouter, BibaRouter>();

            services.AddTransient<BibaCompiler>();

            return services;
        }
    }
}
