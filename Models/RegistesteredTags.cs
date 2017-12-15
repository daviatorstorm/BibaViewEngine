using System;
using System.Collections.Generic;

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