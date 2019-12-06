define(["knockout", "historyService"], function (ko, ds) {

    return function () {
        var token = window.localStorage.getItem('userToken');

        var page = 1;
        var maxPages = 10;

        var prevPage = function(){
            if (page > 1) page--;
            console.log("page value is: " + page);  //todo remove
        };

        var nextPage = function(){
            page++;
            console.log("page value is: " + page);  //todo remove
        };

        const historyItems = ko.observableArray([]);
        ds.getHistory(token, page, maxPages,function (response) {
            historyItems(response.historyItems);
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
            historyItems,
            deletions
        };


    };

});
