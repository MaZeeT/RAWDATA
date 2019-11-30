define(["knockout", "dataservice"], function (ko, ds) {

    return function () {

        var historyItems = [
            { title: "tester1", url: "www.something.com", date: "today" },
            { title: "tester32", url: "www.somethingelse.com", date: "22/5" },
            { title: "tester45", url: "www.goggles.com", date: "24/12" }
        ];
          

        return {
            historyItems
        }
    }

});
