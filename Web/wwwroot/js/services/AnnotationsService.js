define([""], function () {


    //GET http://localhost:5001/api/annotations
    //GetAllAnnotationsOfUser([FromQuery] PagingAttributes pagingAttributes)
    let getAllAnnos = async function (page, pageSize, callback) {
        let token = globalThis.localStorage.getItem('userToken');
        let response = await fetch(
            buildUrl("api/annotations/user", {
                page: page,
                pageSize: pageSize
            }),
            {
                method: "GET",
                headers: {
                    Authorization: "Bearer " + token
                }
            }
        );
        let data = await response.json();
        callback(data);
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
        getAllAnnos
    }
});