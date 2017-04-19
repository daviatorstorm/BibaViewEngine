using BibaViewEngine.Models;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BibaViewEngine.Compiler
{
    public class BibaCompiler
    {
        public string[] registeredTags = new string[] {
            "div", "h1", "p", "form", "html", "body", "head",
            "doctype", "meta", "style", "script", "#comment",
            "#text", "title", "hr", "ul", "li"
        };

        private readonly Assembly _ass;
        private readonly HtmlDocument _doc;

        public BibaCompiler(Assembly ass)
        {
            _ass = ass;
            _doc = new HtmlDocument();
        }

        public string GlobalCompile(string html)
        {
            _doc.LoadHtml(html);

            return ExecuteCompiler(_doc.DocumentNode);
        }

        private ComponentModule FindComponent(HtmlNode node)
        {
            var component = _ass.GetTypes().AsQueryable().Where(x => x.GetTypeInfo().BaseType == typeof(Component))
                .Single(x => x.Name.Replace("Component", "").ToLower() == node.Name);

            var componentInstance = Activator.CreateInstance(component) as Component;

            componentInstance._compiler = this;
            componentInstance.HtmlElement = node;

            return new ComponentModule(componentInstance);
        }

        private string Parse(ComponentModule module, HtmlNode element)
        {
            var componentHtml = module.Template;
            return componentHtml;
        }

        public IList<HtmlNode> ExtractNodes(HtmlNode node)
        {
            var excractedNodes = new List<HtmlNode>();

            var nodes = node.Descendants().ToList();

            foreach (var element in nodes.Where(x => !registeredTags.Contains(x.Name)))
            {
                excractedNodes.Add(element);
            }

            return excractedNodes;
        }

        public string ExecuteCompiler(HtmlNode node)
        {
            var extracted = ExtractNodes(node);
            foreach (var element in extracted)
            {
                var module = FindComponent(element);
                module.Component.HtmlElement = element;
                var parsedHtml = Parse(module, element);
                element.InnerHtml = parsedHtml;
            }

            var stream = new MemoryStream();
            _doc.Save(stream);
            stream.Position = 0;

            return new StreamReader(stream).ReadToEnd();
        }
    }
}
