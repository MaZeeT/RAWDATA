define(['knockout', 'postservice', 'messaging'], function (ko, postservice ,messaging) {
    return function () {
        var postUrl = ko.observable(messaging.getState().selectedPost);

        var postDetails = ko.observable([]);
        var showspinner = ko.observable(true);
        var responseData = ko.observable(false);
        var showAnnotTextArea = ko.observable(false);
        let enableDetails = ko.observable(true);
       
        console.log("You only see ....", postUrl());
        postservice.getAllChildDataOfPostUrl(postUrl(), function (responseData) {
            console.log("Fucking url ", postUrl());
            if (responseData) {
                console.log("My response data is: ", responseData);
                postDetails(responseData);
                showspinner(false);
            } 

        });
        var addAnnotation = function (value) {
            console.log(value.id, value.createAnnotationLink);
            console.log(showAnnotInput());
        };
        
        var addBookmark = function (value) {
            console.log(value.id, value.createBookmarkLink);
            const createBookmarkUrl = value.createBookmarkLink;
            postservice.savePostAsBookmark(createBookmarkUrl, function (responseFromServer) {
                console.log("ResponseDataFromBookmarkis: ", responseData);
                console.log("ResponseDataFromBookmarkis: ", responseFromServer);
                responseData(responseFromServer);
                console.log("responseFromServer is now: ", responseData());
               

            });

        };


        return {
            postUrl,
            postDetails,
            addAnnotation,
            addBookmark,
            showAnnotTextArea,
            showspinner,
            enableDetails,
            responseData
        };
    };
});