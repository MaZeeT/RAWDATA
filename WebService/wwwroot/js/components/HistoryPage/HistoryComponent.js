define(["knockout", "historyService", 'messaging', 'util'], function (ko, ds, mess, util) {

    return function () {
        let token = window.localStorage.getItem('userToken');

        let pgSizeOptions = ko.observableArray([5, 10, 20, 30, 40, 50]);
        let pgSize = ko.observable(10);
        let totalPages = ko.observable();
        let totalResults = ko.observable();
        let currentPage = ko.observable(1);

        let prevUrl = ko.observable();
        let nextUrl = ko.observable();
        let items = ko.observableArray();

        let getData = function (url) {
            ds.getHistory(token, url, function (response) {
                if (util.getParameterByName('page', url)) {
                    currentPage(util.getParameterByName('page', url));
                };
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
            currentPage(1)
            let url = ds.buildUrl(currentPage(), pgSize());
            getData(url);
        };

        let navPage = function (url) {
            if (url != null) {
                getData(url);
            }
        };

        let deletions = function () {
            ds.deleteHistory(token, function (response) {
                //return response;
                currentPage(1)
                let url = ds.buildUrl(currentPage(), pgSize());
                getData(url);
            })

        };

        let selectPostItem = function (item) {
            mess.dispatch(mess.actions.selectPost(item));
            mess.dispatch(mess.actions.selectMenu("postdetails"));
        };

        //store stuff from this view
        let saveStuff = function () {
            mess.dispatch(mess.actions.selectCurrentPage(currentPage()));
            mess.dispatch(mess.actions.selectMaxPages(pgSize()));
            mess.dispatch(mess.actions.selectPreviousView("History"));
        };

        //comp change requested
        function changeComp(component) {
            if (component === 'anno') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("Annotations"));
            } else if (component === 'book') {
                saveStuff()
                mess.dispatch(mess.actions.selectMenu("Bookmarks"));
            } else if (component === 'searchhistory') {
                saveStuff()
                mess.dispatch(mess.actions.selectMenu("Search History"));
            } else if (component === 'previous' && storedPreviousView) {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu(storedPreviousView));
            }
        };

        //restore stuff to this view
        let restoreStuff = function () {
            //get previous component/view
            storedPreviousView = mess.getState().selectedPreviousView;
            //restore fields
            let storedMaxPages = mess.getState().selectedMaxPages;
            let storedCurrentPage = mess.getState().selectedCurrentPage;
            if (storedPreviousView == "History" && (storedCurrentPage)) { currentPage(storedCurrentPage); }
            if (storedMaxPages) {
                pgSize(storedMaxPages);
            }
        };

        //run initially
        let storedPreviousView;
        restoreStuff();
        saveStuff();
        let url = ds.buildUrl(currentPage(), pgSize());
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
