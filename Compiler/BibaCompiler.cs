using HtmlAgilityPack;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BibaViewEngine.Attributes;
using System.Text.RegularExpressions;
using BibaViewEngine.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting; // Will potantialy used

namespace BibaViewEngine.Compiler
{
    public class BibaCompiler
    {
        private readonly RegistesteredTags _tags;
        private readonly Assembly _ass;
        private readonly HtmlDocument _doc;
        private readonly IEnumerable<Type> _registeredComponents;
        private readonly Regex directive = new Regex("\\(\\[([\\w]+)\\]\\)");

        public BibaCompiler(RegisteredComponentsCollection components, RegistesteredTags tags)
        {
            _ass = Assembly.GetEntryAssembly();
            _doc = new HtmlDocument();
            _registeredComponents = components.components;
            _tags = tags;
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

            foreach (var element in nodes.Where(x => !_tags.Contains(x.Name)))
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

        public Component Compile(Component child, Component parent = null)
        {
            var evalParentProps = Enumerable.Empty<KeyValuePair<string, object>>();

            if (parent != null)
            {
                evalParentProps = GetParentProps(parent);
            }

            var componentAttributes = child.HtmlElement.Attributes
                .Select(x => new KeyValuePair<string, object>(x.Name, x.Value));

            var childProps = child.GetType().GetProperties();

            if (evalParentProps.Count() > 0)
            {
                foreach (var attr in componentAttributes)
                {
                    var childProp = childProps.Single(x => x.Name.ToLower() == attr.Key.ToLower());
                    var parentValue = evalParentProps.FirstOrDefault(x => !directive.Match(attr.Value.ToString()).Success &&
                        x.Key.ToLower() == attr.Value.ToString().ToLower()).Value;

                    if (parentValue == null)
                    {
                        childProp.SetValue(child, attr.Value);
                    }
                    else
                    {
                        childProp.SetValue(child, parentValue);
                    }
                }
            }

            child.InnerCompile();

            return child;
        }

        public HtmlNode Compile(HtmlNode node, object context)
        {
            var newContext = context.ToDictionary();
            var matches = directive.Matches(node.InnerHtml);

            foreach (Match match in matches)
            {
                var itemName = match.Groups[1].Value;
                var matchValue = match.Value;
                object propValue;
                if (!newContext.TryGetValue(itemName, out propValue))
                {
                    propValue = null;
                }
                node.InnerHtml = node.InnerHtml.Replace(matchValue, propValue?.ToString());
            }

            return node;
        }

        public IEnumerable<KeyValuePair<string, object>> GetParentProps(Component parent)
        {
            var props = parent.GetType().GetProperties()
                .Where(x => !x.CustomAttributes.Any(y => y.AttributeType.Equals(typeof(IgnoreAttribute))))
                .Select(x => new KeyValuePair<string, object>(x.Name.ToLower(), x.GetValue(parent)));

            return props;
        }

        public void ClearAttributes(HtmlNode node)
        {
            node.Attributes.RemoveAll();
        }

        public void Transclude(Component component)
        {
            var regex = new Regex("<\\s*template\\s*\\/>");

            if (!regex.Match(component.Template).Success)
            {
                throw new Exception("Template must");
            }

            component.HtmlElement.InnerHtml = Regex.Replace(component.Template, "<\\s*template\\s*\\/>", component.HtmlElement.InnerHtml);
        }
    }

    static class Extentions
    {
        public static Dictionary<string, object> ToDictionary(this object source)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            if (source is Dictionary<string, object>)
            {
                return source as Dictionary<string, object>;
            }

            return source.GetType().GetProperties().ToDictionary(x => x.Name.ToLower(), x => x.GetValue(source));
        }
    }
}
