interface Window {
    _controllerCache: any;
}

class Biba {
    activatedController: ViewController;
    glboalController: ViewController;
    private static _cache: { [index: string]: any } = {};

    private constructor(mainComponent: ViewController, private router: BibaRouter) {
        this.router.onRouteFinish((args) => {
            this.activatedController = Biba.activateController(args.currentRoute.path, args.element, this.router);
        });
        this.glboalController = mainComponent;
    }

    static Start(startComponent: string) {
        var mainElement = document.getElementsByTagName(startComponent)[0];

        var xhr = new XMLHttpRequest();
        xhr.open('Get', '/app/start');
        xhr.onloadend = (res: any) => {
            var data = JSON.parse(res.target.response);
            mainElement.innerHTML = data.html;
            Biba.inject('scope', data.scope);
            var router = new BibaRouter();
            new Biba(Biba.activateController('*', mainElement, router), router);
        };
        xhr.send();
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