
require.config({
    baseUrl: "js",
    paths: {
        jquery: "../jquery/dist/jquery",
        knockout: "../lib/knockout/build/output/knockout-latest.debug",
        text: "../lib/requirejs-text/text",
        historyService: "services/HistoryService",
        authservice: "services/AuthenticationService",
        bookmarksService: "services/BookmarksService",
        homepageService: "services/HomePageService",
        annotationsService: "services/AnnotationsService"
    }
});

require(["knockout"], function (ko) {
    ko.components.register('history', {
        viewModel: { require: "components/HistoryPage/HistoryComponent" },
        template: { require: "text!components/HistoryPage/HistoryComponent.html" }
    });
    ko.components.register('annotations', {
        viewModel: { require: "components/AnnotationsPage/AnnotationsPage" },
        template: { require: "text!components/AnnotationsPage/HistoryComponent.html" }
    });
    ko.components.register('bookmarks', {
        viewModel: { require: "components/BookmarksPage/BookmarksPage" },
        template: { require: "text!components/BookmarksPage/BookmarksPage.html" }
    });
    ko.components.register('homepage', {
        viewModel: { require: "components/HomePage/HomePage" },
        template: { require: "text!components/HomePage/HomePage.html" }
    });
});

require(["knockout", "app"], function (ko, app) {
    ko.applyBindings(app);
});