
require.config({
    baseUrl: "js",
    paths: {
        jquery: "../jquery/dist/jquery",
        knockout: "../lib/knockout/build/output/knockout-latest.debug",
        text: "../lib/requirejs-text/text",
        dataservice: "services/HistoryService",
        authservice: "services/AuthenticationService"

    }
});

require(["knockout"], function (ko) {
    ko.components.register('mycomp', {
        viewModel: { require: "viewModel" },
        template: { require: "text!../mycomp.html" }
    });
    ko.components.register('nav-bar', {
       // viewModel: { require: "components/navigation-menu/nav" },
        template: { require: "text!components/navigation-menu/nav.html" }
    });
    ko.components.register('index', {
        viewModel: { require: "components/page1/page1" },
        template: { require: "text!components/page1/page1.html" }
    });
    ko.components.register('page2', {
        viewModel: { require: "components/page2/page2" },
        template: { require: "text!components/page2/page2.html" }
    });
    ko.components.register('history', {
        viewModel: { require: "components/HistoryPage/HistoryComponent" },
        template: { require: "text!components/HistoryPage/HistoryComponent.html" }
    });
});



require(["knockout", "app"], function (ko, app) {
    //console.log(app.name);

    ko.applyBindings(app);
});