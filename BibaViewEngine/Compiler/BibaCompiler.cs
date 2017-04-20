using HtmlAgilityPack;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BibaViewEngine.Attributes;
using System.Text.RegularExpressions;
using System.Collections;

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
        private readonly IEnumerable<Type> _registeredComponents;

        public BibaCompiler(Assembly ass, RegisteredComponentsCollection components)
        {
            _ass = ass;
            _doc = new HtmlDocument();
            _registeredComponents = components.components;
        }

        public string StartCompile(string html)
        {
            _doc.LoadHtml(html);

            return ExecuteCompiler(_doc.DocumentNode).InnerHtml;
        }

        public IList<HtmlNode> ExtractComponents(HtmlNode node)
        {
            var excractedNodes = new List<HtmlNode>();

            var nodes = node.Descendants().ToList();

            foreach (var element in nodes.Where(x => !registeredTags.Contains(x.Name)))
            {
                excractedNodes.Add(element);
            }

            return excractedNodes;
        }

        public HtmlNode ExecuteCompiler(HtmlNode node, Component parent = null)
        {
            var extracted = ExtractComponents(node);
            foreach (var element in extracted)
            {
                var component = FindComponent(element);
                var compilerResult = Compile(component, parent);
            }

            return _doc.DocumentNode;
        }

        public Component FindComponent(HtmlNode node)
        {
            var components = _registeredComponents.Concat(_ass.GetTypes().Where(x => x.GetTypeInfo().BaseType == typeof(Component)));
            var component = components.Single(x => x.Name.Replace("Component", "").ToLower() == node.Name);

            var componentInstance = Activator.CreateInstance(component) as Component;

            componentInstance._compiler = this;
            componentInstance.HtmlElement = node;

            return componentInstance;
        }

        public Component Compile(Component component, Component parent = null)
        {
            var htmlNode = component.HtmlElement;
            var compiledAttributes = new List<string>();
            var evalParentProps = Enumerable.Empty<KeyValuePair<string, object>>();

            if (parent != null)
            {
                // TODO: Finished here
                evalParentProps = GetAttributesForComponent(component);
                compiledAttributes.AddRange(evalParentProps.Select(x => x.Key));
            }

            var componentAttributes = component.HtmlElement.Attributes
                .Where(x => !compiledAttributes.Contains(x.Name.ToLower()))
                .Select(x => new KeyValuePair<string, object>(x.Name, x.Value));

            var connectableValues = component.GetType().GetProperties()
                .Where(x => componentAttributes.Any(y => y.Key.ToLower() == x.Name.ToLower()) &&
                        !evalParentProps.Any(y => y.Key == x.Name.ToLower())
                );

            foreach (var item in connectableValues)
            {
                var assignValue = componentAttributes.Single(x => x.Key.ToLower() == item.Name.ToLower()).Value;
                item.SetValue(component, assignValue);
            }

            component.InnerCompile();

            return component;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAttributesForComponent(Component component)
        {
            var attrs = component.HtmlElement.Attributes.Select(x => new KeyValuePair<string, object>(x.Name, x.Value));

            var props = component.GetType().GetProperties().Where(x =>
                    attrs.Any(attr => attr.Key.ToLower() == x.Name.ToLower()) &&
                        x.CustomAttributes.Any(y => y.AttributeType.Equals(typeof(InputAttribute)))
                );

            var evalProps = props.Select(x => new KeyValuePair<string, object>(x.Name.ToLower(), x.GetValue(x)));

            return evalProps;
        }

        public HtmlNode Compile(HtmlNode node, object context)
        {
            // TODO: Test this
            var regex = new Regex("\\(\\[([\\w]+)\\]\\)");
            var nodes = node.SelectNodes("//*[text()[contains(., '[(')]]") ?? new HtmlNodeCollection(node);

            foreach (var innerNode in nodes)
            {
                var matches = regex.Matches(string.Empty);
                foreach (Match match in matches)
                {
                    var itemName = match.Groups[1].Value;
                    var matchValue = match.Value;
                    var propValue = context.GetType().GetField(itemName).GetValue(context).ToString();
                    innerNode.InnerHtml = innerNode.InnerHtml.Replace(matchValue, propValue);
                }
            }

            return node;
        }
    }
}
