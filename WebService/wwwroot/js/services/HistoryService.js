define(["jquery"], function () {
    data = {
        "historyItems": []
    };
    
    var getHistory = async function (token, page, maxPages, callback) {
        try {
            const response = await fetch("api/history", {
                method: 'GET', // or 'PUT'
                headers: new Headers({
                    'Authorization': 'Bearer ' + token,
                    'Content-Type': 'application/json'
                })
            }).then(function (response) {
                return response.json();
            }).then(function (responseBody) {
                return responseBody;
            });
            callback(response);

        } catch (error) {
            console.error('Error:', error);
        }

    };
    
    var deleteHistory = async function (token, callback) {
        try {
            console.log("token: " + token);
            const response = await fetch("api/history/delete/all", {
                method: 'DELETE', // or 'PUT'
                headers: new Headers({
                    'Authorization': 'Bearer ' + token,
                    'Content-Type': 'application/json'
                })
            }).then(function (response) {
                return response;
            }).then(function (responseBody) {
                return responseBody;
            });
            callback(response);

        } catch (error) {
            console.error('Error:', error);
        }
    };

    return {
        getHistory,
        deleteHistory
    }
});