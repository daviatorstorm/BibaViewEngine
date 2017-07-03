using BibaViewEngine;
using System.Collections.Generic;
using BibaViewEngine.Attributes;

namespace ViewEngineEnvironment.Client
{
    public class AppComponent : Component
    {
        [Input]
        public object Config { get; set; }
        public List<string> Values { get; set; } = new List<string> { "value1", "value2", "value3" };
    }
}
