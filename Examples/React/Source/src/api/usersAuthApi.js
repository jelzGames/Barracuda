import { handleResponse, handleError, serviceKeys, getApiUrl } from "./apiUtils";
import getRequestOptions from "./authorizationUtils";
import axios from "axios";

//Here start the SSO functions
export const Register = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/Register`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const Login = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/Login`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const Logout = async(cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.get(getApiUrl(serviceKeys.api) + `/api/permissions/Logout`, options)
        .then(handleResponse)
        .catch(handleError);
};

export const RemoveRefreshToken = async(cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.get(getApiUrl(serviceKeys.api) + `/api/permissions/RemoveRefreshToken`, options)
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
// End SSO Functions

// Start Administrative
export const AddUser = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/AddUser`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const DeleteUser = async(id, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.delete(getApiUrl(serviceKeys.api) + `/api/permissions/DeleteUser/${id}`, options)
        .then(handleResponse)
        .catch(handleError);
};

export const ChangePasswordToUser = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/ChangePasswordToUser`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const UpdateTenants = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/UpdateTenants`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const UpdateScopes = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/UpdateScopes`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

export const GetAdditional = async(id, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.get(getApiUrl(serviceKeys.api) + `/api/permissions/GetAdditional/${id}`, options)
        .then(handleResponse)
        .catch(handleError);
};

export const BlockUser = async(model, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.post(getApiUrl(serviceKeys.api) + `/api/permissions/BlockUser`, model, options)
        .then(handleResponse)
        .catch(handleError);
};

// End Administrative

//Generic
export const CheckEmail = async(email, cancelToken) => {
    const options = getRequestOptions(cancelToken);
    return await axios.get(getApiUrl(serviceKeys.api) + `/api/permissions/CheckEmail/${email}`, options)
        .then(handleResponse)
        .catch(handleError);
};
//Generic End