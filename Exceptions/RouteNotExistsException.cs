using System;

namespace BibaViewEngine.Exceptions
{
    public class RouteNotExistsException : Exception
    {
        public string RouteName { get; }
        public RouteNotExistsException(string routeName)
        {
            RouteName = routeName;
            Data.Add("RouteName", RouteName);
        }

        public override string Message { get => $"Route with name \"{RouteName}\" does not exists"; }
    }
}