define(['knockout', 'postservice', 'messaging'], function (ko, postservice ,messaging) {
    return function () {
        var postUrl = ko.observable(messaging.getState().selectedPost);
        var annotationBodyText = ko.observable("");
        let annotatedPostValues = ko.observable();

        var postDetails = ko.observable([]);
        var showspinner = ko.observable(true);
        var showAnnotTextArea = ko.observable(false);
        var responseData = ko.observable(false);
       
        postservice.getAllChildDataOfPostUrl(postUrl(), function (responseData) {
            if (responseData) {
                postDetails(responseData);
                showspinner(false);
            } 

        });

        let addAnnotation = function (value, event) {
            console.log("value ", value);
            console.log("you clicked " + event.target.id);
            console.log(showAnnotTextArea());
            showAnnotTextArea(true);
        };

        
        let addBookmark = function (value) {
            console.log(value.id, value.createBookmarkLink);
            const createBookmarkUrl = value.createBookmarkLink;
            postservice.savePostAsBookmark(createBookmarkUrl, function (responseFromServer) {
                responseData(responseFromServer);
            });
        };

        annotationBodyText.subscribe(function (annotBody) {
            console.log("Value from test: ", annotBody);
            console.log("myStinkingValue: ", annotatedPostValues());
            
            if (annotBody.length === 0 ) {
                return;
            } 
            const createAnnotObject = {
                postid: annotatedPostValues().id,
                annotBody
            };
            postservice.saveAnnotationOnPost(createAnnotObject, function (responseData) {
                if (responseData) {
                    console.log(responseData);
                    //TODO: Append the annotation text that is currently added as well as append the existing annotations on the thread if there are any ;) -> 
                    // how to do: 1. the text needs to append on the observable with the proper id... probably need an array or something of that kind... also if there are existing annotations and they are seen
                    // then these need to be updatable and then updating on view procedure is the same as for no 1. 
                    // if too complicated then change approach somehow... though it should work. 
                }

            });
        });

        return {
            postUrl,
            postDetails,
            addAnnotation,
            addBookmark,
            showspinner,
            showAnnotTextArea,
            responseData,
            annotationBodyText,
            annotatedPostValues
        };
    };
});