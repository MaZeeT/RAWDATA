define(["knockout"], function (ko) {
    const selectGeneralComp = "SELECT_Gen_Comp";
    const selectMenu = "SELECT_MENU";

    var subscribers = [];

    var currentState = {};

    var getState = () => currentState;

    var subscribe = function (callback) {
        subscribers.push(callback);

        return function () {
            subscribers = subscribers.filter(x => x !== callback);
        };
    };

    var reducer = function (state, action) {
        switch (action.type) {
            case selectGeneralComp:
                return Object.assign({}, state, { selectedComp: action.selectedComp });
            case selectMenu:
                return Object.assign({}, state, { selectedMenu: action.selectedMenu });
            default:
                return state;
        }
    };

    var dispatch = function (action) {
        currentState = reducer(currentState, action);

        subscribers.forEach(callback => callback());
    };

    var actions = {
        selectGeneralComponent: function (component) {
              return {
                  type: selectGeneralComp,
                  selectedComp: component
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