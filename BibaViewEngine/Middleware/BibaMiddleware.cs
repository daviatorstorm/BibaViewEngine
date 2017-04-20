using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using BibaViewEngine.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace BibaViewEngine.Middleware
{
    public class BibaMiddleware
    {
        private readonly BibaCompiler _compiler;
        private readonly RequestDelegate _next;
        private readonly BibaViewEngineProperties _props;
        private readonly IBibaRouter _router;

        public BibaMiddleware(RequestDelegate next, BibaCompiler compiler, BibaViewEngineProperties props, IBibaRouter router)
        {
            _compiler = compiler;
            _next = next;
            _props = props;
            _router = router;
        }

        public async Task Invoke(HttpContext context)
        {
            var mainHtml = File.ReadAllText(_props.IndexHtml);

            await context.Response.WriteAsync(await Task.Run(() =>
            {
                return _compiler.StartCompile(mainHtml);
            }));
        }
    }
}
