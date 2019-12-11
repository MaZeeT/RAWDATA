define(["knockout", "historyService"], function (ko, ds) {

    return function () {
        var token = window.localStorage.getItem('userToken');

        var page = ko.observable(2);
        var maxPages = ko.observable(10);

        var prevPage = function (pageVal){
            if (page > 1) page--;
            console.log("page value is: " + pageVal);  //todo remove
        };

        var nextPage = function(value){
            page++;
            console.log("page value is: " + value);  //todo remove
        };

        var historyItems = ko.observableArray([]);
        ds.getHistory(token, page, maxPages,function (response) {
            historyItems(response);
        });

        var deletions = function () {
            ds.deleteHistory(token, function (response) {
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
