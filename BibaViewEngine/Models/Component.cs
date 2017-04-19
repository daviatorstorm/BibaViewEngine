using BibaViewEngine.Models;
using System;
using BibaViewEngine.Compiler;
using HtmlAgilityPack;

namespace BibaViewEngine
{
    public class Component
    {
        public BibaCompiler _compiler;

        public string Name
        {
            get
            {
                var _name = GetType().Name.Replace("Component", "").ToLower();
                return string.IsNullOrWhiteSpace(_name) ? _name : throw new Exception("Component must have name");
            }
        }

        public HtmlNode HtmlElement { get; internal set; }

        public delegate void CompileComplete(HtmlElement element);
        public delegate void CompileStart(HtmlElement element);

        public event CompileComplete OnCompileComplete;
        public event CompileStart OnCompileStart;

        public string InnerCompile(string template)
        {
            foreach (var element in _compiler.ExtractNodes(HtmlElement))
            {
                _compiler.ExecuteCompiler(element);
            }
            return string.Empty;
        }
    }
}
