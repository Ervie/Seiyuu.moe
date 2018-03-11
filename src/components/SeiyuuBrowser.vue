<template>
  <v-layout>
      <v-flex>
        <v-autocomplete :items="items" v-model="item" :get-label="getLabel" :component-item="suggestionTemplate" @change="updateItems">
        </v-autocomplete>
        <v-text-field
          prepend-icon="search"
          name="searchIdBox"
          label="Search..."
          hint="Search by Seiyuu MAL Id"
          single-line
          v-model="searchedId"
          :rules="idSearchRules"/>
          <v-btn raised color="primary" v-on:click="searchById" :disabled="!isIdValid">Search</v-btn>
          <v-btn raised color="error" v-on:click="clearList" :disabled="searchedIdCache.length < 1">Reset</v-btn>
      </v-flex>
  </v-layout>
</template>

<script>
import axios from 'axios'
import Suggestion from './Suggestion.vue'

export default {
  name: 'SeiyuuBrowser',
  props: ['searchedIdCache'],
  data () {
    return {
      searchedId: '',
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
          mal_id: 8,
          name: 'Takehito Koyasu',
          image_url: 'https://myanimelist.cdn-dena.com/images/voiceactors/2/10264.jpg'
        }
      ],
      items: [],
      item: { name: '', mal_id: -1 },
      suggestionTemplate: Suggestion
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
            this.returnedData = ''
          })
      }
    },
    clearList () {
      this.$emit('resetList')
    },
    getLabel (item) {
      return item.name
    },
    updateItems (text) {
      console.log(text)
      this.items = this.cachedSeiyuu.filter(function (seiyuu) {
        return seiyuu.name.toLowerCase().indexOf(text.toLowerCase()) !== -1
      })
    }
  }
}
</script>
