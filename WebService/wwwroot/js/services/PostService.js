define(["jquery"], function () {

    var getAllChildDataOfPostUrl = async function (url, callback) {
        console.log("Fucking url is: ", url);
        if (url) {
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
                console.error('Error:', error);
                callback(null);
            }
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

    };

    let saveAnnotationOnPost = async function (incomingData, callback) {
        console.log("Save annotation postid: ", incomingData.postid);
        console.log("Save annotation body: ", incomingData);
        const requestBody = { PostId: incomingData.postid, Body: incomingData.annotBody };
        try {
            const response = await fetch('http://localhost:5001/api/annotations', {
                method: 'POST', // or 'PUT'
                body: JSON.stringify(requestBody), // data can be `string` or {object}!

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
        savePostAsBookmark,
        saveAnnotationOnPost
    }


});