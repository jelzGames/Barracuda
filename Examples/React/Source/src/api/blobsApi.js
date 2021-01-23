import { handleResponse, handleError, serviceKeys, getApiUrl } from "./apiUtils";
import getRequestOptions from "./authorizationUtils";
import axios from "axios";


export const GetSASToken = (blobName, permissions, size, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.get(getApiUrl(serviceKeys.api) + `/api/Blobs/GetSASToken?blobName=${blobName}&permissions=${permissions}&size=${size}`, options)
        .then(handleResponse)
        .catch(handleError);
};

export const CreateBlob = (model) => {
    const options = getRequestOptions();
    return axios.post(getApiUrl(serviceKeys.api) + "/api/Blobs/Create",model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const GetBlob = (id, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.get(getApiUrl(serviceKeys.api) + `/api/Blobs/Get/${id}`, options)
        .then(handleResponse)
        .catch(handleError);
};

export const UpdateBlob = (model) => {
    const options = getRequestOptions();
    return axios.put(getApiUrl(serviceKeys.api) + "/api/Blobs/Update", model , options)
        .then(handleResponse)
        .catch(handleError);
};

export const GetAll = (model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return axios.post(getApiUrl(serviceKeys.api) + "/api/Blobs/GetAll", model , options)
        .then(handleResponse)
        .catch(handleError);
};

export const DeleteBlob = (id) => {
    const options = getRequestOptions();
    return axios.delete(getApiUrl(serviceKeys.api) + `/api/Blobs/Delete/${id}`, options)
        .then(handleResponse)
        .catch(handleError);
};