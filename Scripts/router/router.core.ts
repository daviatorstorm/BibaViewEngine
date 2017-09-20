class BibaRouter {
    private routerContainer: HTMLElement;
    private noop = () => { };
    onRouteStart: RouteEvent;
    onRouteFinish: RouteEvent;
    currentRoute: Route;

    constructor() {
        document.addEventListener('DOMContentLoaded', () => this.initRouterLinks());
        this.onRouteStart = this.noop;
        this.onRouteFinish = this.noop;
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
            this.getComponent(location.pathname).then((template: string) => {
                this.routerContainer.innerHTML = template;
            });
        }

        return {} as any;
    }

    private giveAnchorHandler(el: HTMLElement) {
        el.onclick = (e) => this.routerLinkClickHandler(e);
    }

    private routerLinkClickHandler(event: Event) {
        let componentPath = (event.target as any).path;

        this.onRouteStart(this.currentRoute, { path: componentPath }, this);

        this.getComponent(componentPath).then(template => {
            this.routerContainer.innerHTML = template;

            history.pushState({}, document.title, componentPath || '/');

            this.currentRoute = { path: componentPath };

            this.onRouteFinish({}, this.currentRoute, this);
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
}

new BibaRouter();
