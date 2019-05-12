export const state = () => ({
  mainRolesOnly: false,
  tvSeriesOnly: true
})

export const getters = {
  getMainRolesOnly(state) {
    return state.mainRolesOnly;
  },
  getTvSeriesOnly(state) {
    return state.tvSeriesOnly;
  }
}

export const mutations = {
  setMainRolesOnly(state, mainRolesOnly) {
    state.mainRolesOnly = mainRolesOnly;
  },
  setTvSeriesOnly(state, TVSeriesOnly) {
    state.tvSeriesOnly = TVSeriesOnly;
  }
}