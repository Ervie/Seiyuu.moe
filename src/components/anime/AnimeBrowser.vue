<template>
  <v-layout row wrap>
      <v-flex xs12>
        <v-text-field
          prepend-icon="search"
          name="searchIdBox"
          label="Search anime by Title..."
          hint="Search anime by title"
          single-line
          v-on:keyup.enter="search"
          v-model="searchQuery"/>
          <v-btn raised color="secondary" v-on:click="search" :disabled="loadingSearch || searchQuery === ''" :loading="loadingSearch">Search</v-btn>
      </v-flex>
      <v-dialog v-model="showChoiceDialog" max-width="700">
        <v-layout row wrap v-show="searchResults.length > 0" hidden-sm-and-down>
          <v-flex  v-for="(result) in searchResults" :key="result.mal_id" xs3>
            <v-card max-width="200">
              <v-card-text style="font-weight: bold" v-on:click="selectSearchResult(result.mal_id)"> {{ truncateLongString(result.title) }}</v-card-text>
              <v-card-media :src="pathToImage(result.image_url)" :height="175" v-on:click="selectSearchResult(result.mal_id)" hidden-sm-and-down></v-card-media>
            </v-card>
            </v-flex>
        </v-layout>
      </v-dialog>
      <v-alert dismissible color="error" v-model="noResultfound">
        No result found!
      </v-alert>
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
      showChoiceDialog: false,
      noResultfound: false,
      longStringTreshold: 25
    }
  },
  methods: {
    search () {
      this.loadingSearch = true
      axios.get(process.env.JIKAN_URL + 'search/anime/' + String(this.searchQuery.replace('/', ' ')))
        .then((response) => {
          if (response.data.result === null || response.data.result.length < 1 || response.data.result[0] === null) {
            this.noResultfound = true
          } else {
            this.selectEntryFromSearchResults(response.data.result)
          }
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
      var lowerCaseQuery = this.searchQuery.toLowerCase()
      console.log(results)
      if (results.length === 1 ||
      results[0].title.toLowerCase() === lowerCaseQuery ||
      (results[0].title.toLowerCase().includes(lowerCaseQuery) && !results[1].title.toLowerCase().includes(lowerCaseQuery))) {
        this.sendAnimeRequest(results[0].mal_id)
      } else {
        var displayedResults = []
        for (let i = 0; i < results.length; i++) {
          if (results[i].title.toLowerCase().includes(lowerCaseQuery)) {
            displayedResults.push(results[i])
          } else {
            break
          }
        }

        if (displayedResults.length > 0) {
          this.searchResults = displayedResults
          this.showChoiceDialog = true
          this.noResultfound = false
        } else {
          this.noResultfound = true
        }
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
    },
    truncateLongString (inputString) {
      if (inputString.length > this.longStringTreshold) {
        return inputString.substring(0, this.longStringTreshold) + '...'
      } else {
        return inputString
      }
    }
  }
}
</script>

<style>

</style>
