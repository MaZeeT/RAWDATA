define(['knockout', 'postservice', 'messaging'], function (ko, postservice ,messaging) {
    return function () {
        var postUrl = ko.observable(messaging.getState().selectedPost);
 
        //messaging.subscribe(function () {
        //    var state = messaging.getState();
        //    postUrl(state.selectedPerson);
        //});

        let postDetails = ko.observable([]);

      
        console.log("You only see ....", postUrl());
        postservice.getAllChildDataOfPostUrl(postUrl(), function (responseData) {
            console.log("Fucking url ", postUrl);
            if (responseData) {
                console.log("My response data is: ", responseData);
                postDetails(responseData.items);
            }

        });
       




        return {
            postUrl,
            postDetails
        };
    };
});