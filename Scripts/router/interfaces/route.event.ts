interface RouteEvent {
    (from?: Route | any, to?: Route, args?: BibaRouter): boolean | any;
}