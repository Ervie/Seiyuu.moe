<template>
  <v-layout row wrap>
      <v-flex xs6>
        <v-select
          :items="cachedSeiyuu"
          v-model="selectModel"
          label="Search by Seiyuu Name..."
          item-text="name"
          item-value="mal_id"
          max-height="auto"
          autocomplete
          prepend-icon="search"
        >
          <template slot="item" slot-scope="data">
            <template v-if="typeof data.item !== 'object'">
              <v-list-tile-content v-text="data.item"></v-list-tile-content>
            </template>
            <template v-else>
              <v-list-tile-avatar>
                <img :src="data.item.image_url">
              </v-list-tile-avatar>
              <v-list-tile-content>
                <v-list-tile-title v-html="data.item.name"></v-list-tile-title>
              </v-list-tile-content>
            </template>
          </template>
        </v-select>
          <v-btn raised color="success" v-on:click="searchByName" :disabled="!selectModel || loading" :loading="loading">Add</v-btn>
      </v-flex>
      <v-flex xs6>
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
      loader: null,
      idSearchRules:
      [
        v => !!v || 'Cannot be empty.',
        v => this.isPositiveIntegerValidation(v) || 'Must be a positive number'
      ],
      cachedSeiyuu:
      [
        {
          mal_id: 90,
          name: 'Maaya Sakamoto',
          image_url: 'https://myanimelist.cdn-dena.com/images/voiceactors/2/44284.jpg'
        },
        {
          mal_id: 99,
          name: 'Miyuki Sawashiro',
          image_url: 'https://myanimelist.cdn-dena.com/images/voiceactors/1/41394.jpg'
        },
        {
          mal_id: 8,
          name: 'Kugimiya Rie',
          image_url: 'https://myanimelist.cdn-dena.com/images/voiceactors/3/44863.jpg'
        },
        {
          mal_id: 160,
          name: 'Takehito Koyasu',
          image_url: 'https://myanimelist.cdn-dena.com/images/voiceactors/2/10264.jpg'
        }
      ],
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
      if (this.searchedIdCache.indexOf(parseInt(this.searchedId)) > -1) {
        this.$emit('alreadyOnTheList')
      } else {
        axios.get('https://api.jikan.me/person/' + this.searchedId)
          .then((response) => {
            this.$emit('seiyuuReturned', response.data)
          })
          .catch((error) => {
            console.log(error)
          })
      }
    },
    searchByName () {
      this.loader = 'loading'
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
      }
    }
  },
  watch: {
    loader () {
      const l = this.loader
      this[l] = !this[l]
      setTimeout(() => (this[l] = false), 1000)
      this.loader = null
    }
  }
}
</script>
