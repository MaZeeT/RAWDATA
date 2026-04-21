define(["knockout", "historyService", 'messaging', 'util'], function (knockout, historyService, messaging, util) {

    return function () {
        let token = globalThis.localStorage.getItem('userToken');

        let pgSizeOptions = knockout.observableArray([5, 10, 20, 30, 40, 50]);
        let pgSize = knockout.observable(10);
        let totalPages = knockout.observable();
        let totalResults = knockout.observable();
        let currentPage = knockout.observable(1);

        let prevUrl = knockout.observable();
        let nextUrl = knockout.observable();
        let items = knockout.observableArray();

        let getData = function (url) {
            historyService.getHistory(token, url, function (response) {
                if (util.getParameterByName('page', url)) {
                    currentPage(util.getParameterByName('page', url));
                }
                totalPages(response.numberOfPages);
                totalResults(response.totalResults);
                prevUrl(response.prev);
                nextUrl(response.next);
                items(response.items);
                saveStuff();
            });
        };

        let pageSize = function (size) {
            pgSize(size);
            currentPage(1);
            let url = historyService.buildUrl(currentPage(), pgSize());
            getData(url);
        };

        let navPage = function (url) {
            if (url != null) {
                getData(url);
            }
        };

        let deletions = function () {
            historyService.deleteHistory(token, function (response) {
                //return response;
                currentPage(1);
                let url = historyService.buildUrl(currentPage(), pgSize());
                getData(url);
            })

        };

        let selectPostItem = function (item) {
            messaging.dispatch(messaging.actions.selectPost(item));
            messaging.dispatch(messaging.actions.selectMenu("postdetails"));
        };

        //store stuff from this view
        let saveStuff = function () {
            messaging.dispatch(messaging.actions.selectCurrentPage(currentPage()));
            messaging.dispatch(messaging.actions.selectMaxPages(pgSize()));
            messaging.dispatch(messaging.actions.selectPreviousView("History"));
        };

        //comp change requested
        function changeComp(component) {
            if (component === 'anno') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Annotations"));
            } else if (component === 'book') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Bookmarks"));
            } else if (component === 'searchhistory') {
                saveStuff();
                messaging.dispatch(messaging.actions.selectMenu("Search History"));
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
            let storedMaxPages = messaging.getState().selectedMaxPages;
            let storedCurrentPage = messaging.getState().selectedCurrentPage;
            if (storedPreviousView == "History" && (storedCurrentPage)) { currentPage(storedCurrentPage); }
            if (storedMaxPages) {
                pgSize(storedMaxPages);
            }
        };

        //run initially
        let storedPreviousView;
        restoreStuff();
        saveStuff();
        let url = historyService.buildUrl(currentPage(), pgSize());
        getData(url);


        return {
            totalPages,
            totalResults,
            currentPage,
            pageSize,
            pgSize,
            pgSizeOptions,
            items,
            navPage,
            nextUrl,
            prevUrl,
            deletions,
            changeComp,
            selectPostItem
        };

    };
});
