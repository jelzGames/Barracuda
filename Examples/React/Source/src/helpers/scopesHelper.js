import { store } from "../redux/store";
import { BOAAdmin } from "../constants";

export const ValidScopes = (scope) => {
    var idx = store.getState().userAuth.scopes.findIndex((e) => e === scope || e === BOAAdmin);
    if(idx > -1){
        return true;
    }
    return false;
}