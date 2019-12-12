define(["knockout", "authservice", "messaging"], function (ko, authservice, messaging) {

    return function () {

        //LogInForm
        let loginUsername = ko.observable("Username");
        let loginPassword = ko.observable("Password");

        let getCredentials = function (username, password) {
            if (username && username !== "Username" && password && password !== "Password") {
                console.log("Correct");
                return {Username: username, Password: password};
            }else {
                console.log("Incorrect");
            }
        };

        let loginUser = function (data) {
                const login = getCredentials(loginUsername(), loginPassword());
                authservice.getLoginUser(login, function (authenticationResponse) {
                    const token = authenticationResponse.token;
                    if (token) {
                        console.log('If token yes');
                        window.localStorage.setItem("userToken", token);
                        window.location.reload();
                        //console.log(messaging.actions);
                        //messaging.dispatch(messaging.actions.selectMenu("Home"));
                    }
                });

        };

        function clearInputFields(field) {

            if (field === 'user' && loginUsername() === "Username") {
                loginUsername('')
            } else if (field === 'pass' && loginPassword() === "Password") {
                loginPassword('')
            }

        }


        let newUserSignup = function () {
            const login = getCredentials(loginUsername(), loginPassword());
            authservice.signUpUser(login, function (authenticationResponse) {
                const token = authenticationResponse.token;
                console.log("User created");
                console.log(token);
            });
            console.log("I have been clicked for signup new user");
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