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
                new BibaRoute { Path = "complex", Component = typeof(FakeComplexComponent) },
                new BibaRoute { Path = "param/{id}", Component = typeof(FakeParamComponent) },
                new BibaRoute { Path = "child", Component = typeof(FakeComplexComponent), Children = new Routes {
                        new BibaRoute { Path = "sub", Component = typeof(FakeSubComponent) },
                        new BibaRoute { Path = "sub2", Component = typeof(FakeSub2Component) }
                    }
                },
                new BibaRoute { Path = "bad-child", Component = typeof(FakeBadComplexComponent), Children = new Routes {
                        new BibaRoute { Path = "sub", Component = typeof(FakeSubComponent) }
                    }
                }
            }, propsAct: props => props.ComponentsRoot = "FakeComponents");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseBibaViewEngine();
        }
    }
}
