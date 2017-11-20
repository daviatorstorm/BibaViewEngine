using BibaViewEngine.Interfaces;
using System.Collections.Generic;

namespace BibaViewEngine.Models
{
    public class BibaViewEngineProperties
    {
        public string IndexHtml { get; set; } = "wwwroot/index.html";
        public string ContentRoot { get; set; } = "wwwroot";
        public string LibRoot { get; set; } = "wwwroot";
    }
}
