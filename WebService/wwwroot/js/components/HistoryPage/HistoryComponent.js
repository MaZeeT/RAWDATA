define(["knockout", "historyService"], function (ko, ds) {

    return function () {

        var page = 1;

        var prevPage = function(){
            if (page > 1) page--;
            console.log("page value is: " + page);  //todo remove
        };

        var nextPage = function(){
            page++;
            console.log("page value is: " + page);  //todo remove
        };

        var historyItems = ko.observableArray([]);
        ds.getHistory("goat", page,function (response) {
            historyItems(response.historyItems);
        });

        var deletions = function () {
            ds.deleteHistory("goat", function (response) {
                return response;
            })
        };

        return {
            prevPage,
            nextPage,
            historyItems,
            deletions
        }
    }

});
