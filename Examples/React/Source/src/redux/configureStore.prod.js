import { createStore, applyMiddleware, compose } from "redux";
import createRootReducer from "./reducers";
import thunk from "redux-thunk";
//import { loadUser } from "redux-oidc";
//import userManager from "../services/userManager";
import { history } from "./history";

export default function configureStore(initialState) {
    const store = createStore(
        createRootReducer(history),
        initialState,
        compose(
            applyMiddleware(
                thunk
            )
        ),
    );

    //loadUser(store, userManager);

    return store;
}