using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BibaViewEngine.Models
{
    public class RegistesteredTags : HashSet<string>
    {
        public RegistesteredTags(IEnumerable<string> tags)
            : base(tags, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}