define(["knockout", "historyService", "util"], function (ko, ds, util) {

    return function () {
        let token = window.localStorage.getItem('userToken');

        let pgSizeOptions = ko.observableArray([5, 10, 15, 25, 50, 100]);
        let pgSize = ko.observable(10);
        let totalPages = ko.observable();
        let currentPage = ko.observable(1);
        let prevUrl = ko.observable();
        let nextUrl = ko.observable();
        let items = ko.observableArray();
        console.log("maxpage value is: " + pgSize());  //todo remove

        let getData = function (url) {
            ds.getHistory(token, url, function (response) {
                currentPage(util.getParameterByName('page', url));
                totalPages(response.numberOfPages);
                prevUrl(response.prev);
                nextUrl(response.next);
                items(response.items);
            });
        };

        //let page = 1;
        let url = ds.buildUrl(currentPage(), pgSize());
        getData(url);

        let pageSize = function (size) {
            pgSize(size);
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
                return response;
            })
        };



        return {
            totalPages,
            currentPage,
            pageSize,
            pgSize,
            pgSizeOptions,
            items,
            navPage,
            nextUrl,
            prevUrl,
            deletions
        };

    };
});
