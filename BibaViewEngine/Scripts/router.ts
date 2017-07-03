class BibaRouter {
    constructor() {
        document.addEventListener('DOMContentLoaded', () => this.initRouterLinks());
    }

    initRouterLinks() {
        var routerLinks = document.getElementsByTagName('router-link');
        var allElements = document.body.getElementsByTagName('*');

        for (var i = 0; i < routerLinks.length; i++) {
            var routerLink = routerLinks[i];

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

interface Route<T> {
    path: string;
    args: T | any;
}

new BibaRouter();
