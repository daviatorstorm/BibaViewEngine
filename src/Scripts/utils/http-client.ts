class HttpClient {
    static request<T>(url: string, method: HttpMethod, options?: any): Promise<T | any> {
        return new Promise((resolve, reject) => {
            let xhr = new XMLHttpRequest();

            xhr.open(HttpMethod[method], url, true);

            xhr.onload = (event: any) => {
                if (xhr.status >= 200 && xhr.status < 300)
                    resolve(event.target);
                else
                    reject({
                        status: xhr.status,
                        errorText: xhr.responseText
                    });
            };

            xhr.onerror = (event) => {
                reject({
                    status: xhr.status,
                    errorText: xhr.responseText
                });
            };
            xhr.send(options ? options.data : null);
        });
    }
}

enum HttpMethod {
    GET,
    POST,
    PUT,
    PATCH,
    DELETE,
    OPTIONS,
    HEAD,
    TRACE,
    CONNECT
}