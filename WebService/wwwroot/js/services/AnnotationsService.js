define([""], function () {


    //GET http://localhost:5001/api/annotations
    //GetAllAnnotationsOfUser([FromQuery] PagingAttributes pagingAttributes)
    let getAllAnnos = async function (p, ps, callback) {
        let toekn = window.localStorage.getItem('userToken');
        let response = await fetch(
            buildUrl("api/annotations/user", {
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
        let data = await response.json();
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


    return {
        getAllAnnos
    }
});