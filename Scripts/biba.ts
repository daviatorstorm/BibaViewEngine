class Biba {
    router: BibaRouter;

    private constructor(private closure: Closure) {
        this.router = new BibaRouter();
        
        this.router.onRouteStart((args) => {
            console.log('Route start', args);
        });

        this.router.onRouteFinish((args) => {
            console.log('Route fisnish', args);
        });
    }

    static Start(startComponent: string) {
        var mainElement = document.getElementsByTagName(startComponent)[0];

        var xhr = new XMLHttpRequest();
        xhr.open('Get', '/app/start');
        xhr.onloadend = (res: any) => {
            mainElement.innerHTML = res.target.response; // Set compiled html from server
            new Biba({}); // Pass closure
        };
        xhr.send();
    }
}