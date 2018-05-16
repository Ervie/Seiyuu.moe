<template>
  <v-layout row wrap>
      <v-flex xs12>
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
          <v-btn raised color="secondary" v-on:click="searchByName" :disabled="!selectModel || loading" :loading="loading">Add</v-btn>
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
    searchByName () {
      this.loading = true
      if (this.searchedIdCache.indexOf(parseInt(this.selectModel)) > -1) {
        this.$emit('alreadyOnTheList')
        this.loading = false
      } else {
        axios.get(process.env.JIKAN_URL + 'person/' + String(this.selectModel))
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
    loadCachedSeiyuu () {
      this.loadPopularList()
      axios.get(process.env.API_URL2 + '/api/Seiyuu')
        .then((response) => {
          this.cachedSeiyuu = response.data
        })
        .catch((error) => {
          console.log(error)
          axios.get(process.env.API_URL2 + '/api/Seiyuu')
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
    pathToImage (initialPath) {
      if (initialPath) {
        return initialPath
      } else {
        return 'static/questionMark.png'
      }
    },
    sendDataFetchedEvent () {
      if (typeof this.cachedSeiyuu !== 'undefined' && this.cachedSeiyuu.length > 0) {
        this.$emit('dataFetched')
      }
    },
    loadPopularList (callback) {
      var xhr = new XMLHttpRequest()
      var self = this
      var decodedAnser = ''
      xhr.responseType = 'arraybuffer'

      xhr.onload = function () {
        if (xhr.status === 200) {
          var decoder = new TextDecoder('utf-8')
          decodedAnser = decoder.decode(xhr.response)
          self.cachedSeiyuu = JSON.parse(decodedAnser)
        } else {
          console.log('error')
        }
      }
      xhr.open('GET', '/static/quickSeiyuuList.json', true)

      xhr.send()
    }
  },
  created () {
    this.loadCachedSeiyuu()
  },
  watch: {
    cachedSeiyuu: {
      handler: 'sendDataFetchedEvent',
      immediate: true
    }
  }
}
</script>
