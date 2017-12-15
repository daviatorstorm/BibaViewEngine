using BibaViewEngine.Attributes;
using BibaViewEngine.Compiler;
using System.Collections.Generic;

namespace BibaViewEngine
{
    public class ForComponent : Component
    {
        public ForComponent(BibaCompiler bibaCompiler)
            : base(bibaCompiler)
        {
            PreventDefaults = true;
        }

        [Input]
        public IEnumerable<object> Source { get; set; }
        public object _Item { get; set; }
        public int _Index { get; set; }

        public override string InnerCompile()
        {
            var replacement = string.Empty;

            if (Source != null)
            {
                var evaluator = Evaluator.Create();
                var index = 0;

                foreach (var item in Source)
                {
                    _Item = item;
                    _Index = ++index;
                    replacement += _compiler.Compile(HtmlElement.InnerHtml, new { _Item, _Index }, evaluator);
                }
            }

            return replacement;
        }
    }
}