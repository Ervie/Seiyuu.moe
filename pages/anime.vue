<template>
  <v-container grid-list-md text-xs-center>
    <browser @animeReturned="addAnime" @noResultFound="noResultsFoundToggle" />
    <v-alert dismissible color="error" v-model="tooMuchRecords">
      You can choose {{ maximumAnimeNumber }} anime at max.
    </v-alert>
    <v-alert dismissible color="error" v-model="alreadyOnTheList">
      This anime is already selected.
    </v-alert>
    <v-alert dismissible color="error" v-model="noResultsFound">
      No results found!
    </v-alert>
    <anime-card-list :animeToCompare="animeToCompare" :maximumAnimeNumber="maximumAnimeNumber" @animeRemoved="removeAnime"/>
    <result-area :inputData="animeToCompare" @resetList="resetList"/>
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
      noResultsFound: false,
      alreadyOnTheList: false
    }
  },
  computed: {
    searchedId () {
      return this.animeToCompare.map(anime => anime.mal_id)
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
  },
  async asyncData(allParams) {
    console.log(allParams.route);
  }
}
</script>

<style scoped>

</style>
