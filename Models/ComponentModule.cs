using System.IO;
using System.Linq;

namespace BibaViewEngine.Models
{
    // Hack: Maybe depracated
    public class ComponentModule
    {
        private readonly Component _component;
        private readonly string _template;

        public Component Component
        {
            get
            {
                return _component;
            }
        }
        public string Template
        {
            get
            {
                return _template;
            }
        }
        public string Name
        {
            get
            {
                return _component.GetType().Name;
            }
        }
        public string TemplateLocation { get; private set; }

        public ComponentModule(Component component)
        {
            _component = component;
            var fileLocation = Directory.GetFiles("Client", "*.html", SearchOption.AllDirectories)
                .Single(x => Path.GetFileNameWithoutExtension(x) == component.GetType().Name);

            var plainTemplate = File.ReadAllText(fileLocation);

            // _template = component.InnerCompile(HtmlElement);
        }
    }
}
