export const state = {
    scale: 1,
    oldScale: 1,
    isFinish: false
}

export const mutations = {
    SET_SCALE(state, newValue) {
        state.oldScale = state.scale;
        state.scale = newValue;

    },
    SET_FINISH(state, newValue) {
        state.isFinish = newValue;
    }
}

export const actions = {
    setScale({ commit }, scale ) {
        commit('SET_SCALE', scale)
    },
    setFinish({ commit }, finish ) {
        commit('SET_FINISH', finish)
    }
}