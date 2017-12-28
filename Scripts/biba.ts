class Biba {
    activatedController: ViewController;
    glboalController: ViewController;
    private static _cache: { [index: string]: any } = {};

    private constructor(mainComponent: ViewController, private router: BibaRouter) {
        this.router.onRouteFinish((args) => {
            this.activatedController = Biba.activateController(args.currentRoute.path == '/' ? '' : args.currentRoute.path, args.element, this.router);
        });
        this.glboalController = mainComponent;
        window.onpopstate = (event: any) => {
            if (event.target.history.state)
                this.router.route(event.target.history.state.path);
        };
    }

    static Start(mainElement: Element) {
        HttpClient.request('/app/start', HttpMethod.GET).then(res => {
            var data = JSON.parse(res.response);
            mainElement.innerHTML = data.html;
            Biba.inject('scope', data.scope);
            var router = new BibaRouter('/');
            router.initRouterLinks();
            router.route(location.pathname);
            new Biba(Biba.activateController('*', mainElement, router), router);
            Biba.inject('currentRoute', { path: location.pathname });
        });
    }

    static inject(name: string, injectible: any) {
        Biba._cache[name] = injectible;
    }

    static _get(name: string) {
        return Biba._cache[name] as any;
    }

    private static activateController(path: string, el: Element, router: BibaRouter) {
        var controller = Biba._get(path);
        var instance;
        if (controller) {
            instance = new controller(el, router);
        }
        return instance;
    }
}

function Controller(path: string) {
    return function (target) {
        Biba.inject(path, target);
    }
}

(function () {
    document.onreadystatechange = function (event) {
        if (document.readyState == 'complete') {
            Biba.Start(document.getElementsByTagName('app')[0]);
        }
    };
})();