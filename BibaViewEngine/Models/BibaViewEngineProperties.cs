using BibaViewEngine.Interfaces;
using System.Collections.Generic;

namespace BibaViewEngine.Models
{
    public class BibaViewEngineProperties
    {
        public string IndexHtml { get; set; } = "wwwroot/index.html";
        public IEnumerable<IRoute> Routes { get; set; }
    }
}
