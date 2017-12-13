using BibaViewEngine.Compiler;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using BibaViewEngine.Attributes;
using System;
using BibaViewEngine.Interfaces;
using System.Dynamic;

namespace BibaViewEngine
{
    public abstract class Component
    {
        private HtmlNode _htmlElement;
        protected readonly BibaCompiler _compiler;
        public Component(BibaCompiler bibaCompiler)
        {
            _compiler = bibaCompiler;

            var fileLocation = Directory.GetFiles("Client", $"{GetType().Name}.html",
                                        SearchOption.AllDirectories).FirstOrDefault();

            if (fileLocation != null)
                using (var stream = File.OpenText(fileLocation))
                {
                    Template = stream.ReadToEnd();
                }
        }

        [Ignore]
        public HtmlNode HtmlElement
        {
            get
            {
                if (_htmlElement == null)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(Template);
                    _htmlElement = doc.DocumentNode;
                }

                return _htmlElement;
            }
            internal set { _htmlElement = value; if (!string.IsNullOrWhiteSpace(Template)) _htmlElement.InnerHtml = Template; }
        }
        [Ignore]
        public virtual string Template { get; set; }
        [Ignore]
        protected bool PreventDefaults { get; set; } = false;
        [Ignore]
        public dynamic Scope { get; internal set; } = new ExpandoObject();

        public delegate void EmptyDelegate();
        public delegate void BeforePropertiesSet(object sender);

        protected event EmptyDelegate OnCompileFinish;
        protected event EmptyDelegate OnCompileStart;

        public virtual string InnerCompile() { return string.Empty; }

        internal string _InnerCompile()
        {
            if (OnCompileStart != null)
            {
                OnCompileStart();
            }

            _compiler.ExecuteCompiler(HtmlElement, this);

            var compilerResult = PreventDefaults ? InnerCompile() : _compiler.Compile(HtmlElement.InnerHtml, this);

            _compiler.ClearAttributes(HtmlElement);

            if (OnCompileFinish != null)
            {
                OnCompileFinish();
            }

            return compilerResult;
        }
    }
}