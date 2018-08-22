<template>
  <v-layout row wrap>
      <v-flex xs12>
        <v-autocomplete
          :items="cachedSeiyuu"
          :filter="customFilter"
          v-model="selectModel"
          label="Search by Seiyuu Name..."
          item-text="name"
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
                <v-list-tile-title v-html="data.item.name"></v-list-tile-title>
              </v-list-tile-content>
            </template>
          </template>
        </v-autocomplete>
        <v-btn raised color="secondary" v-on:click="searchByName" :disabled="!selectModel || loading" :loading="loading">Add</v-btn>
      </v-flex>
  </v-layout>
</template>

<script>
import axios from 'axios'
import seiyuu from 'static/quickSeiyuulist.json'

export default {
  name: 'SeiyuuBrowser',
  props: ['searchedIdCache'],
  data () {
    return {
      searchedId: '',
      loading: false,
      cachedSeiyuu: [],
      selectModel: null,
      shareLinkData: null
    }
  },
  methods: {
    customFilter (item, queryText, itemText) {
        const nameSurname = item.name.toLowerCase()
        const surnameName = this.swapNameSurname(item.name.toLowerCase(), " ")
        const searchText = queryText.toLowerCase()

        return nameSurname.indexOf(searchText) > -1 ||
          surnameName.indexOf(searchText) > -1
    },
    searchByName () {
      this.loading = true
      if (this.searchedIdCache.indexOf(parseInt(this.selectModel)) > -1) {
        this.$emit('alreadyOnTheList')
        this.loading = false
      } else {
        axios.get(process.env.JIKAN_URL + 'person/' + String(this.selectModel))
          .then((response) => {
            this.$emit('seiyuuReturned', response.data)
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
    },
    loadCachedSeiyuu () {
      this.loadPopularList()
      this.$axios.get(process.env.API_URL + '/api/Seiyuu')
        .then((response) => {
          this.cachedSeiyuu = response.data
        })
        .catch((error) => {
          console.log(error)
          this.$axios.get(process.env.API_URL2 + '/api/Seiyuu')
            .then((response) => {
              this.cachedSeiyuu = response.data
            })
            .catch((error) => {
              console.log(error)
              this.$emit('reloadNeeded')
              this.cachedSeiyuu = []
            })
        })
    },
    sendDataFetchedEvent () {
      if (typeof this.cachedSeiyuu !== 'undefined' && this.cachedSeiyuu.length > 200) {
        this.$emit('dataFetched')
      }
    },
    loadPopularList (callback) {
      this.cachedSeiyuu = seiyuu
    },
    loadDataFromLink () {
      if (this.shareLinkData.length > 0 && this.shareLinkData.length < 6) {
        this.shareLinkData.forEach(element => {
          axios.get(process.env.JIKAN_URL + 'person/' + String(element))
            .then((response) => {
              this.$emit('seiyuuReturned', response.data)
              this.loading = false
            })
            .catch((error) => {
              console.log(error)
              if (error.response.status === 404) {
                this.$emit('apiIsDown')
              }
              this.loading = false
            })
        })
      }
      
    },
    emitEunImmediately () {
      if (this.searchedIdCache != null && this.shareLinkData != null) {
        if (this.searchedIdCache.length === this.shareLinkData.length) {
          this.$emit('runImmediately')
        }
      }
    }
  },
  created () {
    this.loadCachedSeiyuu()
  },
  mounted () {
    if (this.$route.query !== null && !this.isEmpty(this.$route.query)) {
      this.shareLinkData = this.$route.query.seiyuuIds.split(';')
      this.loadDataFromLink()
    }
  },
  watch: {
    cachedSeiyuu: {
      handler: 'sendDataFetchedEvent',
      immediate: true
    },
    searchedIdCache: {
      handler: 'emitEunImmediately',
      immediate: true
    }
  }
}
</script>

<style scoped>
 .avatar img {
   object-fit: cover;
   object-position: 100% 30%;
 }
</style>
