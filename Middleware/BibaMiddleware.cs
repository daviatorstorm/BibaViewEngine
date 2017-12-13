using BibaViewEngine.Compiler;
using BibaViewEngine.Interfaces;
using BibaViewEngine.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using HtmlAgilityPack;

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
            if (!context.Response.HasStarted && context.Response.StatusCode != 404 &&
                !Path.HasExtension(context.Request.Path))
                using (var mainHtml = File.Open(_props.IndexHtml, FileMode.Open))
                    await context.Response.WriteAsync(AppendMainScript(mainHtml));
        }

        private string AppendMainScript(Stream mainHtml)
        {
            var doc = new HtmlDocument();
            doc.Load(mainHtml);
            var node = doc.DocumentNode;

            var headNode = node.SelectSingleNode("//head");

            if (headNode == null)
            {
                headNode = doc.CreateElement("head");
                node.AppendChild(headNode);
            }

            var link = doc.CreateElement("script");
            link.SetAttributeValue("src", "biba.min.js");

            var baseTag = doc.CreateElement("base");
            baseTag.Attributes.Add("href", "/");

            headNode.AppendChild(baseTag);
            headNode.AppendChild(link);

            mainHtml.Close();

            return node.OuterHtml;
        }
    }
}
