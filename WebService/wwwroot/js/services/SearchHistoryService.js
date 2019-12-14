define([""], function () {



    //GET http://localhost:5001/api/hisory/searches
    //GetAllAnnotationsOfUser([FromQuery] PagingAttributes pagingAttributes)
    var getSearchHist = async function (p, ps, callback) {
        let toekn = window.localStorage.getItem('userToken');
        console.log(' token ' + toekn);
        var response = await fetch(
            buildUrl("api/history/searches", {
                page: p,
                pageSize: ps
            }),
            {
                method: "GET",
                headers: {
                    Authorization: "Bearer " + toekn
                }
            }
        );
        var data = await response;
        if (response.status != 401) //we are not unauthorized
        {
            try {
                data = await response.json();    //try to parse
            }
            catch (error) {         //json was incomplete
                var errorresponse = new Object();
                errorresponse.status = 666; //custom status code
                data = errorresponse;
            }
        } else if (response.status == 401) { //we are unauthorized!
            var errorresponse = new Object();
            errorresponse.status = response.status;  //send back status 401
            data = errorresponse;
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
        //console.log("Annotation to be deleted id is: ", annotationId);
        const url = "api/history/searches/delete/all"
        try {
            const response = await fetch(url, {
                method: 'DELETE', 

                headers: new Headers({
                    'Authorization': 'Bearer ' + window.localStorage.getItem("userToken"),
                    'Content-Type': 'application/json'
                }),
            }).then(function (response) {
                return response;
            });
            callback(response);

        } catch (error) {
            console.log('Error:', error);
        }
    }


    return {
        getSearchHist,
        deleteSearchHistory
    }
});