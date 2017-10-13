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
            var controller = Biba._get(args.currentRoute.path);
            this.activatedController = new controller(args.element);
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
            var controller = Biba._get('*');
            new Biba(new controller(mainElement));
        };
        xhr.send();
    }

    static inject(name: string, controller: Function) {
        Biba._cache[name] = controller;
    }

    private static _get(name: string) {
        return Biba._cache[name] as any;
    }
}

function Controller(path: string) {
    return function (target, a, b) {
        Biba.inject(path, target);
    }
}