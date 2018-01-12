using BibaViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using test.FakeComponents;

namespace test.FaceMiscs
{
    public class FakeEmptyStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBibaViewEngine<FaceAppComponent>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseBibaViewEngine();
        }
    }
}
