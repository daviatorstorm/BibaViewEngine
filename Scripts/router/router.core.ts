class BibaRouter {
    private routerContainer: HTMLElement;
    private _currentRoute: Route;
    baseUrl: string;

    public get currentRoute(): Route {
        return Biba._get('currentRoute');
    }

    constructor(baseUrl: string, private rootElement = document.documentElement) {
        this.baseUrl = baseUrl || '/';
    }

    route(path = this.baseUrl) {
        path = path || this.baseUrl;

        if (this.currentRoute && path == this.currentRoute.path)
            return;

        document.dispatchEvent(new CustomEvent('onRouteStart', { detail: { path } }));

        return this.getComponent(path).then((response: any) => {
            let data = JSON.parse(response.response);
            this.routerContainer.innerHTML = data.html;
            Biba.inject('scope', data.scope);

            history.pushState({}, document.title, path);
            var newRouter = new BibaRouter(location.pathname, this.routerContainer);
            if (!newRouter.initRouterLinks()) {
                newRouter = null;
                this.initRouterLinks();
            }

            this.currentRoute.path = path;

            document.dispatchEvent(new CustomEvent('onRouteFinish', {
                detail: {
                    currentRoute: this.currentRoute,
                    element: this.routerContainer
                }
            }));

            return response;
        });
    }

    initRouterLinks(): boolean {
        let hasChildContainer = false;
        var rc = this.rootElement.querySelector('[router-container]');

        if (rc) {
            this.routerContainer = rc as HTMLElement;
            this.routerContainer.attributes.removeNamedItem('router-container');
            hasChildContainer = true;
        }

        let allElements = this.rootElement.querySelectorAll('[router-path]');

        allElements.forEach((item) => {
            let path = item.attributes.getNamedItem('router-path').value;
            if (path[0] != '/' && hasChildContainer)
                path = UrlUtils.join(this.baseUrl, path);
            (item as any).path = path;
            item.attributes.removeNamedItem('router-path');
            this.giveAnchorHandler(item as HTMLElement);
        });

        return hasChildContainer;
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
                if (req.status >= 200 && req.status < 300)
                    resolve(event.target);
                else
                    reject({
                        status: req.status,
                        errorText: req.responseText
                    });
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
