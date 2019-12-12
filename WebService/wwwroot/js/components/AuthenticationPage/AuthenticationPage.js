define(["knockout", "authservice", "messaging"], function (ko, authservice, messaging) {

    return function () {

        //LogInForm
        let usernamePrefill = "Username";
        let passwordPrefill = "Password";

        let loginUsername = ko.observable(usernamePrefill);
        let loginPassword = ko.observable(passwordPrefill);

        let getCredentials = function (username, password) {
            if (username && username !== usernamePrefill && password && password !== passwordPrefill) {
                console.log("Correct");
                return {Username: username, Password: password};
            } else {
                console.log("Incorrect");
                usernamePrefill = "Input your username here :)";
                return null;
            }
        };

        let loginUser = function (data) {
            const login = getCredentials(loginUsername(), loginPassword());
            if (login !== null) {
                authservice.getLoginUser(login, function (authenticationResponse) {
                    const token = authenticationResponse.token;
                    if (token) {
                        console.log('If token yes');
                        window.localStorage.setItem("userToken", token);
                        window.location.reload();
                        //console.log(messaging.actions);
                        //messaging.dispatch(messaging.actions.selectMenu("Home"));

                       /* //get previous component/view
                        let storedPreviousView = messaging.getState().selectedPreviousView;
                        console.log('stored comp : ', storedPreviousView);
                        if (storedPreviousView) {
                            messaging.dispatch(messaging.actions.selectMenu(storedPreviousView));
                        } else messaging.dispatch(messaging.actions.selectMenu("Home"));*/
                    }
                });
            }
        };

        function clearInputFields(field) {

            if (field === 'user' && loginUsername() === usernamePrefill) {
                loginUsername('')
            } else if (field === 'pass' && loginPassword() === passwordPrefill) {
                loginPassword('')
            }

        }


        let newUserSignup = function () {
            const login = getCredentials(loginUsername(), loginPassword());
            if (login !== null) {
                authservice.signUpUser(login, function (authenticationResponse) {
                    const token = authenticationResponse.token;
                    console.log("User created");
                    console.log(token);
                });
                console.log("I have been clicked for signup new user");
            }
        };

        return {
            loginUsername,
            loginPassword,
            loginUser,
            clearInputFields,
            newUserSignup

        }
    }

});