import {apiClient} from "@/state/modules/apiClient";
const  controller = "Label";
export const actions = {
    async get({commit}) {
        return apiClient.get(controller +"/get-all-data");
    },
    async getwithprojid({commit}, id) {
        return apiClient.get(controller +"/get-with-projid/"+ id);
    },
    async getfind({commit}, key) {
        return apiClient.get(controller +"/get-find", key);
    },
    async getDonViCha({commit}) {
        return apiClient.get(controller +"/getTree");
    },
    async getTree({commit}) {
        return apiClient.get(controller +"/get-tree");
    },
    async getTreewithprojid({commit}, id) {
        return apiClient.get(controller +"/get-tree-with-projid/" + id);
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller + "/get-paging-params", params);
    },
    async create({commit}, values) {
        return apiClient.post(controller + "/create", values);
    },
    async update({commit, dispatch}, values) {
        return apiClient.put(controller + "/update", values);
    },
    async delete({commit}, id) {
        return await apiClient.delete(controller + "/delete/" + id);
    },
    async getById({commit}, id) {
        return apiClient.get(controller + "/get-by-id/" + id);
    },
    async getByIdByFields({commit}, params) {
        return apiClient.post(controller + "/get-by-id-by-fields" , params);
    },
    async addFields({commit}, values) {
        return apiClient.post(controller + "/add-fields", values);
    },
    async deleteFields({commit}, values) {
        return apiClient.post(controller + "/delete-fields", values);
    },
};
