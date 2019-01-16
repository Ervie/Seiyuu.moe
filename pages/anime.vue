<template>
  <v-container grid-list-md text-xs-center>
    <browser 
      @animeReturned="addAnime" 
      @noResultFound="noResultsFoundToggle"
      @tooManyRequests="tooManyRequests = true"
      :searchedIdCache="searchedId"
      @runImmediately="runImmediately = true" />
    <v-alert dismissible color="error" v-model="tooMuchRecords">
      You can choose {{ maximumAnimeNumber }} anime at max.
    </v-alert>
    <v-alert dismissible color="error" v-model="alreadyOnTheList">
      This anime is already selected.
    </v-alert>
    <v-alert dismissible color="error" v-model="tooManyRequests">
      The Jikan API has too many requests to send. Wait a little and try again.
    </v-alert>
    <v-alert dismissible color="error" v-model="noResultsFound">
      No results found!
    </v-alert>
    <anime-card-list 
      :animeToCompare="animeModels" 
      :maximumAnimeNumber="maximumAnimeNumber" 
      @animeRemoved="removeAnime"/>
    <result-area 
      :charactersData="charactersRosters" 
      :animeData="animeModels" 
      @resetList="resetList" 
      :runImmediately="runImmediately"/>
  </v-container>
</template>

<script>
import AnimeBrowser from '@/components/anime/AnimeBrowser.vue'
import AnimeCardList from '@/components/anime/AnimeCardList.vue'
import ResultArea from '@/components/anime/ResultArea.vue'

export default {
  name: 'AnimeArea',
  components: {
    'browser': AnimeBrowser,
    'anime-card-list': AnimeCardList,
    'result-area': ResultArea
  },
  data () {
    return {
      animeToCompare: [],
      maximumAnimeNumber: 6,
      tooMuchRecords: false,
      tooManyRequests: false,
      noResultsFound: false,
      alreadyOnTheList: false,
      runImmediately: false
    }
  },
  computed: {
    searchedId () {
      if (this.animeToCompare.length > 0) {
        return this.animeToCompare.map(animeEntry => animeEntry.anime.mal_id)
      } else {
        return [];
      }
    },
    animeModels () {
      return this.animeToCompare.map(animeEntry => animeEntry.anime)
    },
    charactersRosters () {
      return this.animeToCompare.map(animeEntry => animeEntry.characters)
    }
  },
  methods: {
    addAnime (animeData) {
      if (this.animeToCompare.length >= this.maximumAnimeNumber) {
        this.tooMuchRecords = true
      } else if (this.searchedId.indexOf(parseInt(animeData.mal_id)) !== -1) {
        this.alreadyOnTheList = true
      } else {
        this.tooMuchRecords = false
        this.tooManyRequests = false;
        this.alreadyOnTheList = false
        this.animeToCompare.push(animeData)
      }
    },
    removeAnime (animeId) {
      this.tooMuchRecords = false
      this.animeToCompare.splice(animeId, 1)
    },
    resetList () {
      this.animeToCompare = []
    },
    noResultsFoundToggle (status) {
      this.noResultsFound = status
    }
  }
}
</script>

<style scoped>

</style>
