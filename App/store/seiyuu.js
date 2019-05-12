export const state = () => ({
  mainRolesOnly: false,
  groupBySeries: true
})

export const getters = {
  getMainRolesOnly(state) {
    return state.mainRolesOnly;
  },
  getGroupBySeries(state) {
    return state.groupBySeries;
  }
}

export const mutations = {
  setMainRolesOnly(state, mainRolesOnly) {
    state.mainRolesOnly = mainRolesOnly;
  },
  setGroupBySeries(state, groupBySeries) {
    state.groupBySeries = groupBySeries;
  }
}