<template>
  <v-layout row wrap>
      <v-flex xs12>
        <v-text-field
          prepend-icon="search"
          name="searchIdBox"
          label="Search title"
          hint="Search anime by title"
          single-line
          v-on:keyup.enter="search"
          v-model="searchQuery"/>
          <v-btn raised color="secondary" v-on:click="search" :disabled="loadingSearch || searchQuery === ''" :loading="loadingSearch">Search</v-btn>
      </v-flex>
      <v-dialog v-model="showChoiceDialog" max-width="700">
        <v-layout row wrap v-show="searchResults.length > 0" hidden-sm-and-down>
            <v-card v-for="(result) in searchResults" :key="result.mal_id" xs4 max-width="200">
              <v-card-text style="font-weight: bold" v-on:click="selectSearchResult(result.mal_id)"> {{ result.title }}</v-card-text>
              <v-card-media :src="pathToImage(result.image_url)" :height="140" v-on:click="selectSearchResult(result.mal_id)" hidden-sm-and-down></v-card-media>
            </v-card>
        </v-layout>
      </v-dialog>
  </v-layout>
</template>

<script>
import axios from 'axios'

export default {
  name: 'AnimeBrowser',
  data () {
    return {
      searchQuery: '',
      searchResults: [],
      loadingSearch: false,
      showChoiceDialog: false
    }
  },
  methods: {
    search () {
      this.loadingSearch = true
      axios.get(process.env.JIKAN_URL + 'search/anime/' + String(this.searchQuery.replace('/', ' ')))
        .then((response) => {
          this.selectEntryFromSearchResults(response.data.result)
        })
        .catch((error) => {
          console.log(error)
        })
        .finally(() => {
          this.loadingSearch = false
          this.searchQuery = ''
        })
    },
    sendAnimeRequest (malId) {
      this.loadingSearch = true
      axios.get(process.env.JIKAN_URL + 'anime/' + String(malId) + '/characters_staff')
        .then((response) => {
          this.$emit('animeReturned', response.data)
        })
        .catch((error) => {
          console.log(error)
        })
        .finally(() => {
          this.loadingSearch = false
        })
    },
    selectEntryFromSearchResults (results) {
      if (results.length === 1 ||
      results[0].title.toLowerCase() === this.searchQuery.toLowerCase() ||
      (results[0].title.toLowerCase().includes(this.searchQuery.toLowerCase()) && !results[1].title.toLowerCase().includes(this.searchQuery.toLowerCase()))) {
        this.sendAnimeRequest(results[0].mal_id)
      } else {
        this.searchResults = results
        this.showChoiceDialog = true
      }
    },
    selectSearchResult (key) {
      this.sendAnimeRequest(key)
      this.showChoiceDialog = false
    },
    pathToImage (path) {
      if (path) {
        return path
      } else {
        return 'static/questionMark.png'
      }
    }
  }
}
</script>

<style>

</style>
