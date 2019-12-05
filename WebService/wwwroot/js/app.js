define(["knockout", "authservice"], function (ko, authservice) {

    //Navigation menu
    var currentComponent = ko.observable("index");
    var currentParams = ko.observable({});

    var onMenuItemClick = function (componentName) {
        currentComponent(componentName);
    };

    //LogInForm
    let loginUsername = ko.observable("Username");
    let loginPassword = ko.observable("Password");
    let loginUser = function (data) {
        let username = loginUsername();
        let password = loginPassword();
        if (username && username !== "Username" && password && password !== "Password") {
            console.log("Correct");
            const incomingUserCredentials = { Username: username, Password: password };
            authservice.getLoginUser(incomingUserCredentials, function (authenticationResponse) {
                const token = authenticationResponse.token;
                if (token) {
                    console.log('If token yes');
                    window.localStorage.setItem("userToken", token);
                    currentComponent("homepage");
                }
            });

        } else {
            console.log("Incorrect");
        }
    };

    var newUserSignup = function () {
        console.log("I have been clicked for signup new user");
    };

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