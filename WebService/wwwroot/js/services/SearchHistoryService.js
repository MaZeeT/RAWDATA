define(["jquery"], function () {



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
        var data = await response.json();
        callback(data);
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




    //DELETE http://localhost:5001/api/annotations/52
    //del specific anno



    return {
        getSearchHist
    }
});