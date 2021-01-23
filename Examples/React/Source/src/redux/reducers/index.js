import { combineReducers } from "redux";
import userAuth from "./userAuthReducer";

// eslint-disable-next-line max-lines-per-function
const createRootReducer = (history) => combineReducers({
    userAuth
});

export default createRootReducer;