import env from "../utils/env"; 

export const serviceKeys = {
    api: "api",
};

export const handleResponse = async (response) => {
    if (response.status === 200) {
        return response.data; 
    }
}

export const handleError = (error) => {
    if (error.response && error.response.data) {
        throw error.response.data;
    }
    
    throw error;
};

export const getApiUrl = (serviceKey) => {
    var url = "";
    if (serviceKey === serviceKeys.api) {
        url = env.api;
    }
    return url;
};