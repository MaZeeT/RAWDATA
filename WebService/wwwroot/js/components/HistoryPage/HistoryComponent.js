define(["knockout", "historyService"], function (ko, ds) {

    return function () {
        var token = window.localStorage.getItem('userToken');
        
        var maxPages = ko.observable(15);
        var totalPages = ko.observable();
        var prevUrl = null;
        var nextUrl = null;
        var items = ko.observableArray();
        console.log("maxpage value is: " + maxPages());  //todo remove

        var getData = function (url) {
            ds.getHistory(token, url, function (response) {
                totalPages(response.numberOfPages);
                prevUrl = response.prev;
                nextUrl = response.next;
                items(response.items);
            });
        };

        var page = 1;
        var url = ds.buildUrl(page, maxPages());
        getData(url);

        var prevPage = function () {
            if (prevUrl != null){
                getData(prevUrl);    
            }
        };

        var nextPage = function () {
            if (nextUrl != null) {
                getData(nextUrl);
            }
        };

        var deletions = function () {
            ds.deleteHistory("goat", function (response) {
                return response;
            })
        };

        return {
            maxPages,
            items,
            prevPage,
            nextPage,
            deletions
        };
        
    };
});
