export const state = () => ({
  seasonSummaryMainRolesOnly: false,
  seasonSummaryTVSeriesOnly: true
})

export const getters = {
  getSeasonSummaryMainRolesOnly(state) {
    return state.seasonSummaryMainRolesOnly;
  },
  getSeasonSummaryTVSeriesOnly(state) {
    return state.seasonSummaryTVSeriesOnly;
  }
}

export const mutations = {
  setSeasonSummaryMainRolesOnly(state, mainRolesOnly) {
    state.seasonSummaryMainRolesOnly = mainRolesOnly;
  },
  setSeasonSummaryTVSeriesOnly(state, TVSeriesOnly) {
    state.seasonSummaryTVSeriesOnly = TVSeriesOnly;
  }
}