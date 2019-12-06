define([], function () {

    var getBrowseItems = async function (p, ps, callback) {
        let toekn = window.localStorage.getItem('userToken');
        console.log(' token '+toekn);
                var response = await fetch(
            buildUrl("api/questions", {
                page: p,
                pageSize: ps
            }),
            {
                method: "GET",
                headers: {
                    Authorization: "Bearer "+toekn
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

    return {
        getBrowseItems
    }
});