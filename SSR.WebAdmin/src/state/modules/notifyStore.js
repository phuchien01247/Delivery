import { apiClient } from "@/state/modules/apiClient";
const controller = "Notify";

export const actions = {
  getListNotify({ commit }) {
    return apiClient.get(controller + "/get-list-notify");
  },
  changeStatus({ commit, dispatch}, values) {
    return apiClient.put(controller + "/change-status", values);
  },
  luuCVNoiBo({ commit, dispatch}, id) {
    return apiClient.get(controller + "/luu-cong-van-noi-bo/" + id);
  },
  luuCVDiNoiBo({ commit, dispatch}, id) {
    return apiClient.get(controller + "/luu-cong-van-di-noi-bo/" + id);
  },
  getUserByThongBaoId({ commit, dispatch},values) {
    return apiClient.post(controller + "/get-user-by-thong-bao-id",values);
  },
};
