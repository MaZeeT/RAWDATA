define(["knockout", "bookmarksService", 'messaging'], function (ko, bs, mess) {

    return function () {
        let token = window.localStorage.getItem('userToken');

        let pgSizeOptions = ko.observableArray([5, 10, 15, 25, 50, 100]);
        let pgSize = ko.observable(10);
        let totalPages = ko.observable();
        let prevUrl = ko.observable();
        let nextUrl = ko.observable();
        let items = ko.observableArray();
        console.log("maxpage value is: " + pgSize());  //todo remove

        let getData = function (url) {
            bs.getBookmarks(token, url, function (response) {
                totalPages(response.numberOfPages);
                prevUrl(response.prev);
                nextUrl(response.next);
                items(response.items);
            });
        };

        let page = 1;
        let url = bs.buildUrl(page, pgSize());
        getData(url);

        let pageSize = function (size) {
            pgSize(size);
            let url = bs.buildUrl(page, pgSize());
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
                return response;
            })
        };

        let selectPostItem = function (item) {
            console.log("Item is: ", item);
            mess.dispatch(mess.actions.selectPost(item));
            mess.dispatch(mess.actions.selectMenu("postdetails"));
        };

        return {
            pageSize,
            pgSize,
            pgSizeOptions,
            items,
            navPage,
            nextUrl,
            prevUrl,
            deleteBookmark,
            deletions,
            selectPostItem
        };
    }

});
