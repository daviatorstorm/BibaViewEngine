class UrlUtils {
    static join(...args: string[]) {
        let parts = [];
        Array.prototype.slice.call(arguments).forEach((element: string) => {
            var subParts = element.split('/');
            subParts.forEach((subEl: string) => {
                if (subEl) parts.push(subEl.match(/[\w$-_.+!*'()]+/));
            });
        });

        return parts.join('/');
    }
}

interface NodeListOf<TNode extends Node> extends NodeList {
    forEach: (callback: (currentValue?: TNode, currenIndex?: number, listObj?: NodeListOf<TNode>) => void) => void;
}