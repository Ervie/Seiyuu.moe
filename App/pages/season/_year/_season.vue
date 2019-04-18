<template>
    <v-layout row>
      <v-flex xs12 v-if="$vuetify.breakpoint.smAndUp">
        <v-card v-if="seasonSummaryData">
          <v-toolbar color="primary" class="styledHeader" dark>

            <v-toolbar-title>Seiyuu with most roles in {{ $route.params.season }} {{ $route.params.year }}</v-toolbar-title>

            <v-spacer></v-spacer>

            <!--  Maybe use search bar for bigger data set?
              <v-btn icon>
              <v-icon>search</v-icon>
            </v-btn> -->
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
                  <v-list-tile-title class='styledHeader'> {{ index + 1}}</v-list-tile-title>
                </v-list-tile-action>
                <v-list-tile-avatar size="5em">
                  <img class="dropdownAvatar" :src="pathToImage(item.seiyuu.imageUrl)"/>
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
        </v-card>
      </v-flex>
    </v-layout>
</template>

<script>
import axios from 'axios';

export default {
    name: 'SeasonSummary',
    data () {
      return {
        seasonSummaryData: null
      }
    },
    async asyncData ({ params, error }) {
      return axios.get(process.env.apiUrl + '/api/season/Summary' + 
          '?Page=0&PageSize=25&SortExpression=Popularity DESC' +
          '&SearchCriteria.Season=' + params.season +
          '&SearchCriteria.Year=' + params.year)
      .then((response) => {
        return { seasonSummaryData: response.data.payload }
      })
      .catch((e) => {
        console.log(e);
        error({ statusCode: 404, message: 'Post not found' })
      })
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

</style>


