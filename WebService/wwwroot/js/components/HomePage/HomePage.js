define(['knockout', 'homeService', 'messaging', 'util'], function (ko, homeserv, messaging, util) {

    return function () {

        //Pagination
        let pageSizeSelection = ko.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let selectedPageSize = ko.observable();
        let getPageSize = ko.observable(10);

        let currentPage = ko.observable(1);
        let numberOfPages = ko.observable();

        let nexturi = '666'; //placeholder for grabbing querystring page= value
        let prevuri = '666'; //placeholder for grabbing querystring page= value

        //Other dropdowns
        let searchTypeValSelector = ko.observableArray(["TFIDF", "Exact Match", "Simple Match", "Best Match"]); //selection of searchtypes
        let searchTypeValue = ko.observable("Best Match");
        let selectedSearchType = ko.observable();

        
        //Search
        const placeholderStr = "Input search terms here..."
        let searchTerms = ko.observable(placeholderStr);
        let searchstring = ko.observable("");

        let searchResult = ko.observableArray([]);
        let showTable = ko.observable(false);
        let totalResults = ko.observable("0");

        ///////////////////////////////////////////////////////////////////////////////////////////

        //Pasing the linkthread url from homepage component & navigating to postdetails page;
        let selectSearchResultItem = function (item) {
            saveStuff();
            messaging.dispatch(messaging.actions.selectPost(item.threadLink));
            messaging.dispatch(messaging.actions.selectMenu("postdetails"));
        };

        let clearInputField = function () {
            if (searchTerms() === placeholderStr) {
                searchTerms('');
            }
        }

        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            }
            searchstring(searchStr);
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());

        });

        selectedPageSize.subscribe(function () {

            console.log("selectedPageSize: ", getPageSize());
           // getPageSize();
        /*  getPageSize(value[0]);*/
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());
        });

        selectedSearchType.subscribe(function () {

            console.log("selectedSearchType: ", searchTypeValue());
           // searchTypeValue();
        /* searchTypeValue(value[0]);*/
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());

        });

        //grab/refresh data when page change
        function getPg(direction) {
            let npg = null;
            if (direction === 'next') {
                npg = util.getParameterByName('page', nexturi);
            } else if (direction === 'prev') { npg = util.getParameterByName('page', prevuri); }

            console.log("dat: ", direction);
            console.log("param: ", npg);
            if (npg) {
                //getBrowsing(npg, ps);

                callService(searchstring(), searchTypeValue(), getPageSize(), npg);
            };
        };


   /*     let next = function () {
            console.log("currentPage page on next", currentPage());
            currentPage(currentPage() + 1);
            console.log("currentPage page oafter change next", currentPage());

            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());
        }
        let prev = function () {
            const pageValueUpdated = currentPage();
            if (pageValueUpdated > 1) {
                currentPage(pageValueUpdated - 1);
            }
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());
        }*/


        function callService(searchString, srcTypeVal, pageSize, currPage) {
            if (searchString) {

                let givenSearchType = util.searchTypeSelectorMapping(srcTypeVal);
                let object = util.conputeUrlStringWithPagination(searchString, givenSearchType, pageSize, currPage);
                console.log("Computed object is now: ", object);

                homeserv.getSearchItems(object, function (responseData) {
                    if (responseData) {
                        console.log("Responsedata from homeage is: ", responseData);

                        currentPage(currPage);
                        totalResults(responseData.totalResults);
                        searchResult(responseData.items);
                        numberOfPages(responseData.numberOfPages)
                        nexturi = responseData.next;
                        prevuri = responseData.prev;
                        console.log(searchResult());
                        showTable(true);
                        saveStuff();
                    }

                });
            }
        }


        //store stuff from this view
        function saveStuff() {
            messaging.dispatch(messaging.actions.selectSearchTerms(searchTerms()));
            messaging.dispatch(messaging.actions.selectSearchOptions(searchTypeValue()));
            messaging.dispatch(messaging.actions.selectCurrentPage(currentPage()));
            messaging.dispatch(messaging.actions.selectMaxPages(getPageSize()));
            messaging.dispatch(messaging.actions.selectPreviousView("Search"));
        };

        //comp change requested
        function changeComp(component) {
            if (component === 'browse') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Browse"));
            } else if (component === 'wordcloud') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("WordCloud"));
            } else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu(storedPreviousView));
            }
        };


        //restore stuff to this view
        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = messaging.getState().selectedPreviousView;
            //restore fields
            let storedSearchTerms = messaging.getState().selectedSearchTerms;
            let storedSearchOptions = messaging.getState().selectedSearchOptions;
            let storedMaxPages = messaging.getState().selectedMaxPages;
            let storedCurrentPage = messaging.getState().selectedCurrentPage;
            //console.log("storedSearchOptions: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", storedSearchOptions);

            if (storedPreviousView == "Search" && (storedCurrentPage)) { currentPage(storedCurrentPage) };
            if (storedSearchTerms) { searchTerms(storedSearchTerms) };
            if (storedMaxPages) {
           
                getPageSize(storedMaxPages);
            };
            if (storedSearchOptions) {
                if (storedSearchOptions == "tfidf") { storedSearchOptions = "TFIDF" }
                else if (storedSearchOptions == "best") { storedSearchOptions = "Best Match" }

                console.log("retrieved search options xxxxxxxxxxxxxxx :", storedSearchOptions);
                searchTypeValue(storedSearchOptions);
            };
        };

        //run when changing to this view
        let storedPreviousView;
        restoreStuff();
        saveStuff();
      //  messaging.actions.selectMenu("searchbuttcomp");

        return {
            getPg,
            searchTerms,
            searchResult,
            changeComp,
            showTable,
            totalResults,
            selectSearchResultItem,
            currentPage,
            numberOfPages,
            //next,
            //prev,
            pageSizeSelection,
            getPageSize,
            selectedPageSize,
            searchTypeValSelector,
            searchTypeValue,
            selectedSearchType,
            clearInputField

        }
    }

});