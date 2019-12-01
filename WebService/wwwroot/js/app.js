define(["knockout", "authservice"], function (ko, authserv) {

    //Navigation menu
    var currentComponent = ko.observable("index");
    var currentParams = ko.observable({});

    var onMenuItemClick = function (componentName) {
        currentComponent(componentName);
    };

    //LogInForm
    var loginUsername = ko.observable("Username");
    var loginPassword = ko.observable("Password");
    var loginUser = function (data) {
        let username = loginUsername();
        let password = loginPassword();
        if (username && username !== "Username" && password && password !== "Password") {
            console.log("Correct");
           
            authserv.getLoginUser(function (data) {
                console.log("Data ", data);
            });

        } else {
            console.log("Incorrect");
        }
    };

    var newUserSignup = function () {
        console.log("I have been clicked for signup new user");
    }

    return {
        currentComponent,
        currentParams,
        onMenuItemClick,
        loginUsername,
        loginPassword,
        loginUser,
        newUserSignup
    };
});