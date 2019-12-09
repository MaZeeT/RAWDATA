define(['knockout', 'homeService', 'messaging'], function (ko, homeserv, messaging) {

    return function () {

        let pageSizeSelection = ko.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let getPageSize = ko.observable(5);
        let currentPage = ko.observable(1);

        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);
        let searchstring = ko.observable("");


        let selectedPageSize = ko.observable();

        let searchResult = ko.observableArray([]);
        let showTable = ko.observable(false);
        let totalResults = ko.observable("0");

        

        ///////////////////////////////////////////////////////////////////////////////////////////


        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            }

            searchstring(searchStr);

            console.log("I am in search ", searchStr);
            console.log("I am in givenPageSize ", getPageSize());
            console.log("I am in givenPageNumber ", currentPage());
            const searchString = "?s=" + searchStr;
            const searchType = "&stype=0";
            const pageSize = "&pageSize=" + getPageSize();
            const pageNumber = "&page=" + currentPage();
            
            homeserv.getSearchItems(searchString, searchType, pageSize, pageNumber, function (responseData) {
                if (responseData) {
                    console.log("Responsedata from homeage is: ", responseData);
                    totalResults(responseData.totalResults);
                    searchResult(responseData.items);
                    console.log(searchResult());
                    showTable(true);
                }
               
            });
        });

        selectedPageSize.subscribe(function (value) {
            console.log("My val is an array ", value[0]);
            getPageSize(value[0]);
            let searchStr = searchstring();
            if (searchStr) {

                console.log("I am in search ", searchStr);
                console.log("I am in givenPageSize ", getPageSize());
                console.log("I am in givenPageNumber ", currentPage());
                const searchString = "?s=" + searchStr;
                const searchType = "&stype=0";
                const pageSize = "&pageSize=" + getPageSize();
                const pageNumber = "&page=" + currentPage();

                homeserv.getSearchItems(searchString, searchType, pageSize, pageNumber, function (responseData) {
                    if (responseData) {
                        console.log("Responsedata from homeage is: ", responseData);
                        totalResults(responseData.totalResults);
                        searchResult(responseData.items);
                        console.log(searchResult());
                        showTable(true);
                    }

                });

            }
        });
        let next = function () {
            console.log("currentPage page on next", currentPage());
            currentPage(currentPage() + 1);
            console.log("currentPage page oafter change next", currentPage());

            let searchStr = searchstring();
            if (searchStr) {

                console.log("I am in search ", searchStr);
                console.log("I am in givenPageSize ", getPageSize());
                console.log("I am in givenPageNumber ", currentPage());
                const searchString = "?s=" + searchStr;
                const searchType = "&stype=0";
                const pageSize = "&pageSize=" + getPageSize();
                const pageNumber = "&page=" + currentPage();

                homeserv.getSearchItems(searchString, searchType, pageSize, pageNumber, function (responseData) {
                    if (responseData) {
                        console.log("Responsedata from homeage is: ", responseData);
                        totalResults(responseData.totalResults);
                        searchResult(responseData.items);
                        console.log(searchResult());
                        showTable(true);
                    }

                });

            }
        }
        let prev = function () {
            console.log("currentPage page on prev", currentPage());
            const pageValueUpdated = currentPage();
            if (pageValueUpdated > 1) {
                currentPage(pageValueUpdated - 1);
            }
            console.log("currentPage page after change prev", currentPage());
        }


        let callService = function (searchString, searchType, pageSize, pageNumber) {

            return homeserv.getSearchItems(searchString, searchType, pageSize, pageNumber, function (responseData) {
                if (responseData) {
                    console.log("Responsedata from homeage is: ", responseData);
                    totalResults(responseData.totalResults);
                    searchResult(responseData.items);
                    console.log(searchResult());
                    showTable(true);
                }

            });
        }

       

        let selectSearchResultItem = function (item) {
            console.log("Item.threadlink is: ", item.threadLink);
            console.log("Item is: ", item);
            messaging.dispatch(messaging.actions.selectPost(item.threadLink));
            console.log("In between dispatches");
            messaging.dispatch(messaging.actions.selectMenu("postdetails"));
        };


        return {
            searchTerms,
            searchResult,
            showTable,
            totalResults,
            selectSearchResultItem,
            //getPg,
            //pageSizeSelection,
            //getpgsize,
            //pgsizechanged,
            currentPage,
            next,
            prev,
            pageSizeSelection,
            getPageSize,
            selectedPageSize

        }
    }

});