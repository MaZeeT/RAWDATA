define(["jquery"], function () {



        const baseUrl = "http://localhost:5001/";
        const path ="api/search/"

        var urlApiCall = function (baseUrl, path, searchStr) {
            //const searchString = "?s=" + searchStr;
            //const searchType = "&stype=" + sType;
            //const pageSize = "&pageSize=" + pSize;
            //const pageNumber = "&page=" + pNum;

            const searchString = "?s=" + searchStr;
            const searchType = "&stype=0";
            const pageSize = "&pageSize=5";
            const pageNumber = "&page=10";
            const url = baseUrl + path + searchString + searchType + pageNumber + pageSize;
            return url;
        }

        var getSearchItems = async function (searchStr, callback) {
            const searchString = "?s=" + searchStr;
            const searchType = "&stype=0";
            const pageSize = "&pageSize=5";
            const pageNumber = "&page=10";
            const searchUrl = baseUrl + path + searchString + searchType + pageNumber + pageSize;
            
            console.log("Search Url: ", searchUrl);
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