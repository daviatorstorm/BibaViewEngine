using System;
using System.Collections.Generic;

namespace BibaViewEngine.Router
{
    public class BibaRoute
    {
        public string Path { get; set; }
        public IEnumerable<BibaRoute> Children { get; set; }
        public Type Component { get; set; }
    }
}
