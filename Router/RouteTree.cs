namespace BibaViewEngine.Router
{
    public class RouteTree
    {
        public BibaRoute Route { get; set; }
        public string RouteName { get; set; }
        public RouteTree NestedRoute { get; set; }
    }
}