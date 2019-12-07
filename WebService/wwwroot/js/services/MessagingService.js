define(["knockout"], function (ko) {
    const selectPost = "SELECT_POST";
    const selectMenu = "SELECT_MENU";

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
                console.log("in reducer: ", action.selectedPost);
                return Object.assign({}, state, { selectedPost: action.selectedPost });
            case selectMenu:
                return Object.assign({}, state, { selectedMenu: action.selectedMenu });
            default:
                return state;
        }
    };

    var dispatch = function (action) {
        console.log("Subs in action: ", action);
        currentState = reducer(currentState, action);
        console.log("Subs in messaging: ", subscribers);
        console.log(" subscribers.filter(x => x.action === action): ", subscribers.filter(x => x.action === action));
        subscribers.filter(x => x.action === action.type).forEach(subscriber => subscriber.callback());
    };

    var actions = {
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