<template>
  <v-layout row align-center justify-center fill-height>
    <no-ssr>
      <v-flex xs12>
        <v-card v-if="seasonSummaryData">
          <v-toolbar color="primary" class="styledHeader"
            :prominent="isMobile"
            :extended="isMobile"
            dark>
            <v-toolbar-title>{{ capitalizeFirstLetter($route.params.season) }} {{ $route.params.year }} - Most roles</v-toolbar-title>
            <template v-if="isMobile" v-slot:extension>
              <v-switch label="TV series only" v-model="tvSeriesOnly" color="error"></v-switch>
              <v-switch label="Main roles only" v-model="mainRolesOnly" color="error"></v-switch>
            </template>
            <v-spacer v-if="!isMobile"/>
            <v-switch v-if="!isMobile" label="TV series only" v-model="tvSeriesOnly" color="error"></v-switch>
            <v-switch v-if="!isMobile" label="Main roles only" v-model="mainRolesOnly" color="error"></v-switch>
          </v-toolbar>
          <v-list two-line >
            <v-list-group
              v-for="(item, index) in seasonSummaryData"
              :key="index"
              no-action
              v-bind:class="{ 'season-summary-group': !isMobile }"
            >
            <template v-slot:activator>
              <ranking-list-record 
                :rankingItem="item"
                :rankingPosition="(page - 1) * pageSize + index + 1"
                :isMobile="isMobile"/>
            </template>
              <ranking-list-panel
                :rankingItem="item" />
            </v-list-group>
          </v-list>
          <v-card-text>
            <div class="text-xs-center">
              <v-pagination
                v-model="page"
                :length="totalPages"
                :total-visible="isMobile ? 5 : 7"
              ></v-pagination>
            </div>
          </v-card-text>
        </v-card>
        <v-layout row wrap justify-space-around>
        <link-button
          :seasonName="previousSeason.season"
          :seasonYear="previousSeason.year"
          :isNext="false"
          @loadingAnotherSeason="loadingAnotherSeason = true"
        />
        <link-button
          :seasonName="nextSeason.season"
          :seasonYear="nextSeason.year"
          :isNext="true"
          @loadingAnotherSeason="loadingAnotherSeason = true"
        />
        </v-layout>
      </v-flex>
    </no-ssr>
    <loading-dialog
      :isLoading="loading || loadingAnotherSeason"/>
  </v-layout>
</template>

<script>
import axios from 'axios';
import LoadingDialog from '@/components/shared/ui-components/LoadingDialog.vue';
import RankingListPanel from '@/components/season/RankingListPanel.vue';
import LinkButton from '@/components/season/LinkButton.vue';
import RankingListRecord from '@/components/season/RankingListRecord.vue';

export default {
  name: 'SeasonSummary',
  components: {
    'loading-dialog': LoadingDialog,
    'ranking-list-panel': RankingListPanel,
    'ranking-list-record': RankingListRecord,
    'link-button': LinkButton
  },
  data () {
    return {
      seasonSummaryData: null,
      season: 'Winter',
      year: 2019,
      page: 1,
      pageSize: 25,
      totalPages: 1,
      loading: false,
      loadingAnotherSeason: false,
      seasonItems: [
        'Winter',
        'Spring',
        'Summer',
        'Fall'
      ]
    }
  },
  computed: {
    mainRolesOnly: {
      get() {
        return this.$store.getters.getSeasonSummaryMainRolesOnly;
      },
      set(val) {
        this.$store.dispatch('setSeasonSummaryMainRolesOnly', val);
        this.sendNewRequest();
      }
    },
    tvSeriesOnly: {
      get() {
        return this.$store.getters.getSeasonSummaryTVSeriesOnly;
      },
      set(val) {
        this.$store.dispatch('setSeasonSummaryTVSeriesOnly', val);
        this.sendNewRequest();
      }
    },
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },
    previousSeason() {
      if (this.year !== null && this.season !== null) {
        var previousSeasonYear = this.capitalizeFirstLetter(this.season) === 'Winter' ? 
          this.year - 1 :
          this.year;
        var previousSeasonName = this.seasonItems[this.mod(this.seasonItems.indexOf(this.capitalizeFirstLetter(this.season)) - 1, 4)];
        return {season: previousSeasonName, year: previousSeasonYear};
      } else {
        return null;
      }
    },
    nextSeason() {
      if (this.year !== null && this.season !== null) {
        var nextSeasonYear = this.capitalizeFirstLetter(this.season) === 'Fall' ? 
          this.year + 1 :
          this.year;
        var nextSeasonName = this.seasonItems[(this.seasonItems.indexOf(this.capitalizeFirstLetter(this.season)) + 1) % 4];
        return {season: nextSeasonName, year: nextSeasonYear};
      } else {
        return null;
      }
    }
  },
  methods: {
    sendNewRequest() {
      this.page = 1;
      this.changePage();
    },
    changePage() {
      this.loading = true;
      axios.get(process.env.apiUrl + '/api/season/Summary' + 
        '?Page=' + (this.page - 1) +
        '&PageSize=' + this.pageSize +
        '&Season=' + this.$route.params.season  +
        '&Year=' + this.$route.params.year +
        '&MainRolesOnly=' + this.mainRolesOnly +
        '&TVSeriesOnly=' + this.tvSeriesOnly)
      .then((response) => {
        this.seasonSummaryData = response.data.payload.results;
        this.totalPages = Math.ceil(response.data.payload.totalCount / this.pageSize);
        this.loading = false;
      })
      .catch((e) => {
        console.log(e);
        this.loading = false;
      })
    }
  },
  async asyncData ({ error, store, params }) {
    return axios.get(process.env.apiUrl + '/api/season/Summary' + 
        '?Page=0' +
        '&PageSize=25' +
        '&Season=' + params.season +
        '&Year=' + params.year +
        '&MainRolesOnly=' + store.getters.getSeasonSummaryMainRolesOnly +
        '&TVSeriesOnly=' + store.getters.getSeasonSummaryTVSeriesOnly)
    .then((response) => {
      return { 
        seasonSummaryData: response.data.payload.results,
        totalPages: Math.ceil(response.data.payload.totalCount / response.data.payload.pageSize),
        season: params.season,
        year: Number(params.year)
      }
    })
    .catch((e) => {
      console.log(e);
      error({ statusCode: 404, message: 'Post not found' })
    })
  },
  watch: {
    page: {
      handler: 'changePage',
      immediate: false
    }
  }
}
</script>

<style>

.season-summary-group {
  border-bottom: silver 1px solid;
}

.season-summary-ranking {
  padding: 8px 0 0 0;
  height: 100px;
}

.season-summary-content {
  padding: 0 0 0 16px;
}

.ordinal-number {
  font-size: 32px;
}

.ranking-avatar {
  object-fit: cover;
  object-position: 100% 30%;
}

</style>


