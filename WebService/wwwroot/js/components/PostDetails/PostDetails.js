define(['knockout', 'postservice', 'messaging'], function (ko, postservice ,messaging) {
    return function () {
        var postUrl = ko.observable(messaging.getState().selectedPost);

        var postDetails = ko.observable([]);
        var showspinner = ko.observable(true);
        var showAnnotTextArea = ko.observable(false);
       
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

        };


        return {
            postUrl,
            postDetails,
            addAnnotation,
            addBookmark,
            showAnnotTextArea,
            showspinner
        };
    };
});