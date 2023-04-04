import {apiClient} from "@/state/modules/apiClient";
const controller = "VanBanDi";
export const actions = {
    async get({commit}) {
        return apiClient.get(controller +"/get");
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller + "/get-paging-params", params);
    },
    async getPagingParamsBanHanh({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-banhanh", params);
    },
    async getPagingParamsQuanLyVanBanDi({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-quanlyvanbandi", params);
    },
    async getPagingParamsCapSo({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-capso", params);
    },
    async getPagingParamsXuLy({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-xuly", params);
    },
    async getPagingParamsTatCa({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-tatca", params);
    },
    async getPagingParamsVM({commit}, params) {
        return apiClient.post(controller + "/get-paging-params-vm", params);
    },
    async create({commit}, values) {
        return apiClient.post(controller + "/create", values);
    },
    async xacThuc({commit}, values) {
        return apiClient.post("SignDigital" + "/xac-thuc", values);
    },
    async capSoVanBanDi({commit}, values) {
        return apiClient.post(controller + "/cap-so-van-ban-di", values);
    },
    async assignOrReject({commit}, values) {
        return apiClient.post(controller + "/assign-or-reject", values);
    },
    async assignOrRejectPhapLy({commit}, values) {
        return apiClient.post(controller + "/ky-so-phap-ly", values);
    },
    async assignSign({commit}, values) {
        return apiClient.post(controller + "/them-nguoi-ky-so", values);
    },
    async removeAssignSign({commit}, values) {
        return apiClient.post(controller + "/xoa-nguoi-ky-so", values);
    },
    async update({commit, dispatch}, values) {
        return apiClient.put(controller + "/update", values);
    },
    async delete({commit}, id) {
        return await apiClient.delete(controller + "/delete/" + id);
    },
    async getById({commit}, id) {
        console.log("module", id)
        return apiClient.get(controller + "/get-by-id/" + id);
    },
    async getPhanCongKySoByVanBanId({commit}, id) {
        return apiClient.get(controller + "/get-phancongkyso-by-vanbanid/" + id);
    },
    async getVaCapSo({commit}, id) {
        return apiClient.get(controller + "/cap-so-khi-tao-van-ban");
    },
    async chuyenTrangThaiVanBan({commit}, values) {
        return apiClient.post(controller + "/chuyen-trang-thai-van-ban", values);
    },
    async lanhDaoTuChoiVanBan({commit}, values) {
        return apiClient.post(controller + "/lanh-dao-tu-choi-van-ban", values);
    },
    async thuHoiVanBan({commit}, id) {
        return apiClient.get(controller + "/thu-hoi-van-ban-di/" + id);
    },
    async resetNhomNguoiTiepNhan({commit}, id) {
        return apiClient.get(controller + "/reset-nhom-nguoi-tiep-nhan/" + id);
    },
};
