define([""], function () {
    
    let getSearchItems = async function (objectValues, callback) {
        const path = "api/search/";
        const searchUrl =  path + objectValues.searchString + objectValues.searchType + objectValues.pageNumber + objectValues.pageSize;
        try {
            const response = await fetch(searchUrl, {
                method: 'GET', // or 'PUT'

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