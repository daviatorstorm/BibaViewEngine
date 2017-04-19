using BibaViewEngine.Models;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BibaViewEngine.Compiler
{
    public class BibaCompiler
    {
        string[] registeredTags = new string[] {
            "div", "h1", "p", "form", "html", "body", "head",
            "doctype", "meta", "style", "script", "#comment",
            "#text", "title", "hr"
        };

        private readonly Assembly _ass;

        public BibaCompiler(Assembly ass)
        {
            _ass = ass;
        }

        public string Compile(string html)
        {
            return ExecuteCompiler(html);
        }

        private ComponentModule FindComponent(string name)
        {
            var component = _ass.GetTypes().AsQueryable().Where(x => x.GetTypeInfo().BaseType == typeof(Component))
                .Single(x => x.Name.Replace("Component", "").ToLower() == name);

            var componentInstance = Activator.CreateInstance(component) as Component;

            return new ComponentModule(componentInstance);
        }

        private string ExecuteParsing(ComponentModule module)
        {
            return string.Empty;
        }

        private HtmlNode ExtractNode(HtmlNode node)
        {
            // TODO: Finished here

            return node;
        }

        private string ExecuteCompiler(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode;

            foreach (var element in node.Descendants().Where(x => !registeredTags.Contains(x.Name)))
            {
                FindComponent(element.Name);
            }

            var stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;

            return new StreamReader(stream).ReadToEnd();
        }
    }
}
