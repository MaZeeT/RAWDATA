define(['knockout', 'homeService', 'messaging', 'util'], function (ko, homeserv, messaging, util) {

    return function () {

        //Pagination
        let pageSizeSelection = ko.observableArray(['5', '10', '20', '30', '40', '50']); //selection of pagesizes
        let selectedPageSize = ko.observable('10');
        let getPageSize = ko.observable(selectedPageSize());

        let currentPage = ko.observable(1);

        //Other dropdowns
        let searchTypeValSelector = ko.observableArray(["TFIDF", "Exact Match", "Simple Match", "Best Match"]); //selection of searchtypes
        let searchTypeValue = ko.observable(0);
        let selectedSearchType = ko.observable("TFIDF");

        
        //Search
        const placeholderStr = "Search with terms here..."
        let searchTerms = ko.observable(placeholderStr);
        let searchstring = ko.observable("");

        let searchResult = ko.observableArray([]);
        let showTable = ko.observable(false);
        let totalResults = ko.observable("0");

        ///////////////////////////////////////////////////////////////////////////////////////////

        //Pasing the linkthread url from homepage component & navigating to postdetails page;
        let selectSearchResultItem = function (item) {
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

        selectedPageSize.subscribe(function (value) {
            getPageSize(value[0]);
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());
        });

        selectedSearchType.subscribe(function (value) {
            searchTypeValue(value[0]);
            callService(searchstring(), searchTypeValue(), getPageSize(), currentPage());

        });

        //store stuff from this view
        function saveStuff() {
            messaging.dispatch(messaging.actions.selectSearchTerms(searchTerms()));
            messaging.dispatch(messaging.actions.selectSearchOptions(selectedSearchType()));
            messaging.dispatch(messaging.actions.selectCurrentPage(currentPage()));
            messaging.dispatch(messaging.actions.selectMaxPages(getPageSize()));
        };

        //comp change requested
        function changeComp(component) {
            if (component === 'browse') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Browse"));
            } else if (component === 'wordcloud') {
                saveStuff()
                messaging.dispatch(messaging.actions.selectMenu("wordcloud"));
            }
        };

        let next = function () {
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
        }


        function callService(searchString, srcTypeVal, pageSize, currPage) {
            if (searchString) {

                saveStuff();
                let givenSearchType = util.searchTypeSelectorMapping(srcTypeVal);
                let object = util.conputeUrlStringWithPagination(searchString, givenSearchType, pageSize, currPage);
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
        }

        //execute on coming to this view
        console.log("contesnt of searchterms : ", messaging.getState().selectedSearchTerms);
        console.log("contesnt of searchopts : ", messaging.getState().selectedSearchOptions);

        //get previous component/view
        let storedPreviousView = messaging.getState().selectedPreviousView;

        //store component name
        messaging.dispatch(messaging.actions.selectPreviousView("Home"));

        //restore fields
        let storedSearchTerms = messaging.getState().selectedSearchTerms;
        let storedSearchOptions = messaging.getState().selectedSearchOptions;
        let storedMaxPages = messaging.getState().selectedMaxPages;
        let storedCurrentPage = messaging.getState().selectedCurrentPage;

        if (storedPreviousView == "Home" && (storedCurrentPage)) { currentPage(storedCurrentPage) }
        if (storedMaxPages) { } //again dunno how to set maxpages
        if (storedSearchTerms) { searchTerms(storedSearchTerms) }
     /*   if (storedSearchOptions == "best" || storedSearchOptions == "Best Match" ) {
   
        } else {
           
        }*/ //confused about how to set that option

        return {
            searchTerms,
            searchResult,
            changeComp,
            showTable,
            totalResults,
            selectSearchResultItem,
            currentPage,
            next,
            prev,
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