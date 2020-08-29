<template>
    <v-layout>
      <v-flex>
        <div>
          <v-btn raised large color="error" class="optionButton" v-on:click="resetList" :disabled="animeIds.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" class="optionButton" v-on:click="computeResults" :disabled="animeIds.length < 2 || loadingComparison" :loading="loadingComparison">Compare</v-btn>
          <v-btn depressed large color="secondary" class="optionButton" v-on:click="generateShareLink" :disabled="!showTables || animeIds.length < 2">Share Link</v-btn>
        </div>
        <div>
          <anime-table-selection 
            v-if="showTables" 
            :tableData="outputData" 
            :loadingComparison="loadingComparison"
            :counter="counter"/>
        </div>
      </v-flex>
      <share-link-snackbar
        :showSnackbar="snackbar"
        @hideSnackBar="snackbar = false"/>
      <loading-dialog
        :isLoading="loadingComparison"/>
    </v-layout>
</template>

<script>
import axios from 'axios'
import AnimeTableSelection from '@/components/anime/AnimeTableSelection.vue'
import ShareLinkSnackbar from '@/components/shared/ui-components/ShareLinkSnackbar.vue';
import LoadingDialog from '@/components/shared/ui-components/LoadingDialog.vue';

export default {
  name: 'AnimeResultArea',
  components: {
    'anime-table-selection': AnimeTableSelection,
    'share-link-snackbar': ShareLinkSnackbar,
    'loading-dialog': LoadingDialog
  },
  props: {
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
      loadingComparison: false,
      snackbar: false
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    computeResults () {
      this.outputData = [];
      this.showTables = true;
      this.loadingComparison = true;

      axios.get(this.getAnimeCompareRequest())
        .then((response) => {
          if (response.data !== null) {
            this.outputData = response.data;
            this.loadingComparison = false;
          }
        })
        .catch((error) => {
          this.loadingComparison = false;
        })
    },
    getAnimeCompareRequest() {
      var animeIdPart = '';
      
      this.animeIds.forEach(element => {
        animeIdPart += '&AnimeMalIds=' + element;
      });

      return process.env.apiUrl +
        '/api/anime/Compare?' +
        animeIdPart;
    },
    generateShareLink () {
      var idString = ''
      this.animeIds.forEach(element => {
        idString += element + ';'
      });
      
      idString = idString.slice(0, -1);
      idString = this.encodeURL(idString);

      var shareLink = process.env.baseUrl + $nuxt.$route.path.toLowerCase() + '?animeIds=' + idString;

      this.$copyText(shareLink);
      this.snackbar = true;
    }
  },
  watch: {
    animeIds: function (newVal, oldVal) {
      if (this.animeIds.length === 0) {
        this.showTables = false;
      }
    },
    runImmediately: function (val) {
      if (val === true) {
        this.computeResults();
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
