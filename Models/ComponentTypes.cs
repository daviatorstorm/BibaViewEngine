using System;
using System.Collections.Generic;
using System.Linq;

namespace BibaViewEngine.Models
{
    public class ComponentTypes : List<Type>
    {
        public ComponentTypes(IEnumerable<Type> types) : base(types) { }
    }
}