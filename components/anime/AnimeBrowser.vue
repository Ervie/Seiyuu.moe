<template>
  <v-layout row wrap>
    <v-flex xs12>
      <v-text-field prepend-icon="search" name="searchIdBox" label="Search anime by Title..." hint="Search anime by title" single-line
        v-on:keyup.enter="search" v-model="searchQuery" />
      <v-btn raised color="secondary" v-on:click="search" :disabled="loadingSearch || searchQuery === ''" :loading="loadingSearch">Search</v-btn>
    </v-flex>
    <v-dialog v-model="showChoiceDialog" max-width="700">
      <v-card>
        <h1 class="white--text">Select entry:</h1>
      </v-card>
      <v-layout row wrap v-show="searchResults.length > 0">
        <v-flex v-for="(result) in searchResults" :key="result.mal_id" xs12 class="result-card" hidden-sm-and-down>
          <v-card>
            <v-container fluid grid-list-lg v-on:click="selectSearchResult(result.mal_id)">
              <v-layout row>
                <v-flex xs4 align-center>
                  <v-card-media :height="140" :src="result.image_url" contain />
                </v-flex>
                <v-flex xs8 align-center>
                  <v-card-text  class="headline">{{ result.title }}</v-card-text>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card>
        </v-flex>
        <v-flex v-for="(result) in searchResults" :key="'mobile' + result.mal_id" xs12 class="result-card" hidden-md-and-up>
          <v-card>
            <v-flex xs12 align-center v-on:click="selectSearchResult(result.mal_id)">
              <v-card-text class="subheading">{{ result.title }}</v-card-text>
            </v-flex>
          </v-card>
        </v-flex>
      </v-layout>
    </v-dialog>
  </v-layout>
</template>

<script>
export default {
  name: 'AnimeBrowser',
  data () {
    return {
      searchQuery: '',
      searchResults: [],
      loadingSearch: false,
      showChoiceDialog: false,
      longStringTreshold: 25
    }
  },
  methods: {
    search () {
      this.loadingSearch = true
      this.$axios.get(process.env.JIKAN_URL + 'search/anime/' + String(this.searchQuery.replace('/', ' ')))
        .then((response) => {
          if (response.data.result === null || response.data.result.length < 1 || response.data.result[0] === null) {
            this.$emit('noResultFound', true)
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
      this.$axios.get(process.env.JIKAN_URL + 'anime/' + String(malId) + '/characters_staff')
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
          this.$emit('noResultFound', false)
        } else if (results.length > 0) {
          this.searchResults = results
          this.showChoiceDialog = true
          this.$emit('noResultFound', false)
        } else {
          this.$emit('noResultFound', true)
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
        return 'questionMark.png'
      }
    }
  }
}
</script>

<style>

.result-card {
  margin: 1px;
}

.result-card:hover {
  border: 3px;
  border-color: #26a69a;
  border-style: solid;
}
</style>
