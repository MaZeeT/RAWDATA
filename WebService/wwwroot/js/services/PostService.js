define(["jquery"], function () {

    var getAllChildDataOfPostUrl = async function (url, callback) {
       
        try {
            const response = await fetch(url, {
                method: 'GET', 
                headers: new Headers({
                    'Authorization': 'Bearer ' + window.localStorage.getItem("userToken"),
                    'Content-Type': 'application/json'
                }),
            }).then(function (response) {
                return response.json();
            }).then(function (responseBody) {
                return responseBody;
            });
            callback(response);

        } catch (error) {
            callback(null);
        }
    };

    var savePostAsBookmark = async function (url, callback) {
        console.log("Save as bookmark url: ", url);
        try {
            const response = await fetch(url, {
                method: 'POST', // or 'PUT'
                // body: JSON.stringify(incomingUserCredentials), // data can be `string` or {object}!

                headers: new Headers({
                    'Authorization': 'Bearer ' + window.localStorage.getItem("userToken"),
                    'Content-Type': 'application/json'
                }),
            }).then(function (response) {
                return response.json();
            }).then(function (responseBody) {
                return responseBody;
            });
            callback(response);

        } catch (error) {
            console.error('Error:', error);
        }

    }


    return {
        getAllChildDataOfPostUrl,
        savePostAsBookmark
    }


});