
require.config({
    baseUrl: "js",
    paths: {
        jquery: "../jquery/dist/jquery",
        knockout: "../lib/knockout/build/output/knockout-latest.debug",
        text: "../lib/requirejs-text/text",
        messaging: "services/messagingService",
        historyService: "services/HistoryService",
        authservice: "services/AuthenticationService",
        bookmarksService: "services/BookmarksService",
        annotationsService: "services/AnnotationsService",
        homeService: "services/HomePageService"
    }
});

require(["knockout"], function (ko) {
    ko.components.register('homepage', {
        viewModel: { require: "components/HomePage/HomePage" },
        template: { require: "text!components/HomePage/HomePage.html" }
    });
    ko.components.register('history', {
        viewModel: { require: "components/HistoryPage/HistoryComponent" },
        template: { require: "text!components/HistoryPage/HistoryComponent.html" }
    });
    ko.components.register('annotations', {
        viewModel: { require: "components/AnnotationsPage/AnnotationsPage" },
        template: { require: "text!components/AnnotationsPage/AnnotationsPage.html" }
    });
    ko.components.register('bookmarks', {
        viewModel: { require: "components/BookmarksPage/BookmarksPage" },
        template: { require: "text!components/BookmarksPage/BookmarksPage.html" }
    });
    ko.components.register('authentication', {
        viewModel: { require: "components/AuthenticationPage/AuthenticationPage" },
        template: { require: "text!components/AuthenticationPage/AuthenticationPage.html" }
    });
   
});

require(["knockout", "messaging", "navbar"], function (ko, messaging, app) {
    messaging.subscribe(() => console.log(messaging.getState()));
    ko.applyBindings(app);
});