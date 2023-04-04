import axios  from "axios";
import Vue from 'vue'
import {CONSTANTS} from "@/helpers/constants";

const path = process.env.VUE_APP_API_URL;

/** 400: Bad Request */
export const CLIENT_ERROR_CODE = 400;

export const httpClient = axios.create({
    baseURL: path,
    json: true,
    headers: {
        'Content-Type' : 'application/json'
    },
    timeout: 300000
})

class ApiClient{
    getInstance(){
        if(!Vue.prototype.$auth_token){
            let token = JSON.parse(localStorage.getItem("user-token"));
            Vue.prototype.$auth_token = token;
        }
        httpClient.defaults.headers.common['Authorization'] = `Bearer ${Vue.prototype.$auth_token}`
        return httpClient;
    }
    async get(url, config = null) {
        try {
            const response = await this.getInstance().get(path + url);
            return response.data;
        } catch (e) {
            return {
                success: false,
                code: CLIENT_ERROR_CODE,
                message: e.toString(),
            };
        }
    }
    async getFile(url, config = null) {
        try {
            const response = await this.getInstance().get(path + url);
            return response;
        } catch (e) {
            return {
                success: false,
                code: CLIENT_ERROR_CODE,
                message: e.toString(),
            };
        }
    }
    /**
     * Wrapper for axios.post method.
     * @param {String} url
     * @param {*} data
     */
    async post(url, data, config = null) {
        try {
            const response = await this.getInstance().post(path + url, data);
            return response.data;
        } catch (e) {
            console.log(e)
            return {
                resultString: e.toString(),
                resultCode: CONSTANTS.ERROR,
            };
        }
    }

    /**
     * Wrapper for axios.post method.
     * @param {String} url
     * @param {*} data
     * @param {AxiosRequestConfig} config
     */
    async put(url, data, config = null) {

        try {
            const response = await this.getInstance().post(path + url, data);
            return response.data;
        } catch (e) {
            return {
                resultString: e.toString(),
                resultCode: CONSTANTS.ERROR,
            };
        }
    }

    /**
     * Wrapper for axios.post method.
     * @param {String} url
     * @param {AxiosRequestConfig} config
     */
    async delete(url, config = null) {
        try {
            const response = await this.getInstance().post(path + url);
            return response.data;
        } catch (e) {
            return {
                resultString: e.toString(),
                resultCode: CONSTANTS.ERROR,
            };
        }
    }
    async deleteObject(url,data, config = null) {
        try {
            const response = await this.getInstance().post(path + url , data);
            return response.data;
        } catch (e) {
            return {
                resultString: e.toString(),
                resultCode: CONSTANTS.ERROR,
            };
        }
    }
}

export const apiClient = new ApiClient();
