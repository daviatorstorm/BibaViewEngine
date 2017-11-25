using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BibaViewEngine.Router
{
    public class BibaRoute
    {
        public string Path { get; set; }
        public IEnumerable<BibaRoute> Children { get; set; }
        public Type Component { get; set; }
        public IAuthorizationHandler Handler { get; set; }
    }
}
