import { createStore, applyMiddleware, compose } from "redux";
import createRootReducer from "./reducers";
import reduxImmutableStateInvariant from "redux-immutable-state-invariant";
import thunk from "redux-thunk";
//import { loadUser } from "redux-oidc";
//import userManager from "../services/userManager";
import { history } from "./history";

export default function configureStore(initialState) {
    const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose; // add support for Redux dev tools

    const store = createStore(
        createRootReducer(history),
        initialState,
        composeEnhancers(
            applyMiddleware(
                thunk,
                reduxImmutableStateInvariant()
            )
        )
    );

    //loadUser(store, userManager);

    return store;
}