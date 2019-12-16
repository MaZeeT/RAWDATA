define(["knockout", "authservice", "messaging", "validation"], function (ko, authservice, messaging, validation) {
    return function () {

        let loginUsername = ko.observable();
        let loginPassword = ko.observable();
        let errorMessage = ko.observable(false);
        let errorDuplicatedUserMessage = ko.observable(false);
        let showlogInUserForm = ko.observable(true);
        let showSignUpForm = ko.observable(false);

        let signupUserActionButton = function () {
            //hide the previous form and show new one
            showlogInUserForm(false);
        };
        
        let alreadyExistingUser = function () {
            //hide signup form and show login form
            showlogInUserForm(true);
        };

        // this function does the validation and depending on which form the request is coming from, it does login or signup
        let formSubmitAction = function () {
            let validPassword = validation.isValidPassword(loginPassword());
            let validUsername = validation.isValidUsername(loginUsername());
            if (!validPassword || !validUsername) {
                errorMessage(true);
                return; // return to break the if and not go further;
            }
            let userCredentials = { Username: loginUsername(), Password: loginPassword() };
            if (showlogInUserForm()) {
                loginUser(userCredentials);
            } else {
                signupUser(userCredentials);
            }
        };

        //register a new user
        function signupUser(userCredentials) {
            authservice.signUpUser(userCredentials, function (authenticationResponse) {
                // registering a new user doesn't come with a token by default ;) 
                if (authenticationResponse.status === 201) {
                    loginUser(userCredentials);
                    return;
                }
                // this one below is correct - the request is either 400 because of not unique user
                //or if the user manages to pass over bootstrap validation then iot can be 400 as well but for a different reason. 
                if (authenticationResponse.status === 400 && authenticationResponse.statusText == "Bad Request") {
                    errorDuplicatedUserMessage(true);
                } else {
                    errorMessage(true);
                }
            });
        }

        //login the user - set token to localstorage and then reload the page so that the home component loads
        function loginUser(userCredentials) {
            authservice.getLoginUser(userCredentials, function (authenticationResponse) {
                const token = authenticationResponse.token;
                if (token) {
                    window.localStorage.setItem("userToken", token);
                    window.location.reload();
                    messaging.dispatch(messaging.actions.selectMenu("Home"));
                }
            });
        }


        return {
            loginUsername,
            loginPassword,
            loginUser,
            signupUser,
            formSubmitAction,
            errorMessage,
            showlogInUserForm,
            showSignUpForm,
            signupUserActionButton,
            alreadyExistingUser,
            errorDuplicatedUserMessage

        }
    }

});