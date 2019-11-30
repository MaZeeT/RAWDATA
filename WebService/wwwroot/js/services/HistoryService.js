

define(["jquery"], function() {

    var getNamesWithFetch = function(callback) {
        fetch("api/history")
            .then(function(response) {
                return response.json();
            })
            .then(function(data) {
                callback(data);
            });
    };


    return {
        getNamesWithFetch
    }
});