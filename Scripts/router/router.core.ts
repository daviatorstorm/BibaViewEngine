class BibaRouter {
    private routerContainer: HTMLElement;
    currentRoute: Route;

    constructor() {
        this.initRouterLinks();
    }

    route(path: string) {
        path = path === '' ? '/' : path;

        if (this.currentRoute && path == this.currentRoute.path) {
            return;
        }

        document.dispatchEvent(new CustomEvent('onRouteStart', { detail: { path } }));

        return this.getComponent(path).then((response: any) => {
            let data = JSON.parse(response.response);
            this.routerContainer.innerHTML = data.html;
            Biba.inject('scope', data.scope);
            
            path = path || '/';
            
            history.pushState({}, document.title, path);
            this.initRouterLinks(this.routerContainer);

            this.currentRoute = { path: path };

            document.dispatchEvent(new CustomEvent('onRouteFinish', {
                detail: {
                    currentRoute: this.currentRoute,
                    element: this.routerContainer
                }
            }));

            return response;
        });
    }

    private initRouterLinks(element?: HTMLElement): void {
        let allElements = [];
        if (!element) {
            allElements = Array.prototype.slice.call(document.body.getElementsByTagName('*')) as HTMLElement[];
        } else {
            allElements = Array.prototype.slice.call(element.getElementsByTagName('*')) as HTMLElement[]
        }
        
        // Implement parent url join

        for (let item of allElements) {
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
            var path = location.pathname;

            document.dispatchEvent(new CustomEvent('onRouteStart', { detail: { path } }));

            this.route(path).catch(err => {
                console.log(`Cannot go to ${path}. Error:`, err);
            });
        }
    }

    private giveAnchorHandler(el: HTMLElement) {
        el.onclick = (e) => this.routerLinkClickHandler(e);
    }

    private routerLinkClickHandler(event: Event) {
        var componentPath = (event.target as any).path;

        this.route(componentPath);

        return false;
    }

    private getComponent(path: string, data?: any): Promise<string> {
        return new Promise((resolve, reject) => {
            let req = new XMLHttpRequest();
            let newPath = `c/${path}`.replace('//', '/')

            req.open('POST', newPath, true);

            req.onload = (event: any) => {
                if (req.status >= 200 && req.status < 300) {
                    resolve(event.target);
                } else {
                    reject({
                        status: req.status,
                        errorText: req.responseText
                    });
                }
            };

            req.onerror = (event) => {
                reject({
                    status: req.status,
                    errorText: req.responseText
                });
            };

            req.send(data);
        });
    }

    onRouteStart(handler: { (args: any): void }) {
        document.addEventListener('onRouteStart', (args: CustomEvent) => { handler(args.detail) }, false);
    }

    onRouteFinish(handler: { (args: any): void }) {
        document.addEventListener('onRouteFinish', (args: CustomEvent) => { handler(args.detail) }, false);
    }
}
