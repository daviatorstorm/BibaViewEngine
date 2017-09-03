using HtmlAgilityPack;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BibaViewEngine.Attributes;
using System.Text.RegularExpressions;
using BibaViewEngine.Models;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace BibaViewEngine.Compiler
{
    public class BibaCompiler : IDisposable
    {
        private readonly RegistesteredTags _tags;
        private readonly Assembly _ass;
        private readonly HtmlDocument _doc;
        private readonly IEnumerable<Type> _registeredComponents;
        private readonly Regex directive = new Regex("\\(\\[([\\w \\+\\.\\\"\\-\\*\\/\\(\\)]+)\\]\\)");

        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

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
            var component = _registeredComponents.Single(x => x.Name.Replace("Component", "").ToLower() == node.Name);

            return Component.Create(this, node, component);
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

            child._InnerCompile();

            return child;
        }

        public HtmlNode Compile(HtmlNode node, object context)
        {
            var matches = directive.Matches(node.InnerHtml)
                .OfType<Match>()
                .Distinct(new EqualityComparer());

            string replacement = node.InnerHtml;

            var eval = Evaluator.Create();

            foreach (Match match in matches)
            {
                replacement = replacement
                    .Replace(match.Value, eval.Evaluate(match.Groups[1].Value, context));
            }

            node.InnerHtml = replacement;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                handle.Dispose();
            }

            disposed = true;
        }
    }

    public class EqualityComparer : IEqualityComparer<Match>
    {
        public bool Equals(Match x, Match y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(Match obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}