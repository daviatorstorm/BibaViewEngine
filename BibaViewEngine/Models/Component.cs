using BibaViewEngine.Models;
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
        public HtmlNode HtmlElement { get; internal set; }
        [Ignore]
        public string Template { get; private set; }

        public delegate void CompileComplete(HtmlElement element);
        public delegate void CompileStart(HtmlElement element);

        public event CompileComplete OnCompileComplete;
        public event CompileStart OnCompileStart;

        public virtual void InnerCompile()
        {
            HtmlElement.InnerHtml = Template;
            var compiledTemplate = _compiler.Compile(HtmlElement, this);
            _compiler.ExecuteCompiler(compiledTemplate, this);
        }
    }
}
