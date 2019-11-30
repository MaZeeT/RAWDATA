define(["knockout"], function (ko) {
    var currentComponent = ko.observable("page1");
    var currentParams = ko.observable({});
    var changeContent = () => {
        if (currentComponent() === "page1") {
            currentComponent("HistoryComponent");
        } else {
            currentComponent("page1");
        }
    };

    return {
        currentComponent,
        currentParams,
        changeContent
    };
});