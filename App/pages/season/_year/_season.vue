<template>
    <v-layout row align-center justify-center fill-height>
      <v-flex xs12 v-if="$vuetify.breakpoint.smAndUp">
        <v-card v-if="seasonSummaryData">
          <v-toolbar color="primary" class="styledHeader" dark>

            <v-toolbar-title>{{ capitalizeFirstLetter($route.params.season) }} {{ $route.params.year }} - Most roles</v-toolbar-title>

            <v-spacer></v-spacer>
              <!-- Maybe use search bar for bigger data set? -->
             <!-- <v-btn icon>
              <v-icon>search</v-icon>
            </v-btn> -->
                  <v-switch label="TV series only" v-model="tvSeriesOnly" color="error"></v-switch>
                  <v-switch label="Main roles only" v-model="mainRolesOnly" color="error"></v-switch>
          </v-toolbar>

          <v-list two-line >
            <v-list-group
              v-for="(item, index) in seasonSummaryData"
              :key="index"
              no-action
              class="season-summary-group"
            >
            <template v-slot:activator>
              <v-list-tile
                avatar
                class="season-summary-ranking"
              >
                <v-list-tile-action>
                  <v-list-tile-title class='styledHeader'> {{ (page - 1) * pageSize + index + 1}}</v-list-tile-title>
                </v-list-tile-action>
                <v-list-tile-avatar size="5em">
                  <img class="ranking-avatar" :src="pathToImage(item.seiyuu.imageUrl)"/>
                </v-list-tile-avatar>
                <v-list-tile-content class="season-summary-content">
                  <v-list-tile-title> {{ item.seiyuu.name }}</v-list-tile-title>
                  <v-list-tile-sub-title> {{item.animeCharacterPairs.length}} roles</v-list-tile-sub-title>
                </v-list-tile-content>
              </v-list-tile>
            </template>

              <v-list-tile>
                <v-list-tile-content>
                  <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
            </v-list-group>
          </v-list>
          <v-card-text>
            <div class="text-xs-center">
              <v-pagination
                v-model="page"
                :length="totalPages"
                :total-visible="7"
              ></v-pagination>
            </div>
            </v-card-text>
        </v-card>
      </v-flex>
      <loading-dialog
        :isLoading="loading"/>
    </v-layout>
</template>

<script>
import axios from 'axios';
import LoadingDialog from '@/components/shared/ui-components/LoadingDialog.vue';

export default {
    name: 'SeasonSummary',
    components: {
      'loading-dialog': LoadingDialog
    },
    data () {
      return {
        seasonSummaryData: null,
        page: 1,
        pageSize: 25,
        totalPages: 1,
        tvSeriesOnly: true,
        mainRolesOnly: false,
        loading: false
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
          '&SearchCriteria.Season=' + this.$route.params.season  +
          '&SearchCriteria.Year=' + this.$route.params.year +
          '&SearchCriteria.MainRolesOnly=' + this.mainRolesOnly +
          '&SearchCriteria.TVSeriesOnly=' + this.tvSeriesOnly)
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
    async asyncData ({ params, error }) {
      return axios.get(process.env.apiUrl + '/api/season/Summary' + 
          '?Page=0' +
          '&PageSize=25' +
          '&SearchCriteria.Season=' + params.season +
          '&SearchCriteria.Year=' + params.year)
      .then((response) => {
        return { 
          seasonSummaryData: response.data.payload.results,
          totalPages: Math.ceil(response.data.payload.totalCount / response.data.payload.pageSize)
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
      },
      tvSeriesOnly: {
        handler: 'sendNewRequest',
        immediate: false
      },
      mainRolesOnly: {
        handler: 'sendNewRequest',
        immediate: false
      }
    }
}
</script>

<style scoped>

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


