class ViewController {
    scope: any;

    constructor(public element: HTMLElement) {
        this.scope = Biba._get('scope');
    }
}