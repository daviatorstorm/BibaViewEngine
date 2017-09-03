using BibaViewEngine.Compiler;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using BibaViewEngine.Attributes;
using System;

namespace BibaViewEngine
{
    public class Component
    {
        internal static Component Create(BibaCompiler compiler, HtmlNode node, Type componentType)
        {
            if (!typeof(Component).IsAssignableFrom(componentType))
            {
                throw new Exception($"Component must inherited from Component type");
            }

            var componentInstanse = Activator.CreateInstance(componentType) as Component;

            componentInstanse._compiler = compiler;
            componentInstanse.HtmlElement = node;

            try
            {
                var fileLocation = Directory.GetFiles("Client", "*.html", SearchOption.AllDirectories)
                   .Single(x => Path.GetFileNameWithoutExtension(x) == componentInstanse.GetType().Name);

                componentInstanse.Template = File.ReadAllText(fileLocation);
            }
            catch
            {
                componentInstanse.Template = "";
            }

            return componentInstanse;
        }

        internal BibaCompiler _compiler;
        [Ignore]
        public virtual HtmlNode HtmlElement { get; internal set; }
        [Ignore]
        public virtual string Template { get; private set; }
        [Ignore]
        public string ComponentName
        {
            get
            {
                return GetType().Name.Replace("Component", "");
            }
        }

        public bool _transclude { get; set; } = false;

        public delegate void EmptyDelegate();
        public delegate void BeforePropertiesSet(object sender);

        protected event EmptyDelegate OnCompileFinish;
        protected event EmptyDelegate OnCompileStart;

        protected event BeforePropertiesSet OnBeforePropertiesSet;
        protected event EmptyDelegate OnAfterPropertiesSet;

        public virtual void InnerCompile() { }

        internal void _InnerCompile()
        {
            if (_transclude)
            {
                _compiler.Transclude(this);
            }
            else
            {
                HtmlElement.InnerHtml = Template;
            }

            if (OnCompileStart != null)
            {
                OnCompileStart();
            }

            _compiler.ExecuteCompiler(HtmlElement, this);

            _compiler.Compile(HtmlElement, this);

            _compiler.ClearAttributes(HtmlElement);

            InnerCompile();

            if (OnCompileFinish != null)
            {
                OnCompileFinish();
            }
        }
    }
}