using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BibaViewEngine
{
    public static class EngineExtensions
    {
        public static IApplicationBuilder UseBibaViewEngine(this IApplicationBuilder app)
        {
            var builder = app;

            return builder;
        }

        public static IServiceCollection AddBibaViewEngine(this IServiceCollection services)
        {
            var myServices = services;

            return myServices;
        }
    }
}
