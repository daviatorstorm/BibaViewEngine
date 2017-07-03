using BibaViewEngine;

namespace ViewEngineEnvironment.Client.Components
{
    public class RowComponent : Component
    {
        public RowComponent()
        {
            _transclude = true;
        }

        //public override void InnerCompile()
        //{
        //    _compiler.Transclude(this);

        //    _compiler.ExecuteCompiler(HtmlElement, this);

        //    _compiler.Compile(HtmlElement, this);

        //    _compiler.ClearAttributes(HtmlElement);
        //}
    }
}
