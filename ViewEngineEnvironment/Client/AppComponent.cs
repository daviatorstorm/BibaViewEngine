using BibaViewEngine;
using System.Collections.Generic;

namespace ViewEngineEnvironment.Client
{
    public class AppComponent : Component
    {
        public List<string> Values { get; set; } = new List<string> { "value1", "value2", "value3" };
    }
}
