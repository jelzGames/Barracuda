import { handleResponse, handleError, serviceKeys, getApiUrl } from "./apiUtils";
import getRequestOptions from "./authorizationUtils";
import axios from "axios";

export const Get = (id, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.get(getApiUrl(serviceKeys.api) + `/api/Users/Get/${id}`, options)
        .then(handleResponse)
        .catch(handleError);
};

export const Create = (model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.post(getApiUrl(serviceKeys.api) + `/api/users/Create`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const Update = (model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.put(getApiUrl(serviceKeys.api) + `/api/Users/Update`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const GetAll = (model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.post(getApiUrl(serviceKeys.api) + `/api/Users/GetAll`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const Delete = (id, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.delete(getApiUrl(serviceKeys.api) + `/api/users/Delete/${id}`, options)
        .then(handleResponse)
        .catch(handleError);
};