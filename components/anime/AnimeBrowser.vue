<template>
  <v-layout row wrap>
    <v-flex xs12>
         <v-autocomplete
          :items="items"
          :search-input.sync="search"
          v-model="model"
          hide-no-data
          no-filter
          label="Search anime by title..."
          item-text="title"
          item-value="mal_id"
          max-height="300"
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
                <v-list-tile-title v-html="data.item.title"></v-list-tile-title>
              </v-list-tile-content>
            </template>
          </template>
        </v-autocomplete>
      <v-btn raised color="secondary" v-on:click="sendAnimeRequest" v-if="loadingSearch" :disabled="loadingSearch || !model" :loading="loadingSearch">Add</v-btn>
    </v-flex>
  </v-layout>
</template>

<script>
import axios from 'axios'

export default {
  name: 'AnimeBrowser',
  props: ['searchedIdCache'],
  data () {
    return {
      entries: [],
      model: null,
      search: null,
      searchResults: [],
      loadingSearch: false,
      timeout: null,
      timeoutLimit: 500
    }
  },
  computed: {
    items () {
      return this.entries.map(entry => {
        return Object.assign({}, entry, { mal_id: entry.mal_id, title: entry.title, image_url: entry.image_url })
      })
    }
  },
  methods: {
    sendAnimeRequest () {
      this.loadingSearch = true
      axios.get(process.env.JIKAN_URL + 'anime/' + String(this.model) + '/characters_staff')
        .then((response) => {
          this.$emit('animeReturned', response.data)
          this.resetSearch()
        })
        .catch((error) => {
          console.log(error)
          this.resetSearch()
        })
    },
    resetSearch() {
      this.loadingSearch = false
      this.model = null
      this.search = ''
    },
    loadDataFromLink () {
      if (this.shareLinkData.length > 1 && this.shareLinkData.length < 6) {
        this.shareLinkData.forEach(element => {
          if (this.searchedIdCache.indexOf(element) === -1  && Number.parseInt(element) !== 'NaN' && Number.parseInt(element) > 0) {
            axios.get(process.env.JIKAN_URL + 'anime/' + String(element) + '/characters_staff')
              .then((response) => {
                this.$emit('animeReturned', response.data)
                this.loading = false
              })
              .catch((error) => {
                console.log(error)
                if (error.response.status === 404) {
                  this.$emit('apiIsDown')
                }
                this.loading = false
              })
          }
        })
      }
      
    },
    emitRunImmediately () {
      if (this.searchedIdCache != null && this.shareLinkData != null) {
        if (this.searchedIdCache.length === this.shareLinkData.length) {
          this.$emit('runImmediately')
        }
      }
    }
  },
  watch: {
    search (val) {
        clearTimeout(this.timeout)
        var self = this
        this.timeout = setTimeout(function() {
          self.isLoading = true

          if (self.model != null || val === '' || val === null || val.length < 3) {
            self.entries = []
            return
          }

          axios.get(process.env.JIKAN_URL + 'search/anime/' +  String(val.replace('/', ' ')))
            .then(res => {
              if (res.data.result.length > 0) {
                self.entries = res.data.result
              }
              self.isLoading = false
            })
            .catch(err => {
              console.log(err)
              self.isLoading = false
            })
        }, this.timeoutLimit)
    },
    model (val) {
      if (val !== null)
        this.sendAnimeRequest()
    },
    searchedIdCache: {
      handler: 'emitRunImmediately',
      immediate: true
    }
  },
  mounted () {
    if (this.$route.query !== null && !this.isEmpty(this.$route.query)) {
      if (this.$route.query.animeIds != null) {
        this.shareLinkData = this.$route.query.animeIds.split(';')
        this.loadDataFromLink()
      }
    }
  },
}
</script>
