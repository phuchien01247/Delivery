import {apiClient} from "@/state/modules/apiClient";
const controller = "VanBanDen";
export const actions = {
    async get({commit}) {
        return apiClient.get(controller +"/get");
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller + "/get-paging-params", params);
    },
    async getPagingParamsXuLy({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-xuly", params);
    },
    async create({commit}, values) {
        return apiClient.post(controller + "/create", values);
    },
    async update({commit, dispatch}, values) {
        return apiClient.put(controller + "/update", values);
    },
    async butPhe({commit, dispatch}, values) {
        console.log("but-phe", values);
        return apiClient.put(controller + "/but-phe", values);
    },
    async phanCong({commit, dispatch}, values) {
        console.log("phan-cong", values);
        return apiClient.put(controller + "/phan-cong", values);
    },
    async delete({commit}, id) {
        return await apiClient.delete(controller + "/delete/" + id);
    },
    async getById({commit}, id) {
        return apiClient.get(controller + "/get-by-id/" + id);
    },
    async chuyenTrangThaiVanBan({commit}, values) {
        console.log(" value cttvb", values);
        return apiClient.post(controller + "/chuyen-trang-thai-van-ban", values);
    },
    async xuLyNhiemVuVanBanDen({commit}, values) {
        return apiClient.post(controller + "/xu-ly-nhiem-vu-van-ban-den", values);
    },
};
