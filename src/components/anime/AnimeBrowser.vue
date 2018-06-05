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
          <v-btn raised color="secondary" v-on:click="search" :disabled="loadingSearch" :loading="loadingSearch">Search</v-btn>
      </v-flex>
      <ul>
      <li class="accent" v-for="(searchResult,i) in searchResults" v-bind:key="'Result' + i">
          {{ searchResult.title }}
      </li>
      </ul>
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
      loadingSearch: false
    }
  },
  methods: {
    search () {
      this.loadingSearch = true
      axios.get(process.env.JIKAN_URL + 'search/anime/' + String(this.searchQuery.replace('/', ' ')))
        .then((response) => {
          if (response.data.result.length === 1 ||
          response.data.result[0].title.toLowerCase() === this.searchQuery.toLowerCase() ||
          (response.data.result[0].title.toLowerCase().includes(this.searchQuery.toLowerCase()) && !response.data.result[1].title.toLowerCase().includes(this.searchQuery.toLowerCase()))) {
            this.sendAnimeRequest(response.data.result[0].mal_id)
          } else {
            this.searchResults = response.data.result
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
    }
  }
}
</script>

<style>

</style>
