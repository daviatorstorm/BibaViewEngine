using BibaViewEngine.Attributes;
using BibaViewEngine.Compiler;
using BibaViewEngine.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace BibaViewEngine
{
    public abstract class Component
    {
        private HtmlNode _htmlElement;
        private string _template;
        internal BibaCompiler _compiler;

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
            internal set { _htmlElement = value; if (!string.IsNullOrWhiteSpace(Template) && _htmlElement != null) _htmlElement.InnerHtml = Template; }
        }
        [Ignore]
        public virtual string Template
        {
            get
            {
                if (_template == null)
                {
                    var fileLocation = Directory.GetFiles(Props.ComponentsRoot, $"{GetType().Name}.html",
                                        SearchOption.AllDirectories).FirstOrDefault();

                    if (fileLocation != null & File.Exists(fileLocation))
                        using (var stream = File.OpenText(fileLocation))
                            _template = stream.ReadToEnd();
                }

                return _template;
            }
        }
        [Ignore]
        protected bool PreventDefaults { get; set; }
        [Ignore]
        public dynamic Scope { get; internal set; } = new ExpandoObject();
        [Ignore]
        internal BibaViewEngineProperties Props { get; set; }
        [Ignore]
        public HttpContext HttpContext { get; internal set; }

        public delegate void EmptyDelegate();

        protected event EmptyDelegate OnCompileFinish;
        protected event EmptyDelegate OnCompileStart;
        protected event EmptyDelegate OnInit;

        internal void StartInitEvent()
        {
            OnInit?.Invoke();
        }

        public virtual string InnerCompile() { return string.Empty; }

        internal string _InnerCompile()
        {
            OnCompileStart?.Invoke();

            _compiler.ExecuteCompiler(HtmlElement, this);

            var compilerResult = PreventDefaults ? InnerCompile() : _compiler.Compile(HtmlElement.InnerHtml, this);

            _compiler.ClearAttributes(HtmlElement);

            OnCompileFinish?.Invoke();

            return string.IsNullOrWhiteSpace(compilerResult) ? "<!-- -->" : compilerResult;
        }
    }
}