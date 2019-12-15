
require.config({
    baseUrl: "js",
    paths: {
        jquery: "../jquery/dist/jquery",
        knockout: "../lib/knockout/build/output/knockout-latest.debug",
        text: "../lib/requirejs-text/text",
        jqcloud: "../lib/jqcloud2/dist/jqcloud",
        messaging: "services/MessagingService",
        historyService: "services/HistoryService",
        searchHistoryService: "services/SearchHistoryService",
        //historyService: "services/HistoryServiceMock",    //Mock of historyService
        authservice: "services/AuthenticationService",
        bookmarksService: "services/BookmarksService",
        browseService: "services/BrowseService",
        wordCloudService: "services/WordCloudService",
        annotationsService: "services/AnnotationsService",
        homeService: "services/HomePageService",
        postservice: "services/PostService",
        util: "utils/util",
        validation: "utils/validation"
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
    ko.components.register('browse', {
        viewModel: { require: "components/BrowsePage/BrowsePage" },
        template: { require: "text!components/BrowsePage/BrowsePage.html" }
    });
    ko.components.register('wordcloud', {
        viewModel: { require: "components/WordCloudPage/WordCloudPage" },
        template: { require: "text!components/WordCloudPage/WordCloudPage.html" }
    });
    ko.components.register('bookmarks', {
        viewModel: { require: "components/BookmarksPage/BookmarksPage" },
        template: { require: "text!components/BookmarksPage/BookmarksPage.html" }
    });
    ko.components.register('searchhistory', {
        viewModel: { require: "components/SearchHistoryPage/SearchHistoryPage" },
        template: { require: "text!components/SearchHistoryPage/SearchHistoryPage.html" }
    });
    ko.components.register('authentication', {
        viewModel: { require: "components/AuthenticationPage/AuthenticationPage" },
        template: { require: "text!components/AuthenticationPage/AuthenticationPage.html" }
    });
    ko.components.register('postdetails', {
        viewModel: { require: "components/PostDetails/PostDetails" },
        template: { require: "text!components/PostDetails/PostDetails.html" }
    });
    ko.components.register('prebuttcomp', {
      //  viewModel: { require: "components/PreviousButtonComponent/PreviousButtonComponent" },
        template: { require: "text!components/PreviousButtonComponent/PreviousButtonComponent.html" }
    });
    ko.components.register('searchbuttcomp', {
       // viewModel: { require: "components/SearchButtonComponent/SearchButtonComponent" },
        template: { require: "text!components/SearchButtonComponent/SearchButtonComponent.html" }
    });
    ko.components.register('hisbuttcomp', {
       // viewModel: { require: "components/HistoryButtonComponent/HistoryButtonComponent" },
        template: { require: "text!components/HistoryButtonComponent/HistoryButtonComponent.html" }
    });
});

require(["knockout", "messaging", "navbar"], function (ko, messaging, app) {
    messaging.subscribe(() => console.log(messaging.getState()));
    // enable validation
    ko.applyBindings(app);
});