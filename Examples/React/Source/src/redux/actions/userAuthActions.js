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
        await usersAuthApi.Logout();
        localStorage.setItem(constants.UserAuth, JSON.stringify(initialState.userAuth));
        dispatch(LoginSuccesfull(initialState.userAuth));
        return true;
    }
}

export const RemoveRefreshToken = () => {
    return async function(dispatch) {
        await usersAuthApi.Refresh(true);
        localStorage.setItem(constants.UserAuth, JSON.stringify(initialState.userAuth));
        dispatch(LoginSuccesfull(initialState.userAuth));
        return true;
    }
}

export const LogIn = (model) => {
    return async function(dispatch) {
        var flag = false;
        var result = await usersAuthApi.Login(model)
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
            if(result.validEmail){
                setUserAuth(result, dispatch);
            }
            else{
                throw constants.NotValidEmailConfirmation;
            }
        }

        return true;
    }
}

const setUserAuth = async(result, dispatch) => {
    await tokenRefresh();
    setAuht(result, dispatch);
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
            user = result;
        })
        .catch((error) => {
            flag = false;
        });
        setUserAuth(user, dispatch);
        
        return flag;
    }
}

export const ChangePassword = (model, token) => {
    return async function(dispatch) {
        await usersAuthApi.ChangePassword(model, token)
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

export const ForgotPassword = (email) => {
    return async function(dispatch) {
        await usersAuthApi.ForgotPassword(email)
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

export const ValidEmail = (token) => {
    return async function(dispatch) {
        await usersAuthApi.ValidEmail(token)
        .then( (result) => {
        })
        .catch((error) => {
            if (error === constants.NotAuthorized) {
                console.log("Not Authorized")
            }
            else if(error === constants.ValidateTokenConfirmEmailExpired){
                throw constants.ValidateTokenConfirmEmailExpired;
            }
            else {
                console.log(error)
            }
        });
    }
}
export const ResendValidEmail = (token, resendEmail) => {
    return async function(dispatch) {
        await usersAuthApi.ResendValidEmail(token, resendEmail)
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
