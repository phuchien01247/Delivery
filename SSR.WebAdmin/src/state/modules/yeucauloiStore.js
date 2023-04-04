import {apiClient} from "@/state/modules/apiClient";
const controller = "Issue";
export const actions = {
    async get({commit}) {
        return apiClient.get(controller +"/get");
    },
    async getPagingParamLoiDaGiaiQuyet({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-loi-da-giai-quyet", params);
    },
    async getPagingParamsLoiNgay({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-loi-ngay", params);
    },
    async getPagingParamsLoiTrongNgay({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-loi-trong-ngay", params);
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller + "/get-paging-params", params);
    },
    async getwithprojid({commit}, id) {
        return apiClient.get(controller +"/get-with-projid/"+ id);
    },
    async getopenwithprojid({commit}, id) {
        return apiClient.get(controller +"/get-open-with-projid/"+ id);
    },
    async getclosewithprojid({commit}, id) {
        return apiClient.get(controller +"/get-close-with-projid/"+ id);
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
    }
};
