using System.Collections.Generic;

namespace BibaViewEngine
{
    public class ForComponent : Component
    {
        public IEnumerable<object> Source { get; set; }
        public string Iter { get; set; }

        public ForComponent()
        {
        }

        public override void InnerCompile()
        {
            _compiler.ExtractComponents(HtmlElement);
            // return this.HtmlElement;
        }
    }
}