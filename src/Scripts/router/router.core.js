var BibaRouter = /** @class */ (function () {
    function BibaRouter(baseUrl, rootElement) {
        if (rootElement === void 0) { rootElement = document.documentElement; }
        this.rootElement = rootElement;
        this.baseUrl = baseUrl || '/';
    }
    Object.defineProperty(BibaRouter.prototype, "currentRoute", {
        get: function () {
            return Biba._get('currentRoute');
        },
        enumerable: true,
        configurable: true
    });
    BibaRouter.prototype.route = function (path) {
        var _this = this;
        if (path === void 0) { path = this.baseUrl; }
        path = path || this.baseUrl;
        if (this.currentRoute && path == this.currentRoute.path)
            return;
        document.dispatchEvent(new CustomEvent('onRouteStart', { detail: { path: path } }));
        return this.getComponent(path).then(function (response) {
            var data = JSON.parse(response.response);
            _this.routerContainer.innerHTML = data.html;
            Biba.inject('scope', data.scope);
            history.pushState({ path: path }, document.title, path);
            var newRouter = new BibaRouter(location.pathname, _this.routerContainer);
            if (!newRouter.initRouterLinks()) {
                newRouter = null;
                _this.initRouterLinks();
            }
            _this.currentRoute.path = path;
            document.dispatchEvent(new CustomEvent('onRouteFinish', {
                detail: {
                    currentRoute: _this.currentRoute,
                    element: _this.routerContainer
                }
            }));
            return response;
        });
    };
    BibaRouter.prototype.initRouterLinks = function () {
        var _this = this;
        var hasChildContainer = false;
        var rc = this.rootElement.querySelector('[router-container]');
        if (rc) {
            this.routerContainer = rc;
            this.routerContainer.attributes.removeNamedItem('router-container');
            hasChildContainer = true;
        }
        var allElements = this.rootElement.querySelectorAll('[router-path]');
        allElements.forEach(function (item) {
            var path = item.attributes.getNamedItem('router-path').value;
            if (path[0] != '/' && hasChildContainer)
                path = UrlUtils.join(_this.baseUrl, path);
            item.path = path;
            item.attributes.removeNamedItem('router-path');
            _this.giveAnchorHandler(item);
        });
        return hasChildContainer;
    };
    BibaRouter.prototype.giveAnchorHandler = function (el) {
        var _this = this;
        el.onclick = function (e) { return _this.routerLinkClickHandler(e); };
    };
    BibaRouter.prototype.routerLinkClickHandler = function (event) {
        var componentPath = event.target.path;
        this.route(componentPath);
        return false;
    };
    BibaRouter.prototype.getComponent = function (path, data) {
        return HttpClient.request(("c/" + path).replace('//', '/'), HttpMethod.GET);
    };
    BibaRouter.prototype.onRouteStart = function (handler) {
        document.addEventListener('onRouteStart', function (args) { handler(args.detail); }, false);
    };
    BibaRouter.prototype.onRouteFinish = function (handler) {
        document.addEventListener('onRouteFinish', function (args) { handler(args.detail); }, false);
    };
    return BibaRouter;
}());
