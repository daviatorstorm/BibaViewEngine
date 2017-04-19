using BibaViewEngine.Models;
using System;

namespace BibaViewEngine
{
    public abstract class Component
    {
        public string Name
        {
            get
            {
                var _name = GetType().Name.Replace("Component", "").ToLower();
                return string.IsNullOrWhiteSpace(_name)? _name : throw new Exception("Component must have name");
            }
        }

        public delegate void CompileComplete(HtmlElement element);
        public delegate void CompileStart(HtmlElement element);

        public event CompileComplete OnCompileComplete;
        public event CompileStart OnCompileStart;
    }
}
