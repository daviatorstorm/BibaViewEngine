class BibaRouter {
    private routerContainer: HTMLElement;
    currentRoute: Route;

    constructor() {
        this.initRouterLinks();
    }

    route(path: string) {
        document.dispatchEvent(new CustomEvent('onRouteStart', { detail: { path } }));

        this.getComponent(path).then(template => {
            this.routerContainer.innerHTML = template;

            path = path || '/';

            history.pushState({}, document.title, path);

            this.currentRoute = { path: path };

            document.dispatchEvent(new CustomEvent('onRouteFinish', {
                detail: {
                    currentRoute: this.currentRoute,
                    element: this.routerContainer
                }
            }));
        }).catch(console.error);
    }

    private initRouterLinks() {
        var allElements = Array.prototype.slice.call(document.body.getElementsByTagName('*')) as HTMLElement[];

        for (var item of allElements) {
            let attr = item.attributes.getNamedItem('router-path');
            if (attr) {
                (item as any).path = item.attributes.getNamedItem('router-path').value;
                item.attributes.removeNamedItem('router-path');
                this.giveAnchorHandler(item);
            } else if (item.attributes.getNamedItem('router-container')) {
                this.routerContainer = item;
                this.routerContainer.attributes.removeNamedItem('router-container');
            }
        }

        if (this.routerContainer) {
            document.dispatchEvent(new CustomEvent('onRouteStart', { detail: { path: location.pathname } }));

            var componentPath = location.pathname;

            this.getComponent(componentPath).then((template: string) => {
                this.routerContainer.innerHTML = template;

                this.currentRoute = { path: componentPath };

                document.dispatchEvent(new CustomEvent('onRouteFinish', { detail: { currentRoute: this.currentRoute } }));
            });
        }

        return {} as any;
    }

    private giveAnchorHandler(el: HTMLElement) {
        el.onclick = (e) => this.routerLinkClickHandler(e);
    }

    private routerLinkClickHandler(event: Event) {
        var componentPath = (event.target as any).path;

        this.route(componentPath);

        return false;
    }

    getComponent(path: string, data?: any): Promise<string> {
        return new Promise((resolve, reject) => {
            var req = new XMLHttpRequest();
            var newPath = `c/${path}`.replace('//', '/')

            req.open('POST', newPath, true);

            req.onload = (event: any) => {
                resolve(event.target.response);
            };

            req.onerror = (event) => {
                reject(event);
            };

            req.send(data || null);
        });
    }

    onRouteStart(handler: { (args: any): void }) {
        document.addEventListener('onRouteStart', (args: CustomEvent) => { handler(args.detail) }, false);
    }

    onRouteFinish(handler: { (args: any): void }) {
        document.addEventListener('onRouteFinish', (args: CustomEvent) => { handler(args.detail) }, false);
    }
}
