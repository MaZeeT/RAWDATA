define([""], function () {

    //GET http://localhost:5001/api/hisory/searches
    //GetAllAnnotationsOfUser([FromQuery] PagingAttributes pagingAttributes)
    let getSearchHist = async function (page, pageSize, callback) {
        let token = globalThis.localStorage.getItem('userToken');
        let response = await fetch(
            buildUrl("api/history/searches", {
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
        let data = await response;
        if (response.status != 401) //we are not unauthorized
        {
            try {
                data = await response.json();    //try to parse
            } catch (error) {         //json was incomplete
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
        let qs = "";
        for (const key in parameters) {
            if (parameters.hasOwnProperty(key)) {
                const value = parameters[key];
                qs +=
                    encodeURIComponent(key) + "=" + encodeURIComponent(value) + "&";
            }
        }
        if (qs.length > 0) {
            qs = qs.substring(0, qs.length - 1); //chop off last "&"
            url = url + "?" + qs;
        }

        return url;
    }

    //DELETE http://localhost:5001/api/history/searches/delete/all
    //del specific anno
    let deleteSearchHistory = async function (callback) {
        const url = "api/history/searches/delete/all";
        try {
            const response = await fetch(url, {
                method: 'DELETE',

                headers: new Headers({
                    'Authorization': 'Bearer ' + globalThis.localStorage.getItem("userToken"),
                    'Content-Type': 'application/json'
                }),
            }).then(function (response) {
                return response;
            });
            callback(response);

        } catch (error) {
            console.log('Error:', error);
        }
    };

    return {
        getSearchHist,
        deleteSearchHistory
    }
});