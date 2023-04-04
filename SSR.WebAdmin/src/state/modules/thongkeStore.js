import {apiClient} from "@/state/modules/apiClient";
const controller = "ExportFile";
export const actions = {
    async renderTable({commit}, values ) {
        return apiClient.post(controller +"/render-table" , values);
    },
    async exportThongKe({commit}, values ) {
        return apiClient.post(controller +"/get-export" , values);
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller + "/get-paging-params", params);
    }
};
