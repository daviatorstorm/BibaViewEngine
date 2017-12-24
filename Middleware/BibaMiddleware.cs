using BibaViewEngine.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace BibaViewEngine.Middleware
{
    public class BibaMiddleware
    {
        private readonly BibaViewEngineProperties _props;

        public BibaMiddleware(RequestDelegate next, BibaViewEngineProperties props)
        {
            _props = props;
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
                var body = node.SelectSingleNode("//body");
                body.ParentNode.InsertBefore(headNode, body);
            }

            var link = doc.CreateElement("script");
            link.SetAttributeValue("src", "biba.min.js");

            var baseTag = doc.CreateElement("base");
            baseTag.Attributes.Add("href", "/");

            if (headNode.FirstChild != null)
            {
                headNode.AppendChild(baseTag);
                headNode.AppendChild(link);
            } else {
                headNode.InsertBefore(link, headNode.FirstChild);
                headNode.InsertBefore(baseTag, headNode.FirstChild);
            }

            mainHtml.Close();

            return node.OuterHtml;
        }
    }
}
