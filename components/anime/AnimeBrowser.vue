<template>
  <v-layout row wrap>
    <v-flex xs12>
      <!-- <v-text-field prepend-icon="search" name="searchIdBox" label="Search anime by Title..." hint="Search anime by title" single-line
        v-on:keyup.enter="search" v-model="searchQuery" /> -->
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
export default {
  name: 'AnimeBrowser',
  data () {
    return {
      entries: [],
      model: null,
      modelTitle: "",
      search: null,
      searchResults: [],
      loadingSearch: false
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
      this.$axios.get(process.env.JIKAN_URL + 'anime/' + String(this.model) + '/characters_staff')
        .then((response) => {
          this.$emit('animeReturned', response.data)
        })
        .catch((error) => {
          console.log(error)
        })
        .finally(() => {
          this.loadingSearch = false
          this.model = null
          this.search = ''
        })
    },
  },
  watch: {
    search (val) {
        
        if (val !== this.modelTitle && !this.modelTitle === '') {
          this.model = null
        }
        console.log(this.search)
        if (this.model != null || val === '' || val === null || val.length < 3) {
          this.entries = []
          return
        }

        this.isLoading = true

        this.$axios.get(process.env.JIKAN_URL + 'search/anime/' +  String(val.replace('/', ' ')))
          .then(res => {
            if (res.data.result.length > 0) {
              this.entries = res.data.result
            }
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoading = false))
    },
    model (val) {
      if (val !== null)
        this.sendAnimeRequest()
    }
  }
}
</script>
