<template>
  <v-container grid-list-md text-xs-center>
    <anime-browser 
      @animeReturned="addAnime"
      :searchedId="searchedId"
      @runImmediately="runImmediately = true" />
    <anime-card-list 
      :animeToCompare="animeToCompare" 
      :maximumAnimeNumber="maximumAnimeNumber" 
      @animeRemoved="removeAnime"/>
    <anime-result-area 
      :animeIds="searchedId"
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
        return this.animeToCompare.map(animeEntry => animeEntry.malId)
      } else {
        return [];
      }
    }
  },
  methods: {
    addAnime (animeData) {
        this.animeToCompare.push(animeData);
    },
    resetList () {
      this.animeToCompare = [];
    },
    removeAnime (animeId) {
      this.animeToCompare.splice(animeId, 1);
    }
  },
  head () {
    return {
      title: "Seiyuu.moe - Anime comparison",
      meta: [
        { hid: 'description', name: 'description', content: 'Select up to 6 anime and compare them. This site will find and list all voice actors who have had the opportunity to work on the series you selected.' }
      ]
    }
  }
}
</script>

<style scoped>

</style>
