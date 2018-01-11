using System.Threading.Tasks;
using BibaViewEngine.Router;
using Microsoft.AspNetCore.Authorization;

namespace BibaViewEngine.Auth
{
    public class BibaAuthorizationHandler : IAuthorizationHandler
    {
        private readonly CurrentRoute _route;

        public BibaAuthorizationHandler(CurrentRoute route)
        {
            _route = route;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (_route.Route.Handler != null)
            {
                _route.Route.Handler(context);
            }

            return Task.CompletedTask;
        }
    }
}