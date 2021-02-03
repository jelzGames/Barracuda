import axios from "axios";
import initialState from "../redux/reducers/initialState";
import * as constants from "../constants";
import * as usersAction from "../redux/actions/userAuthActions";
import { store } from "../redux/store";

axios.interceptors.response.use((response) => {
    return response;
}, async (error) => {
    console.log(error.response.data)
    if ((store.getState().userAuth && store.getState().userAuth.id) && 
        error.response && 
        error.response.data === constants.SecurityTokenExpired) {
        var original = error.config;
        var flag = await getRefreshToken();
        if (flag) {
            console.log("Token")
            return axios.request(original);
        }
        else {
            console.log("reject")
            return Promise.reject(original);
        }
    }
    else {
        console.log("final")
        return Promise.reject(error);
    }
});

const getRefreshToken = async() => {
    var user = {...initialState.userAuth};
    var flag = true;
    await store.dispatch(usersAction.RefreshToken(user))
    .then((result) => {
    })
    .catch((error) => {
        flag = false;
    });

    return flag;
}

export default function getRequestOptions(cancelToken) {
    var options = {
        headers: {
            "Content-Type": "application/json"
        },
        crossdomain: true,
        cancelToken
    };

    options.withCredentials = true;
    options.credentials = 'include';
    options.secure = true;
    options.sameSite = "strict";
    return options;
}