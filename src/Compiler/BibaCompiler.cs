using BibaViewEngine.Attributes;
using BibaViewEngine.Extensions;
using BibaViewEngine.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BibaViewEngine.Compiler
{
    public sealed class BibaCompiler : IDisposable
    {
        private readonly RegistesteredTags _tags;
        private readonly IServiceProvider _provider;
        private readonly ComponentTypes _types;
        private readonly Assembly _ass;
        private readonly HtmlDocument _doc;
        private readonly Regex directive = new Regex("\\(\\[([\\w \\+\\.\\\"\\-\\*\\/\\(\\)]+)\\]\\)");

        bool disposed = false;

        public BibaCompiler(RegistesteredTags tags, IServiceProvider services, ComponentTypes types)
        {
            _ass = Assembly.GetEntryAssembly();
            _doc = new HtmlDocument();
            _tags = tags;
            _provider = services;
            _types = types;
        }

        public IList<HtmlNode> ExtractComponents(HtmlNode node)
        {
            var excractedNodes = new List<HtmlNode>();

            var nodes = node.Descendants().ToList();

            foreach (var element in nodes.Where(x => !_tags.Contains(x.Name)))
                excractedNodes.Add(element);

            return excractedNodes;
        }

        public string ExecuteCompiler(HtmlNode node, Component parent = null)
        {
            var extracted = ExtractComponents(node);
            foreach (var element in extracted)
            {
                var component = FindComponent(element, parent?.HttpContext);
                element.InnerHtml = PassValues(component, parent: parent);
            }

            return node.InnerHtml;
        }

        public Component FindComponent(HtmlNode node, HttpContext context)
        {
            var component = _types.Single(x => x.Name.ToLower() == $"{node.Name.ToLower()}component");

            var instance = _provider.CreateComponent(component, node, context);

            return instance;
        }

        public string PassValues(Component child, Component parent = null)
        {
            var evalParentProps = Enumerable.Empty<KeyValuePair<string, object>>();

            if (parent != null)
            {
                evalParentProps = GetParentProps(parent);
                ReAssign(parent.Scope, child.Scope);
            }

            if (evalParentProps.Count() > 0)
            {
                var componentAttributes = child.HtmlElement.Attributes
                    .Select(x => new KeyValuePair<string, object>(x.Name, x.Value));

                var childProps = child.GetType().GetProperties();

                foreach (var attr in componentAttributes)
                {
                    var childProp = childProps.Single(x => x.Name.ToLower() == attr.Key.ToLower());
                    var parentValue = evalParentProps.FirstOrDefault(x => !directive.Match(attr.Value.ToString()).Success &&
                        x.Key.ToLower() == attr.Value.ToString().ToLower()).Value;

                    if (parentValue == null)
                        childProp.SetValue(child, attr.Value);
                    else
                        childProp.SetValue(child, parentValue);
                }
            }

            return child._InnerCompile();
        }

        public string Compile(string template, object context, Evaluator evaluator = null)
        {
            var matches = directive.Matches(template)
                .OfType<Match>()
                .Distinct(new EqualityComparer());

            string replacement = template;

            var eval = evaluator ?? Evaluator.Create();

            foreach (Match match in matches)
                replacement = replacement
                    .Replace(match.Value, eval.Evaluate(match.Groups[1].Value.Trim(), context));

            return replacement;
        }

        public IEnumerable<KeyValuePair<string, object>> GetParentProps(Component parent) =>
            parent.GetType().GetProperties()
                .Where(x => !x.CustomAttributes.Any(y => y.AttributeType.Equals(typeof(IgnoreAttribute))))
                .Select(x => new KeyValuePair<string, object>(x.Name.ToLower(), x.GetValue(parent)));

        public void ClearAttributes(HtmlNode node) =>
            node.Attributes.RemoveAll();

        public void Transclude(Component component)
        {
            var regex = new Regex("<\\s*template\\s*\\/>");

            if (!regex.Match(component.Template).Success)
                throw new Exception("Template must");

            component.HtmlElement.InnerHtml = Regex.Replace(component.Template, "<\\s*template\\s*\\/>", component.HtmlElement.InnerHtml);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
                GC.SuppressFinalize(this);

            disposed = true;
        }

        public void ReAssign(dynamic source, dynamic destination)
        {
            foreach (var item in (IDictionary<String, Object>)source)
                destination[item.Key] = item.Value;
        }
    }

    public class EqualityComparer : IEqualityComparer<Match>
    {
        public bool Equals(Match x, Match y) =>
            x.Value == y.Value;

        public int GetHashCode(Match obj) =>
            obj.Value.GetHashCode();
    }
}