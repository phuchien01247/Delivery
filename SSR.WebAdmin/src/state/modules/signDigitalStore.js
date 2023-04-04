import {apiClient} from "@/state/modules/apiClient";
const controller = "SignDigital";

export const actions = {
    async kySoPhapLy({commit}, values) {
        return apiClient.post(controller + "/KySoPhapLy", values);
    },
    async thietLapKySoPhapLy({commit}, values) {
        return apiClient.post(controller + "/thiet-lap-ky-so-phap-ly", values);
    },
    async thucHienKySoPhapLy({commit}, values) {
        return apiClient.post(controller + "/ThucHienKySoPhapLy", values);
    },
    async thucHienDongMocThemSo({commit}, values) {
        return apiClient.post(controller + "/ThucHienDongMocThemSo", values);
    },
    async kySoNoiBoV2({commit}, values) {
        return apiClient.post(controller + "/KySoNoiBoV2", values);
    }
};
