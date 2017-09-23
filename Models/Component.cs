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
        private string _template;
        public Component(BibaCompiler bibaCompiler)
        {
            _compiler = bibaCompiler;

            var fileLocation = Directory.GetFiles("Client", $"{GetType().Name}.html",
                                        SearchOption.AllDirectories).Single();

            using (var stream = File.OpenText(fileLocation))
            {
                _template = stream.ReadToEnd();
            }
        }

        internal BibaCompiler _compiler;
        [Ignore]
        public HtmlNode HtmlElement { get; internal set; }
        [Ignore]
        public virtual string Template { get => _template; private set => _template = value; }

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

            var compilerResult = _compiler.Compile(_template, this);

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