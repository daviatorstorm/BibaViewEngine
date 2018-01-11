var Biba = /** @class */ (function () {
    function Biba(mainComponent, router) {
        var _this = this;
        this.router = router;
        this.router.onRouteFinish(function (args) {
            _this.activatedController = Biba.activateController(args.currentRoute.path == '/' ? '' : args.currentRoute.path, args.element, _this.router);
        });
        this.glboalController = mainComponent;
        window.onpopstate = function (event) {
            if (event.target.history.state)
                _this.router.route(event.target.history.state.path);
        };
    }
    Biba.Start = function (mainElement) {
        HttpClient.request('/app/start', HttpMethod.GET).then(function (res) {
            var data = JSON.parse(res.response);
            mainElement.innerHTML = data.html;
            Biba.inject('scope', data.scope);
            var router = new BibaRouter('/');
            router.initRouterLinks();
            router.route(location.pathname);
            new Biba(Biba.activateController('*', mainElement, router), router);
            Biba.inject('currentRoute', { path: location.pathname });
        });
    };
    Biba.inject = function (name, injectible) {
        Biba._cache[name] = injectible;
    };
    Biba._get = function (name) {
        return Biba._cache[name];
    };
    Biba.activateController = function (path, el, router) {
        var controller = Biba._get(path);
        var instance;
        if (controller) {
            instance = new controller(el, router);
        }
        return instance;
    };
    Biba._cache = {};
    return Biba;
}());
function Controller(path) {
    return function (target) {
        Biba.inject(path, target);
    };
}
(function () {
    document.onreadystatechange = function (event) {
        if (document.readyState == 'complete') {
            Biba.Start(document.getElementsByTagName('app')[0]);
        }
    };
})();
