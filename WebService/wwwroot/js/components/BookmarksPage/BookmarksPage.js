define(["knockout", "bookmarksService"], function (ko, bookmarskServ) { //todo typo

    return function () {

        var bookmarkItems = [
            {title: "tester1", url: "www.something.com", date: "today"},
            {title: "tester32", url: "www.somethingelse.com", date: "22/5"},
            {title: "tester45", url: "www.goggles.com", date: "24/12"}
        ];


        return {
            bookmarkItems
        }
    }

});
