<template>
  <v-container grid-list-md text-xs-center>
    <v-layout>
      <v-flex>
       <v-text-field
         prepend-icon="search"
         name="searchIdBox"
         label="Search..."
         hint="Search by Seiyuu MAL Id"
         single-line
         v-model="searchedId"/>
        <v-btn small color="primary" v-on:click="searchById">Search</v-btn>
     </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-text-field v-model="returnedData.name" disabled></v-text-field>
    </v-layout>
  </v-container>
</template>

<script>
import axios from 'axios'

export default {
  name: 'MainContent',
  data () {
    return {
      searchedId: '',
      returnedData: []
    }
  },
  methods: {
    searchById () {
      console.log('https://api.jikan.me/person/' + this.searchedId)
      axios.get('https://api.jikan.me/person/' + this.searchedId)
        .then((response) => {
          this.returnedData = response.data
        })
        .catch((error) => {
          console.log(error)
          this.returnedData = ''
        })
    }
  }
}
</script>

<style scoped>
</style>
