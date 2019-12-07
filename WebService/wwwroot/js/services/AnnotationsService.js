define(["jquery"], function () {

    //POST http://localhost:5001/api/annotations
    //add annotation
    //public ActionResult AddAnnotation(AnnotationsDto annotationObj)

    //PUT http://localhost:5001/api/annotations/52
    //update specific anno

    ///GET [HttpGet("post/{postId}")] 
    //get annotations on post

    //GET http://localhost:5001/api/annotations
    //GetAllAnnotationsOfUser([FromQuery] PagingAttributes pagingAttributes)
    var getAllAnnos = async function (p, ps, callback) {
        let toekn = window.localStorage.getItem('userToken');
        console.log(' token ' + toekn);
        var response = await fetch(
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


    //GET http://localhost:5001/api/annotations/2
    //get specific anno

    //DELETE http://localhost:5001/api/annotations/52
    //del specific anno



    return {
        getAllAnnos
    }
});