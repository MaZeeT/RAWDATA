define(['knockout', 'homeService', 'messaging', 'util'], function (ko, homeserv, messaging, util) {

    return function () {

        let pageSizeSelection = ko.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let getPageSize = ko.observable(5);
        let selectedPageSize = ko.observable();

        let searchTypeValSelector = ko.observableArray(['TFIDF', 'Exact Match', 'Simple Match', 'Best Match']); //selection of pagesizes
        let searchTypeValue = ko.observable(0);
        let selectedSearchType = ko.observable();

        let currentPage = ko.observable(1);

        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);
        let searchstring = ko.observable("");

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
            let givenSearchType = searchTypeSelectorMapping(searchTypeValue());
            let object = conputeUrlStringWithPagination(searchstring(), givenSearchType, getPageSize(), currentPage());
            console.log("Computed object is now: ", object);
            
            homeserv.getSearchItems(object, function (responseData) {
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
            getPageSize(value[0]);
            if (searchstring()) {

                let givenSearchType = searchTypeSelectorMapping(searchTypeValue());
                let object = conputeUrlStringWithPagination(searchstring(), givenSearchType, getPageSize(), currentPage());
                console.log("Computed object is now: ", object);

                homeserv.getSearchItems(object, function (responseData) {
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

        selectedSearchType.subscribe(function (value) {
            console.log("My val is an array ", value[0]);
            searchTypeValue(value[0]);

            if (searchstring()) {

                let givenSearchType = searchTypeSelectorMapping(searchTypeValue());
                let object = conputeUrlStringWithPagination(searchstring(), givenSearchType, getPageSize(), currentPage());
                console.log("Computed object is now: ", object);

                homeserv.getSearchItems(object, function (responseData) {
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

                let givenSearchType = searchTypeSelectorMapping(searchTypeValue());
                let object = conputeUrlStringWithPagination(searchstring(), givenSearchType, getPageSize(), currentPage());
                console.log("Computed object is now: ", object);

                homeserv.getSearchItems(object, function (responseData) {
                    if (responseData) {
                        console.log("Responsedata from homeage is: ", responseData);
                        totalResults(responseData.totalResults);
                        searchResult(responseData.items);
                        console.log(searchResult());
                        showTable(true);
                    }

                });

                //util.callService(searchString, searchType, pageSize, pageNumber);

            }
        }
        let prev = function () {
            console.log("currentPage page on prev", currentPage());
            const pageValueUpdated = currentPage();
            if (pageValueUpdated > 1) {
                currentPage(pageValueUpdated - 1);
            }
            console.log("currentPage page after change prev", currentPage());

            if (searchstring()) {

                let givenSearchType = searchTypeSelectorMapping(searchTypeValue());
                let object = conputeUrlStringWithPagination(searchstring(), givenSearchType, getPageSize(), currentPage());
                console.log("Computed object is now: ", object);

                homeserv.getSearchItems(object, function (responseData) {
                    if (responseData) {
                        console.log("Responsedata from homeage is: ", responseData);
                        totalResults(responseData.totalResults);
                        searchResult(responseData.items);
                        console.log(searchResult());
                        showTable(true);
                    }

                });

                //util.callService(searchString, searchType, pageSize, pageNumber);

            }
        }

        let selectSearchResultItem = function (item) {
            console.log("Item.threadlink is: ", item.threadLink);
            console.log("Item is: ", item);
            messaging.dispatch(messaging.actions.selectPost(item.threadLink));
            console.log("In between dispatches");
            messaging.dispatch(messaging.actions.selectMenu("postdetails"));
        };

       
        ////Function that builds the URL - could be taken out in a utils file/ folder and used wherever in the code needed. 
        function conputeUrlStringWithPagination(searchStr, searchTypeVal, pageItemSize, pageNo) {
            const searchString = searchStr ? "?s=" + searchStr : "";
            const searchType = searchTypeVal ? "&stype=" + searchTypeVal : "&stype=0";
            const pageSize = pageItemSize ? "&pageSize=" + pageItemSize : "&pageSize=5";
            const pageNumber = pageNo ? "&page=" + pageNo : "&page=1";

            let paginationObject = {
                searchString,
                searchType,
                pageSize,
                pageNumber
            }
            return paginationObject;
        }

        function searchTypeSelectorMapping(value) {
            switch (value) {
                case "TFIDF":
                    return 0;
                    break;
                case "Exact Match":
                    return 1;
                    break;
                case "Simple Match":
                    return 2;
                    break;
                case "Best Match":
                    return 3;
                    break;
                default:
                    return 0;
            }
        }






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
            selectedPageSize,
            searchTypeValSelector,
            searchTypeValue,
            selectedSearchType

        }
    }

});