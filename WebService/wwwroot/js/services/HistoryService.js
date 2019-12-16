define([""], function () {
    data = {
        "historyItems": []
    };

    let buildUrl = function (page, maxPages) {
        return `api/history?Page=${page}&PageSize=${maxPages}`;
    };

    let getHistory = async function (token, url, callback) {
        try {
            const response = await fetch(url, {
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

    let deleteHistory = async function (token, callback) {
        try {
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
        buildUrl,
        getHistory,
        deleteHistory
    }
});