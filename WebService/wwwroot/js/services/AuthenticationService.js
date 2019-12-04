define(["jquery"], function () {
    
    const data = { Username: 'Monica', Password:'Test'};
    var getLoginUser = async function (callback) {

        try {
            const response = await fetch("api/auth/tokens", {
                method: 'POST', // or 'PUT'
                body: JSON.stringify(data), // data can be `string` or {object}!
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            //console.log('Success:', JSON.stringify(response));
            callback(response)
        } catch (error) {
            console.error('Error:', error);
        }
    };

    return {
        getLoginUser
    }
});