using BibaViewEngine.Compiler;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using BibaViewEngine.Attributes;

namespace BibaViewEngine
{
    public class Component
    {
        public Component()
        {
            try
            {
                var fileLocation = Directory.GetFiles("Client", "*.html", SearchOption.AllDirectories)
                   .Single(x => Path.GetFileNameWithoutExtension(x) == GetType().Name);

                Template = File.ReadAllText(fileLocation);
            }
            catch
            {
                Template = "";
            }
        }

        public BibaCompiler _compiler;
        [Ignore]
        public virtual HtmlNode HtmlElement { get; internal set; }
        [Ignore]
        public virtual string Template { get; private set; }
        [Ignore]
        public string ComponentName
        {
            get
            {
                return GetType().Name;
            }
        }

        public bool _transclude { get; set; } = false;

        public delegate void EmptyDelegate();
        public delegate void BeforePropertiesSet(object sender);

        protected event EmptyDelegate OnCompileFinish;
        protected event EmptyDelegate OnCompileStart;

        protected event BeforePropertiesSet OnBeforePropertiesSet;
        protected event EmptyDelegate OnAfterPropertiesSet;

        public virtual void InnerCompile()
        {
            if (_transclude)
            {
                _compiler.Transclude(this);
            }
            else
            {
                HtmlElement.InnerHtml = Template;
            }

            _compiler.ExecuteCompiler(HtmlElement, this);

            // _compiler.Compile(HtmlElement, this);
            _compiler.CompileV2(HtmlElement, this);

            _compiler.ClearAttributes(HtmlElement);
        }

        public void _InnerCompile()
        {
            if (OnCompileStart != null)
            {
                OnCompileStart();
            }

            InnerCompile();

            if (OnCompileFinish != null)
            {
                OnCompileFinish();
            }
        }
    }
}