class BibaRouter {
    private routerContainer: HTMLElement;
    private noop = () => { };
    currentRoute: Route;

    constructor() {
        this.initRouterLinks();
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

            let componentPath = location.pathname;

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
        let componentPath = (event.target as any).path;

        document.dispatchEvent(new CustomEvent('onRouteStart', { detail: { path: componentPath } }));

        this.getComponent(componentPath).then(template => {
            this.routerContainer.innerHTML = template;

            componentPath = componentPath || '/';

            history.pushState({}, document.title, componentPath);

            this.currentRoute = { path: componentPath };

            document.dispatchEvent(new CustomEvent('onRouteFinish', {
                detail: {
                    currentRoute: this.currentRoute,
                    element: this.routerContainer
                }
            }));
        }).catch(console.error);

        return false;
    }

    getComponent(path: string, data?: any): Promise<string> {
        return new Promise((resolve, reject) => {
            let req = new XMLHttpRequest();
            let newPath = `c/${path}`.replace('//', '/')

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
