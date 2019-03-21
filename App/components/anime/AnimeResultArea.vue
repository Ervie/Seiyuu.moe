<template>
    <v-layout>
      <v-flex>
        <div>
          <v-btn raised large color="error" class="optionButton" v-on:click="resetList" :disabled="animeIds.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" class="optionButton" v-on:click="computeResults" :disabled="animeIds.length < 2">Compare</v-btn>
          <v-btn depressed large color="secondary" class="optionButton" v-on:click="generateShareLink" :disabled="!showTables || animeIds.length < 2">Share Link</v-btn>
        </div>
        <div>
          <anime-table-selection 
            v-if="showTables" 
            :tableData="outputData" 
            :counter="counter"/>
        </div>
      </v-flex>
      <share-link-snackbar
        :showSnackbar="snackbar"
        @hideSnackBar="snackbar = false"/>
    </v-layout>
</template>

<script>
import axios from 'axios'
import AnimeTableSelection from '@/components/anime/AnimeTableSelection.vue'
import ShareLinkSnackbar from '@/components/shared/ui-components/ShareLinkSnackbar.vue';

export default {
  name: 'AnimeResultArea',
  components: {
    'anime-table-selection': AnimeTableSelection,
    'share-link-snackbar': ShareLinkSnackbar
  },
  props: {
    charactersData: {
      type: Array,
      required: false
    },
    animeIds: {
      type: Array,
      required: false
    },
    runImmediately: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      showTables: false,
      counter: 0,
      outputData: [],
      snackbar: false
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    computeResults () {
      this.outputData = []

      axios.get(this.getAnimeCompareRequest())
        .then((response) => {
          if (response.data.payload !== null) {
            this.outputData = response.data.payload;
            this.showTables = true;
          }
        })
        .catch((error) => {
          this.handleBrowsingError('tooManyRequests')
        })
    },
    getAnimeCompareRequest() {
      var animeIdPart = '';
      
      this.animeIds.forEach(element => {
        animeIdPart += '&SearchCriteria.AnimeMalId=' + element;
      });
      

      return process.env.apiUrl +
        '/api/anime/Compare' +
        '?Page=0&PageSize=1000&SortExpression=Popularity DESC' +
        animeIdPart;
    },
    generateShareLink () {
      var idString = ''
      this.animeIds.forEach(element => {
        idString += element + ';'
      });
      
      idString = idString.slice(0, -1)
      idString = this.encodeURL(idString)

      var shareLink = process.env.baseUrl + $nuxt.$route.path.toLowerCase() + '?animeIds=' + idString

      this.$copyText(shareLink)
      this.snackbar = true
    }
  },
  watch: {
    animeIds: function (newVal, oldVal) {
      if (this.animeIds.length === 0) {
        this.showTables = false
      }
    },
    runImmediately: function (val) {
      if (val === true) {
        this.computeResults()
      }
    }
  }
}
</script>

<style>
img.miniav {
    max-height: 98px;
    max-width: 63px;
    width: auto;
    height: auto;
}

img.av {
    max-height: 140px;
    max-width: 90px;
    width: auto;
    height: auto;
}
</style>
