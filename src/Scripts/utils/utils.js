var UrlUtils = /** @class */ (function () {
    function UrlUtils() {
    }
    UrlUtils.join = function () {
        var args = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            args[_i] = arguments[_i];
        }
        var parts = [];
        Array.prototype.slice.call(arguments).forEach(function (element) {
            var subParts = element.split('/');
            subParts.forEach(function (subEl) {
                if (subEl)
                    parts.push(subEl.match(/[\w$-_.+!*'()]+/));
            });
        });
        return parts.join('/');
    };
    return UrlUtils;
}());
