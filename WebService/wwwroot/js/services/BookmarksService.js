define(["jquery"], function () {

    let buildUrl = function (page, maxPages) {
        return `api/bookmark?Page=${page}&PageSize=${maxPages}`;
    };

    let getBookmarks = async function (token, url, callback) {
        console.log(url);
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

    let deleteBookmark = async function (token, postId, callback) {
        let url = `api/bookmark/delete/${postId}`;
        try {
            console.log("token: " + token);
            const response = await fetch(url, {
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

    let deleteBookmarks = async function (token, callback) {
        try {
            console.log("token: " + token);
            const response = await fetch("api/bookmark/delete/all", {
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
        getBookmarks,
        deleteBookmark,
        deleteBookmarks
    }
});