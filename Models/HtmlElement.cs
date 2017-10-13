using System;
using System.Collections.Generic;
using System.Text;

namespace BibaViewEngine.Models
{
    public class HtmlElement
    {
        public string Name { get; set; }

        public IEnumerable<HtmlAttribute> Attributes { get; set; }
        public IEnumerable<HtmlElement> Children { get; set; }

        public HtmlElement()
        {

        }
    }
}
