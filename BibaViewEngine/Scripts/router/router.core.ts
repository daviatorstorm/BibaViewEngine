class BibaRouter {
    constructor() {
        document.addEventListener('DOMContentLoaded', () => this.initRouterLinks());
    }

    initRouterLinks() {
        var routerLinks: HTMLElement[] = [];
        var allElements = Array.prototype.slice.call(document.body.getElementsByTagName('*')) as HTMLElement[];

        for (var item of allElements) {
            var attr = item.attributes.getNamedItem('router-link');
            if (attr) {
                routerLinks.push(item);
            }
        }

        for (var routerLink of routerLinks) {
            var newLinkContainer = document.createElement('a');
            newLinkContainer.innerHTML = routerLink.innerHTML;

            newLinkContainer.href = routerLink.attributes.getNamedItem('path').value;

            this.giveAnchorHandler(newLinkContainer);

            routerLink.parentElement.insertBefore(newLinkContainer, routerLink);
            routerLink.parentElement.removeChild(routerLink);
        }

        return {} as any;
    }

    giveAnchorHandler(el: HTMLElement) {
        el.onclick = (e) => this.routerLinkClickHandler(e);
    }

    routerLinkClickHandler(event: Event) {
        console.log(event);

        var routerContainer = document.getElementsByTagName('router-container');

        var template = this.getComponent((event.target as HTMLAnchorElement).attributes.getNamedItem('href').value);

        return false;
    }

    getComponent(path: string, data?: any) {
        var req = new XMLHttpRequest();
        var template = '';

        req.open('POST', `c/${path}`, false);

        req.onload = (event: Event) => {
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
