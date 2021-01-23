import * as types from "./actionTypes";
import * as constants from "../../constants";
import initialState from "../reducers/initialState";
import * as usersAuthApi from "../../api/usersAuthApi";

export const LoginSuccesfull = (userAuth) => {
    return { type: types.LoginSuccessful, userAuth };
};

export const GetUserAuth = () => {
    return async function(dispatch) {
        var userAuth = localStorage.getItem(constants.UserAuth);
        if (userAuth !== "undefined") {
            userAuth = JSON.parse(userAuth);
        }
        else {
            userAuth = initialState.userAuth;
        }
        dispatch(LoginSuccesfull(userAuth));
        return true;
    }
}

export const Logout = () => {
    return async function(dispatch) {
        localStorage.setItem(constants.UserAuth, JSON.stringify(initialState.userAuth));
        dispatch(LoginSuccesfull(initialState.userAuth));
        return true;
    }
}

export const LogIn = (model) => {
    return async function(dispatch) {
        var flag = false;
        var result = await usersAuthApi.Auth(model)
        .then( (result) => {
            flag = true;
            return result;
        })
        .catch((error) => {
            if (error === constants.NotFound) {
                console.log("Not Found")
            }
            else if (error === constants.NotAuthorized) {
                console.log("Not Authorized")
            }
            else {
                console.log(error)
            }
            throw error;
        })
   
        if (flag) {
            setUserAuth(result, dispatch);
        }

        return true;
    }
}

const setUserAuth = async(result, dispatch) => {
    await tokenRefresh();
    var userAuth = {...initialState.userAuth};
    userAuth.id = result.id;
    userAuth.email = result.email;
    setAuht(userAuth, dispatch);
}

const tokenRefresh = async() => {
    await usersAuthApi.RefreshToken()
    .then((result) => {
    })
    .catch((error) => {
    });
}

const setAuht = (userAuth, dispatch) => {
    localStorage.setItem(constants.UserAuth, JSON.stringify(userAuth));
    dispatch(LoginSuccesfull(userAuth));
}

export const Register = (model) => {
    return async function(dispatch) {
        await usersAuthApi.Register(model)
        .then((result) => {
        })
        .catch((error) => {
            if (error === constants.Found) {
                console.log("Found")
            }
            else {
                console.log(error)
            }
            throw error;
        })
    }
}

export const SocialGoogle = (model) => {
    return async function(dispatch) {
        var flag = false;
        var result = await usersAuthApi.SocialGoogle(model)
        .then( (result) => {
            flag = true;
            return result;    
        })
        .catch((error) => {
            if (error === constants.NotAuthorized) {
                console.log("Not Authorized")
            }
            else {
                console.log(error)
            }
        });
        
        if (flag) {
            setUserAuth(result, dispatch);
        }
    }
}

export const SocialFacebook = (model) => {
    return async function(dispatch) {
        var flag = false;
        var result = await usersAuthApi.SocialFacebook(model)
        .then( (result) => {
            flag = true;
            return result;    
        })
        .catch((error) => {
            if (error === constants.NotAuthorized) {
                console.log("Not Authorized")
            }
            else {
                console.log(error)
            }
        });
        
        if (flag) {
            setUserAuth(result, dispatch);
        }
    }
}

export const SocialMicrosoft = (model) => {
    return async function(dispatch) {
        var flag = false;
        var result = await usersAuthApi.SocialMicrosoft(model)
        .then( (result) => {
            flag = true;
            return result;    
        })
        .catch((error) => {
            if (error === constants.NotAuthorized) {
                console.log("Not Authorized")
            }
            else {
                console.log(error)
            }
        });
        
        if (flag) {
            setUserAuth(result, dispatch);
        }
    }
}

export const RefreshToken = (user) => {
    return async function(dispatch) {
        var flag = true;
        await usersAuthApi.Refresh()
        .then((result) => {
            user.id = result.id;
            user.email = result.email;
        })
        .catch((error) => {
            flag = false;
        });
        setUserAuth(user, dispatch);
        
        return flag;
    }
}

export const ChangePassword = (model) => {
    return async function(dispatch) {
        await usersAuthApi.ChangePassword(model)
        .then( (result) => {
        })
        .catch((error) => {
            if (error === constants.NotAuthorized) {
                console.log("Not Authorized")
            }
            else {
                console.log(error)
            }
        });
    }
}
