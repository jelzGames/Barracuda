import * as types from "../actions/actionTypes";
import initialState from "./initialState";

export default function LoginReducer(state = initialState.userAuth, action) {
    if (action.type === types.LoginSuccessful) {
        return action.userAuth;
    }
    else {
        return state;
    }
}