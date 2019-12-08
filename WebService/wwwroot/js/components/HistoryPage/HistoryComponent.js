define(["knockout", "historyService"], function (ko, ds) {

    return function () {
        var token = window.localStorage.getItem('userToken');

        var page = 2;
        var maxPages = ko.observable(10);
        var totalPages = ko.observable();
        var prevUrl = ko.observable();
        var nextUrl = ko.observable();
        console.log("maxpage value is: " + maxPages());  //todo remove
        
        var prevPage = function(){
            if (page > 1) page--;
            console.log("page value is: " + page);  //todo remove
            console.log("maxpage value is: " + maxPages());  //todo remove
        };

        var nextPage = function(){
            page++;
            console.log("page value is: " + page);  //todo remove
        };
        
        var historyItems = ko.observableArray();
        var items = ko.observableArray();
        console.log("page value is: " + page);
        ds.getHistory(token, page, maxPages, function (response) {
            historyItems(response);
            totalPages(response.numberOfPages);
            prevUrl(response.prev);
            nextUrl(response.next);
            items(response.items);
        });

        var deletions = function () {
            ds.deleteHistory("goat", function (response) {
                return response;
            })
        };
        
        return {
            maxPages,
            prevPage,
            nextPage,
            items,
            deletions
        };
        

    };

});
