using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace BibaViewEngine
{
    public class ForComponent : Component
    {
        public IEnumerable<object> Source { get; set; }
        public string Iter { get; set; }
        public string Element { get; set; }

        public ForComponent()
        {
        }

        public override void InnerCompile()
        {
            if (Iter == null || Element == null)
            {
                throw new Exception("Iter and Element cannot be empty");
            }

            var dic = new Dictionary<string, object>();
            dynamic context = new ExpandoObject();
            var eoCall = (ICollection<KeyValuePair<string, object>>)context;
            var tmpDoc = new HtmlDocument();
            var innerHtml = HtmlElement.InnerHtml;
            HtmlElement.InnerHtml = string.Empty;

            foreach (var item in Source)
            {
                eoCall.Add(new KeyValuePair<string, object>(Iter.ToLower(), item));
                dynamic tmp = context;

                var newNode = tmpDoc.CreateElement(Element);
                newNode.InnerHtml = innerHtml;
                var compiledHtml = _compiler.Compile(newNode, tmp);
                eoCall.Clear();
            }
        }
    }
}