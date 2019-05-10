import Vuex from 'vuex';

const createStore = () => {
  return new Vuex.Store({
    state: {
      seasonSummaryMainRolesOnly: false,
      seasonSummaryTVSeriesOnly: true
    },
    getters: {
      getSeasonSummaryMainRolesOnly(state) {
        return state.seasonSummaryMainRolesOnly;
      },
      getSeasonSummaryTVSeriesOnly(state) {
        return state.seasonSummaryTVSeriesOnly;
      }
    },
    mutations: {
      setSeasonSummaryMainRolesOnly(state, mainRolesOnly) {
        state.seasonSummaryMainRolesOnly = mainRolesOnly;
      },
      setSeasonSummaryTVSeriesOnly(state, TVSeriesOnly) {
        state.seasonSummaryTVSeriesOnly = TVSeriesOnly;
      }
    },
    actions: {
      setSeasonSummaryMainRolesOnly(vuexContext, payload) {
        vuexContext.commit('setSeasonSummaryMainRolesOnly', payload);
      },
      setSeasonSummaryTVSeriesOnly(vuexContext, payload) {
        vuexContext.commit('setSeasonSummaryTVSeriesOnly', payload);
      }
    }
  });
}

export default createStore;