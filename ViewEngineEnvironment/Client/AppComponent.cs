using BibaViewEngine;
using System.Collections.Generic;
using BibaViewEngine.Attributes;

namespace ViewEngineEnvironment.Client
{
    public class AppComponent : Component
    {
        [Input]
        public object Config { get; set; }
    }
}
