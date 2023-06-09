import {apiClient} from "@/state/modules/apiClient";
const controller = "Comment";
export const actions = {
    async get({commit}) {
        return apiClient.get(controller +"/get");
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller + "/get-paging-params", params);
    },
    async getwithissid({commit}, id) {
        return apiClient.get(controller +"/get-with-issid/"+ id);
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
    async getBySlug({commit}, slug) {
        return apiClient.get(controller + "/get-by-slug/" + slug);
    }
};
