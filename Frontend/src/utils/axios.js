import axios from "axios";
import { useTokens } from "../stores/tokenStore.js";
import { refreshTokens } from "./utils.js";

const api = axios.create({
    baseURL: `api/`,
    headers: {
        'Content-Type': 'application/json',
    }
})

api.interceptors.request.use(
    (config) => {
        const accessToken = useTokens.getState().accessToken
        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
)

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });
    failedQueue = [];
}

api.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        const clearTokens = useTokens.getState().clearTokens
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then(token => {
                    originalRequest.headers.Authorization = 'Bearer ' + token;
                    return api(originalRequest);
                }).catch(err => {
                    return Promise.reject(err);
                });
            }

            isRefreshing = true;

            try {
                const newToken = await refreshTokens();
                processQueue(null, newToken)
                originalRequest.headers.Authorization = 'Bearer ' + newToken;
                return api(originalRequest)
            } catch (error) {
                processQueue(error, null);
                clearTokens();
                return Promise.reject(error);
            }
            finally {
                isRefreshing = false;
            }
        }
        return Promise.reject(error);
    }
)

export default api