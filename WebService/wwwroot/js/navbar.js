define(["knockout", "messaging"], function (ko, messaging) {

    /////////////////Menu elements
    let menuElements = [
        {
            name: "Search",
            component: "homepage"
        },
        {
            name: "Browse",
            component: "browse"
        },
        {
            name: "WordCloud",
            component: "wordcloud"
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
        },
        {
            name: "Search History",
            component: "searchhistory"
        }
    ];
    /////////////////Other components elements
    let otherElements = [
        {
            component: "authentication"
        },
        {
            component:"postdetails"
        },
        {
            component: "prebuttcomp"
        },
        {
            component: "searchbuttcomp"
        },
        {
            component: "hisbuttcomp"
        }
    ];

    let currentMenu = ko.observable(menuElements[0]);
    let currentComponent = ko.observable();
    let isTokenSet = ko.observable(false);
    if (window.localStorage.getItem('userToken')) {
        currentComponent = ko.observable(currentMenu().component);
        isTokenSet(true);
    } else {
        currentComponent = ko.observable(otherElements[0].component);
    }


    let changeContent = function (menu) {
        messaging.dispatch(messaging.actions.selectMenu(menu.name));
    };

    messaging.subscribe("SELECT_MENU", () => {
        let menuName = messaging.getState().selectedMenu;
        let menu = menuElements.find(x => x.name === menuName);
        if (menu) {
            currentMenu(menu);
            currentComponent(menu.component);
        } else {
            let component = otherElements.find(x => x.component === menuName);
            currentComponent(component.component);
        }
    });

    let isSelected = function (menu) {
        return menu === currentMenu() ? "active" : "";
    };

    let signOutUser = function () {
        console.log("sign out clicked");
        localStorage.removeItem('userToken');
        //window.localStorage.clear();
        window.location.reload();
    };

    return {
        currentComponent,
        menuElements,
        changeContent,
        isSelected,
        isTokenSet,
        signOutUser
        
    };
});