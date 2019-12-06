define(["knockout", "historyService"], function (ko, ds) {

    return function () {

        var historyItems = ko.observableArray([]);
        ds.getHistory("goat", function (response) {
            historyItems(response.historyItems);
        });

        var deletions = function () {
            ds.deleteHistory("goat", function (response) {
                return response;
            })
        };

        return {
            historyItems,
            deletions
        }
    }

});
