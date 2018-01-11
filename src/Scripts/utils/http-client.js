var HttpClient = /** @class */ (function () {
    function HttpClient() {
    }
    HttpClient.request = function (url, method, options) {
        return new Promise(function (resolve, reject) {
            var xhr = new XMLHttpRequest();
            xhr.open(HttpMethod[method], url, true);
            xhr.onload = function (event) {
                if (xhr.status >= 200 && xhr.status < 300)
                    resolve(event.target);
                else
                    reject({
                        status: xhr.status,
                        errorText: xhr.responseText
                    });
            };
            xhr.onerror = function (event) {
                reject({
                    status: xhr.status,
                    errorText: xhr.responseText
                });
            };
            xhr.send(options ? options.data : null);
        });
    };
    return HttpClient;
}());
var HttpMethod;
(function (HttpMethod) {
    HttpMethod[HttpMethod["GET"] = 0] = "GET";
    HttpMethod[HttpMethod["POST"] = 1] = "POST";
    HttpMethod[HttpMethod["PUT"] = 2] = "PUT";
    HttpMethod[HttpMethod["PATCH"] = 3] = "PATCH";
    HttpMethod[HttpMethod["DELETE"] = 4] = "DELETE";
    HttpMethod[HttpMethod["OPTIONS"] = 5] = "OPTIONS";
    HttpMethod[HttpMethod["HEAD"] = 6] = "HEAD";
    HttpMethod[HttpMethod["TRACE"] = 7] = "TRACE";
    HttpMethod[HttpMethod["CONNECT"] = 8] = "CONNECT";
})(HttpMethod || (HttpMethod = {}));
