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
            var tmpDoc = new HtmlDocument();
            var innerHtml = HtmlElement.InnerHtml;
            HtmlElement.InnerHtml = string.Empty;
            
            foreach (var item in Source)
            {
                var newNode = tmpDoc.CreateElement(Element);
                newNode.InnerHtml = innerHtml;
                dic[Iter] = item;
                var compiledHtml = _compiler.Compile(newNode, dic);
                HtmlElement.AppendChild(compiledHtml);
                dic.Clear();
            }
        }
    }
}