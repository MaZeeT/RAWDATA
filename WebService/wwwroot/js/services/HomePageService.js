define(["jquery"], function () {



    var getSearchItems = async function (searchString, searchType, pageSize, pageNumber, callback) {
        const baseUrl = "http://localhost:5001/";
        const path = "api/search/"

        const searchUrl = baseUrl + path + searchString + searchType + pageNumber + pageSize;
        console.log("URL: ", searchUrl);
        try {
            const response = await fetch(searchUrl, {
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
        getSearchItems
    }


});