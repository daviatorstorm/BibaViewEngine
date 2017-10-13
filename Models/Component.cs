using BibaViewEngine.Compiler;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using BibaViewEngine.Attributes;
using System;

namespace BibaViewEngine
{
    public abstract class Component
    {
        private HtmlNode _htmlElement;
        private readonly BibaCompiler _compiler;
        public Component(BibaCompiler bibaCompiler)
        {
            _compiler = bibaCompiler;

            var fileLocation = Directory.GetFiles("Client", $"{GetType().Name}.html",
                                        SearchOption.AllDirectories).Single();

            using (var stream = File.OpenText(fileLocation))
            {
                Template = stream.ReadToEnd();
            }
        }

        [Ignore]
        public HtmlNode HtmlElement { get => _htmlElement; internal set { _htmlElement = value; _htmlElement.InnerHtml = Template; } }
        [Ignore]
        public virtual string Template { get; set; }

        public delegate void EmptyDelegate();
        public delegate void BeforePropertiesSet(object sender);

        protected event EmptyDelegate OnCompileFinish;
        protected event EmptyDelegate OnCompileStart;

        public virtual void InnerCompile() { }

        internal string _InnerCompile()
        {
            if (OnCompileStart != null)
            {
                OnCompileStart();
            }

            _compiler.ExecuteCompiler(HtmlElement, this);

            var compilerResult = _compiler.Compile(HtmlElement.InnerHtml, this);

            _compiler.ClearAttributes(HtmlElement);

            InnerCompile();

            if (OnCompileFinish != null)
            {
                OnCompileFinish();
            }

            return compilerResult;
        }
    }
}