using BibaViewEngine;
using BibaViewEngine.Router;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using test.FakeComponents;

namespace test.FaceMiscs
{
    public class FakeEmptyStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBibaViewEngine<FaceAppComponent>(new Routes
            {
                new BibaRoute { Path = "main", Component = typeof(FakeMainComponent) },
                new BibaRoute { Path = "complex", Component = typeof(FakeComplexComponent) }
            }, propsAct: props => props.ComponentsRoot = "FakeComponents");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseBibaViewEngine();
        }
    }
}
