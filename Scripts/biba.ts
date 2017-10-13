interface Window {
    _controllerCache: any;
}

class Biba {
    router: BibaRouter;
    activatedController: ViewController;
    glboalController: ViewController;
    private static _cache: { [index: string]: Function } = {};

    private constructor(mainComponent: ViewController) {
        this.router = new BibaRouter();
        this.router.onRouteFinish((args) => {
            this.activatedController = Biba.activateController(args.currentRoute.path, args.element);
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
            new Biba(Biba.activateController('*', mainElement));
        };
        xhr.send();
    }

    static inject(name: string, controller: Function) {
        Biba._cache[name] = controller;
    }

    private static _get(name: string) {
        return Biba._cache[name] as any;
    }

    private static activateController(path: string, el: Element) {
        var controller = Biba._get(path);
        var instance;
        if (controller) {
            instance = new controller(el);
        }
        return instance;
    }
}

function Controller(path: string) {
    return function (target, a, b) {
        Biba.inject(path, target);
    }
}