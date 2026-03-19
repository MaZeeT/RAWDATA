define(['knockout', 'postservice', 'messaging'], function (knockout, postService, messaging) {
    return function () {
        let postUrl = knockout.observable(messaging.getState().selectedPost);
        let annotationBodyText = knockout.observable("");
        let annotatedPostValues = knockout.observable();
        let updateAnnotationValue = knockout.observable("");

        let postDetails = knockout.observable([]);
        let postAnnotationsArray = knockout.observable([]);
        let showspinner = knockout.observable(true);
        let showAnnotTextArea = knockout.observable(false);
        let responseData = knockout.observable(false);
        let deletedAnnotStatus = knockout.observable(false);
        let newAnnotation = knockout.observable({});

        postService.getAllChildDataOfPostUrl(postUrl(), function (responseFromServer) {
            if (responseFromServer) {
                postDetails(responseFromServer);
                postAnnotationsArray(responseFromServer);
                showspinner(false);
            }
        });

        let addAnnotation = function (value, event) {
            showAnnotTextArea(true);
        };

        let addBookmark = function (value) {
            const createBookmarkUrl = value.createBookmarkLink;
            postService.savePostAsBookmark(createBookmarkUrl, function (responseFromServer) {
                responseData(responseFromServer);
            });
        };

        let updateAnnotation = function (value) {
            if (updateAnnotationValue() && value.annotationId) {
                let annotationId = value.annotationId;
                let annotationBody = updateAnnotationValue();
                postService.updateAnnotation(annotationId, annotationBody, function (serverResponse) {
                    let status = serverResponse.status;
                    if (status === 204) {
                        callServiceGetThread(postUrl());
                        updateAnnotationValue("");
                    }
                });
            }
        };

        let deleteAnnotation = function (value) {
            if (value.annotationId) {
                let annotationId = value.annotationId;
                postService.deleteAnnotation(annotationId, function (serverResponse) {
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
            if (annotBody.length === 0) {
                return;
            }
            const createAnnotObject = {
                postid: annotatedPostValues().id,
                annotBody
            };

            postService.saveAnnotationOnPost(createAnnotObject, function (responseFromServer) {
                if (responseFromServer) {
                    annotationBodyText("");
                    newAnnotation(responseFromServer);
                    callServiceGetThread(postUrl());
                }
            });
        });

        function callServiceGetThread(postUrl) {
            postService.getAllChildDataOfPostUrl(postUrl, function (responseFromServer) {
                if (responseFromServer) {
                    postDetails(responseFromServer);
                    postAnnotationsArray(responseFromServer);
                    showspinner(false);
                }
            });
        }

        //comp change requested
        function changeComp(component) {
            if (component === 'previous' && storedPreviousView) {
                messaging.dispatch(messaging.actions.selectMenu(storedPreviousView));
            }
        }

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