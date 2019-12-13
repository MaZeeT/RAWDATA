define(["knockout", "bookmarksService", 'messaging', 'util'], function (ko, bs, mess, util) {

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
            bs.getBookmarks(token, url, function (response) {
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
            currentPage(1);
            let url = bs.buildUrl(currentPage(), pgSize());
            getData(url);
        };

        let navPage = function (url) {
            if (url != null) {
                getData(url);
            }
        };

        let deleteBookmark = function (postId) {
            //delete on backend
            bs.deleteBookmark(token, postId, function (response) {
                return response;
            });

            //delete on fronted
            items.remove(function (currentItem) {
                return currentItem.postId === postId
            });
        };

        let deletions = function () {
            bs.deleteBookmarks(token, function (response) {
              //  return response;
                currentPage(1);
                let url = bs.buildUrl(currentPage(), pgSize());
                getData(url);
            })

        };

        let selectPostItem = function (item) {
            console.log("Item is: ", item);
            mess.dispatch(mess.actions.selectPost(item));
            mess.dispatch(mess.actions.selectMenu("postdetails"));
        };

        //store stuff from this view
        let saveStuff = function () {
            mess.dispatch(mess.actions.selectCurrentPage(currentPage()));
            mess.dispatch(mess.actions.selectMaxPages(pgSize()));
            mess.dispatch(mess.actions.selectPreviousView("Bookmarks"));
        };

        //comp change requested
        function changeComp(component) {
            if (component === 'anno') {
                saveStuff();
                mess.dispatch(mess.actions.selectMenu("Annotations"));
            } else if (component === 'history') {
                saveStuff()
                mess.dispatch(mess.actions.selectMenu("History"));
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
            if (storedPreviousView == "Bookmarks" && (storedCurrentPage)) { currentPage(storedCurrentPage); }
            if (storedMaxPages) {
                pgSize(storedMaxPages);
            }
        };

        //run initially
      //  mess.actions.selectMenu("hisbuttcomp");
        let storedPreviousView;
        restoreStuff();
        saveStuff();
        //let page = 1;
        let url = bs.buildUrl(currentPage(), pgSize());
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
            deleteBookmark,
            deletions,
            changeComp,
            selectPostItem
        };
    }

});
