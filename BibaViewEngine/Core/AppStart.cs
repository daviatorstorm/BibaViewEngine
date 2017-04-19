using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using BibaViewEngine.Compiler;

namespace BibaViewEngine.Core
{
    public class BibaViewEngine
    {
        string htmlBody;
        
        //public BibaViewEngine(IServiceCollection services)
        //{
        //    var ass = Assembly.GetEntryAssembly();

        //    var htmlBody = File.ReadAllText("wwwroot/index.html");

        //    //var components = ass.GetTypes().AsQueryable().Where(x => x.GetTypeInfo().BaseType == typeof(Component));

        //    services.AddSingleton(this);
        //    services.AddSingleton(ass);

        //    services.AddTransient<BibaCompiler>();
        //}

       
    }
}
