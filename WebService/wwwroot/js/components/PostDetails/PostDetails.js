define(['knockout', 'postservice', 'messaging'], function (ko, postservice ,messaging) {
    return function () {
        let postUrl = ko.observable(messaging.getState().selectedPost);
        let annotationBodyText = ko.observable("");
        let annotatedPostValues = ko.observable();
        let updateAnnotationValue = ko.observable("");

        let postDetails = ko.observable([]);
        let postAnnotationsArray = ko.observable([]);
        let showspinner = ko.observable(true);
        let showAnnotTextArea = ko.observable(false);
        let responseData = ko.observable(false);
        let deletedAnnotStatus = ko.observable(false);
        let newAnnotation = ko.observable({});

        

        postservice.getAllChildDataOfPostUrl(postUrl(), function (responseFromServer) {
            if (responseFromServer) {
                postDetails(responseFromServer);
                postAnnotationsArray(responseFromServer)
                showspinner(false);
            }
        });

        let addAnnotation = function (value, event) {
            showAnnotTextArea(true);
        };

        
        let addBookmark = function (value) {
            const createBookmarkUrl = value.createBookmarkLink;
            postservice.savePostAsBookmark(createBookmarkUrl, function (responseFromServer) {
                responseData(responseFromServer);
            });
        };


        let updateAnnotation = function (value) {
            if (updateAnnotationValue() && value.annotationId) {
                let annotationId = value.annotationId;
                let annotationBody = updateAnnotationValue();
                postservice.updateAnnotation(annotationId, annotationBody, function (serverResponse) {
                    let status = serverResponse.status;
                    if (status === 204) {
                        callServiceGetThread(postUrl());
                        updateAnnotationValue("");
                    }
                });
            }
        }

        let deleteAnnotation = function (value) {

            if (value.annotationId) {
                let annotationId = value.annotationId;
                postservice.deleteAnnotation(annotationId, function (serverResponse) {
                    let status = serverResponse.status;
                    if (status === 200) {
                        updateAnnotationValue("");
                        callServiceGetThread(postUrl());
                        deletedAnnotStatus(true);
                    } else {
                        deletedAnnotStatus(false);
                    }
                });

            } else {
                deletedAnnotStatus(false);
            }

        };
       


        annotationBodyText.subscribe(function (annotBody) {            
            if (annotBody.length === 0 ) {
                return;
            } 
            const createAnnotObject = {
                postid: annotatedPostValues().id,
                annotBody
            };

            postservice.saveAnnotationOnPost(createAnnotObject, function (responseFromServer) {
                if (responseFromServer) {
                    annotationBodyText("");
                    newAnnotation(responseFromServer)
                    callServiceGetThread(postUrl());
                }

            });
        });
 

        function callServiceGetThread(postUrl) {
            postservice.getAllChildDataOfPostUrl(postUrl, function (responseFromServer) {
                if (responseFromServer) {
                    postDetails(responseFromServer);
                    postAnnotationsArray(responseFromServer)
                    showspinner(false);
                }
            });
        }

        //comp change requested
        function changeComp(component) {
            if (component === 'previous' && storedPreviousView) {
               // saveStuff();
                messaging.dispatch(messaging.actions.selectMenu(storedPreviousView));
            }
        };

        //get previous component/view
        let storedPreviousView = messaging.getState().selectedPreviousView;

        //store current component name
        //or not, to have previous page restore currentpage also, ifusing 'back' button
        //messaging.dispatch(messaging.actions.selectPreviousView("postdetails"));

        return {
            changeComp,
            postUrl,
            postDetails,
            addAnnotation,
            addBookmark,
            showspinner,
            showAnnotTextArea,
            responseData,
            annotationBodyText,
            annotatedPostValues,
            newAnnotation,
            updateAnnotationValue,
            updateAnnotation,
            deleteAnnotation,
            deletedAnnotStatus
        };
    };
});