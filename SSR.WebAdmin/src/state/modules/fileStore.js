import {apiClient} from "@/state/modules/apiClient";
const controller = "Files";
export const actions = {
    async viewFile({commit}, id) {
        return apiClient.getFile(controller + "/view/" + id);
    },
    async uploadFile({commit}, file) {
        return apiClient.post(controller + "/upload", file);
    }
};
