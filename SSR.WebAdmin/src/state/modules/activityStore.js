import {apiClient} from "@/state/modules/apiClient";
const controller = "Activities";
export const actions = {

    async getByIssueId({commit}, id) {
        return apiClient.get(controller + "/get-by-issue-id/" + id);
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller + "/get-paging-params", params);
    },
    
    async getByProjectId({commit}, id) {
        return apiClient.get(controller + "/get-by-project-id/" + id);
    }
};
