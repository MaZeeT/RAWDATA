define([], function () {
    return function (params) {
        var name = params ? params.name : "noname";
        return {
            name
        };
    };
});