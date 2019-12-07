define(["jquery"], function () {

    var getAllChildDataOfPostUrl = async function (url, callback) {
       
        console.log("QuestionUrl", url);
        try {
            const response = await fetch(url, {
                method: 'GET', // or 'PUT'
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


    return {
        getAllChildDataOfPostUrl
    }


});