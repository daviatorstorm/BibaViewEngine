using BibaViewEngine.Interfaces;
using System.Collections.Generic;

namespace BibaViewEngine.Models
{
    public class BibaViewEngineProperties
    {
        public string IndexHtml { get; set; } = "wwwroot/index.html";
        public string IndexHtmlBuild { get; } = "wwwroot/index.build.html";

        public IEnumerable<IRoute> Routes { get; set; }
        public string RouterBuild { get; } = "Scripts/build.js";

        public string OutputPath { get; } = "bin/Debug/netcoreapp1.1";
    }
}
