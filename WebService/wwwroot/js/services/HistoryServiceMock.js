define([""], function() {

    // mock data
    var data = {
        "historyItems":[
            {"title": "tester1", "threadUrl": "www.something.com.mock", "date": "today"},
            {"title": "tester32", "threadUrl": "www.somethingelse.com.mock", "date": "22/56667"},
            {"title": "tester45", "threadUrl": "www.goggles.com.mock", "date": "24/12"}
        ]
    };

    var getHistory = async function(token, page, maxPages, callback) {
        console.log("token: " + token);
        console.log("page: " + page + ", max: " + maxPages);
            callback(data.historyItems);
    };

    var deleteHistory = async function(token, callback) {
        data = {
            "historyItems":[]
        };
        console.log("deleting called");
        console.log("token: " + token);
        callback(data);
    };

    return {
        getHistory,
        deleteHistory
    }
});