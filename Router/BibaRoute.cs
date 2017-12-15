using Microsoft.AspNetCore.Authorization;
using System;

namespace BibaViewEngine.Router
{
    public class BibaRoute
    {
        public string Path { get; set; }
        public Type Component { get; set; }
        public Routes Children { get; set; }
        public IAuthorizationHandler Handler { get; set; }
    }
}
