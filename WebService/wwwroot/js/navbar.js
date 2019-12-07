﻿define(["knockout", "messaging"], function (ko, messaging) {

    /////////////////Menu elements
    var menuElements = [
        {
            name: "Home",
            component: "homepage"
        },
        {
            name: "History",
            component: "history"
        },
        {
            name: "Bookmarks",
            component: "bookmarks"
        },
        {
            name: "Annotations",
            component: "annotations"
        }
    ];
    /////////////////Other components elements
    var otherElements = [
        {
            component: "authentication"
        }
    ]

    var currentMenu = ko.observable(menuElements[0]);
    var currentComponent = ko.observable();
    let isTokenSet = ko.observable(false);
    if (window.localStorage.getItem('userToken')) {
        currentComponent = ko.observable(currentMenu().component);
        isTokenSet(true);
    } else {
        currentComponent = ko.observable(otherElements[0].component);
    }
   

    var changeContent = function (menu) {
        messaging.dispatch(messaging.actions.selectMenu(menu.name));
    };

    messaging.subscribe(() => {
        var menuName = messaging.getState().selectedMenu;
        var menu = menuElements.find(x => x.name === menuName);
        if (menu) {
            currentMenu(menu);
            currentComponent(menu.component);
        }
    });

    var isSelected = function (menu) {
        return menu === currentMenu() ? "active" : "";
    };

   


    return {
        currentComponent,
        menuElements,
        changeContent,
        isSelected,
        isTokenSet
        
    };
});