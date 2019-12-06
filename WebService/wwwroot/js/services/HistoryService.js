define(["jquery"], function() {

   // var userToken = window.localStorage.getItem(userToken);

    var data = {
        "historyItems":[
            {"title": "tester1", "url": "www.something.com", "date": "today"},
            {"title": "tester32", "url": "www.somethingelse.com", "date": "22/5666"},
            {"title": "tester45", "url": "www.goggles.com", "date": "24/12"}
        ]
    };


    var getHistory = async function(token, callback) {
        callback(data);
    };

    var deleteHistory = async function(token, callback) {
        data = {
            "historyItems":[]
        };
        callback(data);
    };



/*
    var getHistory = function(callback) {
        fetch("api/history")
            .then(function(response) {
                return response.json();
            })
            .then(function(data) {
                callback(data);
            });
    };
*/

    /*async*/
   /* var getHistory = async function(callback) {
        var response = await fetch("api/history");
        var data = await response.json();
        callback(data);
    };

    /*async*/
  /*  var deleteHistory = async function(callback) {
        var URL = "api/history/delete/all";

        const response = await fetch(URL, {
            method: 'get',
            headers: new Headers({
                'Authorization': userToken,
                'Content-Type': 'application/json'
            })
        });
        const posts = await response.json();




        var data = await response.json();
        callback(data);
    };*/





    return {
        getHistory,
        deleteHistory
    }
});