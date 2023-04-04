import {apiClient} from "@/state/modules/apiClient";
const controller = "Export";
export const actions = {
    async dsUser({commit} ) {
        return apiClient.get(controller + "/excel-dsuser");
    }
};
