define([""], function () {
    const selectPost = "SELECT_POST";
    const selectSearchTerms = "SELECT_TERMS";
    const selectSearchOptions = "SELECT_OPTIONS";
    const selectPreviousView = "SELECT_PREVVIEW";
    const selectMaxWords = "SELECT_MAXWORDS";
    const selectMenu = "SELECT_MENU";
    const selectMaxPages = "SELECT_MAXPU";
    const selectCurrentPage = "SELECT_CURRP";

    var subscribers = [];

    var currentState = {};

    var getState = () => currentState;

    var subscribe = function (action, callback) {
        let obj = {
            action,
            callback
        }
        subscribers.push(obj);

        return function () {
            subscribers = subscribers.filter(x => x.action !== action && x.callback !== callback);
        };
    };

    var reducer = function (state, action) {
        switch (action.type) {
            case selectPost:
                return Object.assign({}, state, { selectedPost: action.selectedPost });
            case selectMaxPages:
                return Object.assign({}, state, { selectedMaxPages: action.selectedMaxPages });
            case selectCurrentPage:
                return Object.assign({}, state, { selectedCurrentPage: action.selectedCurrentPage });
            case selectSearchTerms:
                return Object.assign({}, state, { selectedSearchTerms: action.selectedSearchTerms });
            case selectSearchOptions:
                return Object.assign({}, state, { selectedSearchOptions: action.selectedSearchOptions });
            case selectMaxWords:
                return Object.assign({}, state, { selectedMaxWords: action.selectedMaxWords });
            case selectPreviousView:
                return Object.assign({}, state, { selectedPreviousView: action.selectedPreviousView });
            case selectMenu:
                return Object.assign({}, state, { selectedMenu: action.selectedMenu });
            default:
                return state;
        }
    };

    var dispatch = function (action) {
        currentState = reducer(currentState, action);
        subscribers.filter(x => x.action === action.type).forEach(subscriber => subscriber.callback());
    };

    var actions = {
        selectSearchTerms: function (post) {
            return {
                type: selectSearchTerms,
                selectedSearchTerms: post
            };
        },
        selectMaxPages: function (post) {
            return {
                type: selectMaxPages,
                selectedMaxPages: post
            };
        },
        selectCurrentPage: function (post) {
            return {
                type: selectCurrentPage,
                selectedCurrentPage: post
            };
        },
        selectSearchOptions: function (post) {
            return {
                type: selectSearchOptions,
                selectedSearchOptions: post
            };
        },
        selectMaxWords: function (post) {
            return {
                type: selectMaxWords,
                selectedMaxWords: post
            };
        },
        selectPreviousView: function (post) {
            return {
                type: selectPreviousView,
                selectedPreviousView: post
            };
        },
        selectPost: function (post) {
            return {
                type: selectPost,
                selectedPost: post
            };
        },
        selectMenu: function (menu) {
            return {
                type: selectMenu,
                selectedMenu: menu
            };
        }
    };

    return {
        getState,
        subscribe,
        dispatch,
        actions
    };
});