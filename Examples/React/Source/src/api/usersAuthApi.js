import { handleResponse, handleError, serviceKeys, getApiUrl } from "./apiUtils";
import getRequestOptions from "./authorizationUtils";
import axios from "axios";

export const Register = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/Register`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const Auth = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/Auth`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const Refresh = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/Refresh`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const RefreshToken = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/RefreshToken`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const SocialGoogle = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/SocialGoogle`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const SocialFacebook = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/SocialFacebook`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const SocialMicrosoft = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/SocialMicrosoft`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const ChangePassword = async(model, token, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    if(token) {
        return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/ChangePassword?changepasswordtoken=${token}`, model, options)
        .then(handleResponse)
        .catch(handleError);
    }
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/ChangePassword`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const ForgotPassword = async(email, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.get(getApiUrl(serviceKeys.api) + `/api/permissions/ForgotPassword?email=${email}`, options)
        .then(handleResponse)
        .catch(handleError);
};

export const ValidEmail = async(token, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/ValidEmail?validEmailToken=${token}`, null, options)
        .then(handleResponse)
        .catch(handleError);
};

export const ResendValidEmail = async(token, resendEmail, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/ResendValidEmail?validEmailToken=${token}&email=${resendEmail}`, null, options)
        .then(handleResponse)
        .catch(handleError);
};

