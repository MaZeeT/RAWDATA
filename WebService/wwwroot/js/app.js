define(["knockout"], function (ko) {
 
    var currentComponent = ko.observable("index");
    var currentParams = ko.observable({});

    var onMenuItemClick = function (componentName) {
        currentComponent(componentName);
    }


    return {
        currentComponent,
        currentParams,
        onMenuItemClick
    };
});