define(['knockout', 'homeService', 'messaging', 'util'], function (knockout, homeService, messaging, util) {

    return function () {

        //Pagination
        let pageSizeSelection = knockout.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let selectedPageSize = knockout.observable();
        let getPageSize = knockout.observable(10);

        let currentPage = knockout.observable(1);
        let numberOfPages = knockout.observable();

        let nexturi = '666'; //placeholder for grabbing querystring page= value
        let prevuri = '666'; //placeholder for grabbing querystring page= value

        //Other dropdowns
        let searchTypeValSelector = knockout.observableArray(["TFIDF", "Exact Match", "Simple Match", "Best Match"]); //selection of searchtypes
        let searchTypeValue = knockout.observable("Best Match");
        let selectedSearchType = knockout.observable();
        
        //Search
        const placeholderStr = "Input search terms here...";
        let searchTerms = knockout.observable(placeholderStr);
        let searchstring = knockout.observable("");

        let searchResult = knockout.observableArray([]);
        let showTable = knockout.observable(false);
        let totalResults = knockout.observable("0");

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
        };

        searchTerms.subscribe(function (searchStr) {
            if (searchStr.length === 0) {
                searchResult([]);
                return;
            }
            searchstring(searchStr);
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());
        });

        selectedPageSize.subscribe(function () {
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());
        });

        selectedSearchType.subscribe(function () {
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());

        });

        //grab/refresh data when page change
        function getPg(direction) {
            let npg = null;
            if (direction === 'next') {
                npg = util.getParameterByName('page', nexturi);
            } else if (direction === 'prev') {
                npg = util.getParameterByName('page', prevuri);
            }

            if (npg) {

                callService(searchstring(), searchTypeValue(), getPageSize(), npg);
            }
        }

        function callService(searchString, srcTypeVal, pageSize, currPage) {
            if (searchString) {

                let givenSearchType = util.searchTypeSelectorMapping(srcTypeVal);
                let object = util.conputeUrlStringWithPagination(searchString, givenSearchType, pageSize, currPage);

                homeService.getSearchItems(object, function (responseData) {
                    if (responseData) {

                        currentPage(currPage);
                        totalResults(responseData.totalResults);
                        searchResult(responseData.items);
                        numberOfPages(responseData.numberOfPages);
                        nexturi = responseData.next;
                        prevuri = responseData.prev;
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
        }

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
        }

        //restore stuff to this view
        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = messaging.getState().selectedPreviousView;
            //restore fields
            let storedSearchTerms = messaging.getState().selectedSearchTerms;
            let storedSearchOptions = messaging.getState().selectedSearchOptions;
            let storedMaxPages = messaging.getState().selectedMaxPages;
            let storedCurrentPage = messaging.getState().selectedCurrentPage;

            if (storedPreviousView == "Search" && (storedCurrentPage)) {
                currentPage(storedCurrentPage)
            }

            if (storedMaxPages) {
                getPageSize(storedMaxPages);
            }

            if (storedSearchOptions) {
                if (storedSearchOptions == "tfidf") {
                    storedSearchOptions = "TFIDF"
                } else if (storedSearchOptions == "best") {
                    storedSearchOptions = "Best Match"
                }
                searchTypeValue(storedSearchOptions);
            }

            if (storedSearchTerms) {
                searchTerms(storedSearchTerms)
            }
        };

        //run when changing to this view
        let storedPreviousView;
        restoreStuff();
        saveStuff();

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