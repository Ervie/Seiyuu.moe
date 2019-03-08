<template>
  <v-container grid-list-md text-xs-center>
    <anime-browser 
      @animeReturned="addAnime"
      :searchedId="searchedId"
      @runImmediately="runImmediately = true" />
    <anime-card-list 
      :animeToCompare="animeModels" 
      :maximumAnimeNumber="maximumAnimeNumber" 
      @animeRemoved="removeAnime"/>
    <anime-result-area 
      :charactersData="charactersRosters" 
      :animeData="animeModels"
      :runImmediately="runImmediately"
      @resetList="resetList"/>
  </v-container>
</template>

<script>
import AnimeBrowser from '@/components/anime/AnimeBrowser.vue'
import AnimeCardList from '@/components/anime/AnimeCardList.vue'
import AnimeResultArea from '@/components/anime/AnimeResultArea.vue'

export default {
  name: 'AnimeArea',
  components: {
    'anime-browser': AnimeBrowser,
    'anime-card-list': AnimeCardList,
    'anime-result-area': AnimeResultArea
  },
  data () {
    return {
      animeToCompare: [],
      maximumAnimeNumber: 6,
      runImmediately: false
    }
  },
  computed: {
    searchedId () {
      if (this.animeToCompare.length > 0) {
        return this.animeToCompare.map(animeEntry => animeEntry.anime.malId)
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
        this.animeToCompare.push(animeData)
    },
    resetList () {
      this.animeToCompare = []
    },
    removeAnime (animeId) {
      this.animeToCompare.splice(animeId, 1)
    }
  }
}
</script>

<style scoped>

</style>
