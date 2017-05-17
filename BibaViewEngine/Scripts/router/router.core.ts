class BibaRouter {
    routerContainer: HTMLElement;

    constructor() {
        document.addEventListener('DOMContentLoaded', () => this.initRouterLinks());
    }

    initRouterLinks() {
        var routerLinks: HTMLElement[] = [];
        var allElements = Array.prototype.slice.call(document.body.getElementsByTagName('*')) as HTMLElement[];

        for (var item of allElements) {
            var attr = item.attributes.getNamedItem('router-path');
            if (attr) {
                routerLinks.push(item);
            } else if (item.attributes.getNamedItem('router-container')) {
                this.routerContainer = item;
                this.routerContainer.attributes.removeNamedItem('router-container');
            }
        }

        for (var routerLink of routerLinks) {
            var newLinkContainer = document.createElement('a');
            newLinkContainer.innerHTML = routerLink.innerHTML;

            newLinkContainer.href = routerLink.attributes.getNamedItem('router-path').value;

            this.giveAnchorHandler(newLinkContainer);

            routerLink.parentElement.insertBefore(newLinkContainer, routerLink);
            routerLink.parentElement.removeChild(routerLink);
        }

        this.routerContainer.innerHTML = this.getComponent(location.pathname);

        return {} as any;
    }

    giveAnchorHandler(el: HTMLElement) {
        el.onclick = (e) => this.routerLinkClickHandler(e);
    }

    routerLinkClickHandler(event: Event) {
        console.log(event);

        var routerContainer = document.getElementsByTagName('router-container');

        this.routerContainer.innerHTML = this.getComponent((event.target as HTMLAnchorElement).attributes.getNamedItem('href').value);

        return false;
    }

    getComponent(path: string, data?: any) {
        let req = new XMLHttpRequest();
        let template = '';
        let newPath = `c/${path}`.replace('//', '/')

        req.open('POST', newPath, false);

        req.onload = (event: any) => {
            template = event.target.response;
            console.log(event);
        };

        req.onerror = (event) => {
            console.log(event);
        };

        req.send(data || null);

        return template;
    }
}

//interface Route<T> {
//    path: string;
//    args: T | any;
//}

new BibaRouter();
