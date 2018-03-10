<template>
  <v-layout>
      <v-flex>
        <v-form>
          <v-text-field
            prepend-icon="search"
            name="searchIdBox"
            label="Search..."
            hint="Search by Seiyuu MAL Id"
            single-line
            v-model="searchedId"
            :rules="[rules.required, rules.isPositiveInteger]"/>
            <v-btn raised color="primary" v-on:click="searchById" :disabled="!isIdValid">Search</v-btn>
        </v-form>
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
      rules: {
        required: (value) => this.requiredIdValidation(value) || 'Cannot be empty.',
        isPositiveInteger: (value) => this.isPositiveIntegerValidation(value) || 'Must be a positive number'
      }
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
    }
  }
}
</script>
