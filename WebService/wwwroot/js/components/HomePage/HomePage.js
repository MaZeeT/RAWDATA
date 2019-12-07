define(['knockout', 'homeService', 'messaging'], function (ko, homeserv, messaging) {

    return function () {

        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);

        let searchResult = ko.observableArray([]);
        let showTable = ko.observable(false);
        let totalResults = ko.observable("0");

        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            }
            
            homeserv.getSearchItems(searchStr, function (responseData) {
                if (responseData) {
                    totalResults(responseData.totalResults);
                    searchResult(responseData.items);
                    showTable(true);
                }
               
            });
        });

        let selectSearchResultItem = function (item) {
            console.log("Item.threadlink is: ", item.threadLink);
            messaging.dispatch(messaging.actions.selectPost(item.threadLink));
            console.log("In between dispatches");
            messaging.dispatch(messaging.actions.selectMenu("postdetails"));
        };








        return {
            searchTerms,
            searchResult,
            showTable,
            totalResults,
            selectSearchResultItem

        }
    }

});