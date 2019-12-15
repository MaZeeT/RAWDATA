define([""], function () {

    let getAllChildDataOfPostUrl = async function (url, callback) {
        if (url) {
            try {
                const response = await fetch(url, {
                    method: 'GET',
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
                callback(null);
            }
        }
    };

    let savePostAsBookmark = async function (url, callback) {
        try {
            const response = await fetch(url, {
                method: 'POST', // or 'PUT'
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

    let saveAnnotationOnPost = async function (incomingData, callback) {
        const requestBody = { PostId: incomingData.postid, Body: incomingData.annotBody };
        try {
            const response = await fetch('http://localhost:5001/api/annotations', {
                method: 'POST', // or 'PUT'
                body: JSON.stringify(requestBody), // data can be `string` or {object}!

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

    let updateAnnotation = async function (annotationId, newAnnotationBody, callback) {
        const requestBody = { Body: newAnnotationBody };
        const url = "http://localhost:5001/api/annotations/" + annotationId;
        try {
            const response = await fetch(url, {
                method: 'PUT', // or 'PUT'
                body: JSON.stringify(requestBody), // data can be `string` or {object}!

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
    };

    let deleteAnnotation = async function (annotationId, callback) {
        const url = "http://localhost:5001/api/annotations/" + annotationId;
        try {
            const response = await fetch(url, {
                method: 'DELETE', // or 'PUT'
                
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
    };
    
    return {
        getAllChildDataOfPostUrl,
        savePostAsBookmark,
        saveAnnotationOnPost,
        updateAnnotation,
        deleteAnnotation
    }

});