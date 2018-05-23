<template>
  <v-layout row wrap>
      <v-flex xs12>
        <v-text-field
          prepend-icon="search"
          name="searchIdBox"
          label="Search title"
          hint="Search anime by title"
          single-line
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
      axios.get(process.env.JIKAN_URL + 'search/anime/' + String(this.searchQuery))
        .then((response) => {
          this.searchResults = response.data.result
        })
        .catch((error) => {
          console.log(error)
        })
        .finally(() => {
          this.loading = false
        })
    }
  }
}
</script>

<style>

</style>
