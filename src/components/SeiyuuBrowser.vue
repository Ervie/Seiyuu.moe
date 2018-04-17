<template>
  <v-layout row wrap>
      <v-flex xs9>
        <v-select
          :items="cachedSeiyuu"
          v-model="selectModel"
          label="Search by Seiyuu Name..."
          item-text="name"
          item-value="mal_id"
          max-height="300"
          autocomplete
          prepend-icon="search"
        >
          <template slot="item" slot-scope="data">
            <template v-if="typeof data.item !== 'object'">
              <v-list-tile-content v-text="data.item"></v-list-tile-content>
            </template>
            <template v-else>
              <v-list-tile-avatar>
                <img :src="pathToImage(data.item.image_url)">
              </v-list-tile-avatar>
              <v-list-tile-content>
                <v-list-tile-title v-html="data.item.name"></v-list-tile-title>
              </v-list-tile-content>
            </template>
          </template>
        </v-select>
          <v-btn raised color="success" v-on:click="searchByName" :disabled="!selectModel || loading" :loading="loading">Add</v-btn>
      </v-flex>
      <v-flex xs3>
        <v-text-field
          prepend-icon="search"
          name="searchIdBox"
          label="Search by MAL Id..."
          hint="Search by Seiyuu MAL Id"
          single-line
          v-model="searchedId"
          :rules="idSearchRules"/>
          <v-btn raised color="primary" v-on:click="searchById" :disabled="!isIdValid || loading" :loading="loading">Search</v-btn>
      </v-flex>
  </v-layout>
</template>

<script>
import axios from 'axios'

export default {
  name: 'SeiyuuBrowser',
  props: ['searchedIdCache'],
  data () {
    return {
      searchedId: '',
      loading: false,
      idSearchRules:
      [
        v => !!v || 'Cannot be empty.',
        v => this.isPositiveIntegerValidation(v) || 'Must be a positive number'
      ],
      cachedSeiyuu: [],
      selectModel: null
    }
  },
  computed: {
    isIdValid () {
      return this.requiredIdValidation(this.searchedId) && this.isPositiveIntegerValidation(this.searchedId)
    }
  },
  methods: {
    requiredIdValidation (value) {
      return !!value
    },
    isPositiveIntegerValidation (value) {
      var parsed = Math.floor(Number(value))
      return (parsed !== Infinity && String(parsed) === value && parsed >= 0)
    },
    searchById () {
      this.loading = true
      if (this.searchedIdCache.indexOf(parseInt(this.searchedId)) > -1) {
        this.$emit('alreadyOnTheList')
      } else {
        axios.get('https://api.jikan.me/person/' + this.searchedId)
          .then((response) => {
            this.$emit('seiyuuReturned', response.data)
            this.addSeiyuuToCache(response.data)
          })
          .catch((error) => {
            console.log(error)
          })
          .finally(() => {
            this.loading = false
          })
      }
    },
    searchByName () {
      this.loading = true
      if (this.searchedIdCache.indexOf(parseInt(this.selectModel)) > -1) {
        this.$emit('alreadyOnTheList')
      } else {
        axios.get('https://api.jikan.me/person/' + String(this.selectModel))
          .then((response) => {
            this.$emit('seiyuuReturned', response.data)
          })
          .catch((error) => {
            console.log(error)
          })
          .finally(() => {
            this.loading = false
          })
      }
    },
    addSeiyuuToCache (seiyuuData) {
      if (this.cachedSeiyuu.filter(x => (x.mal_id !== seiyuuData.mal_id))) {
        this.cachedSeiyuu.push({
          mal_id: seiyuuData.mal_id,
          name: seiyuuData.name,
          image_url: seiyuuData.image_url
        })
        // sort cachedSeiyuu
        this.cachedSeiyuu.sort(function (a, b) { return (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0) })

        axios.post(process.env.API_URL + '/api/Seiyuu', {
          mal_id: seiyuuData.mal_id,
          name: seiyuuData.name,
          image_url: seiyuuData.image_url
        })
          .then((response) => {
            console.log(response)
          })
          .catch((error) => {
            console.log(error)
            this.cachedSeiyuu = []
          })
      }
    },
    loadCachedSeiyuu () {
      axios.get(process.env.API_URL + '/api/Seiyuu')
        .then((response) => {
          this.cachedSeiyuu = response.data
        })
        .catch((error) => {
          console.log(error)
          this.cachedSeiyuu = []
        })
    },
    pathToImage (initialPath) {
      if (initialPath) {
        return initialPath
      } else {
        return 'static/questionMark.png'
      }
    }
  },
  created () {
    this.loadCachedSeiyuu()
  }
}
</script>
