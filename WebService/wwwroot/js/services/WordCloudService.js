define([], function () {

    let getWCItems = async function (searchTerm, searchType, max, callback) {
        let response = null;
        let token = globalThis.localStorage.getItem('userToken');
        if (max == 0) {
            response = await fetch(
                buildUrl("api/search/wordrank", {
                    s: searchTerm,
                    stype: searchType
                }),
                {
                    method: "GET",
                    headers: {
                        Authorization: "Bearer " + token
                    }
                }
            );
        }
        else {
            response = await fetch(
                buildUrl("api/search/wordrank", {
                    s: searchTerm,
                    stype: searchType,
                    maxresults: max
                }),
                {
                    method: "GET",
                    headers: {
                        Authorization: "Bearer " + token
                    }
                }
            );
        }


        let data = await response;
        if (response.status != 401) //we are not unauthorized
        {
            try {
                data = await response.json();    //try to parse
            }
            catch (error) {         //json was incomplete
                let errorResponse = new Object();
                errorResponse.status = 666; //custom status code
                data = errorResponse;
            }
        } else if (response.status == 401) { //we are unauthorized!
            let errorResponse = new Object();
            errorResponse.status = response.status;  //send back status 401
            data = errorResponse;
        }
        callback(data);     //ok? then send it back
    };


    function buildUrl(url, parameters) {
        let queryString = "";
        for (const key in parameters) {
            if (parameters.hasOwnProperty(key)) {
                const value = parameters[key];
                queryString +=
                    encodeURIComponent(key) + "=" + encodeURIComponent(value) + "&";
            }
        }
        if (queryString.length > 0) {
            queryString = queryString.substring(0, queryString.length - 1); //chop off last "&"
            url = url + "?" + queryString;
        }
        return url;
    }

    return {
        getWCItems
    }
});