<template>
  <v-container grid-list-md text-xs-center>
    <browser @animeReturned="addAnime" />
    <v-alert dismissible color="error" v-model="tooMuchRecords">
      You can choose {{ maximumAnimeNumber }} anime at max.
    </v-alert>
    <v-alert dismissible color="error" v-model="alreadyOnTheList">
      This anime is already selected.
    </v-alert>
  </v-container>
</template>

<script>
import AnimeBrowser from '@/components/anime/AnimeBrowser.vue'

export default {
  name: 'AnimeArea',
  components: {
    'browser': AnimeBrowser
  },
  data () {
    return {
      animeToCompare: [],
      maximumAnimeNumber: 6,
      tooMuchRecords: false,
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
    }
  }
}
</script>

<style scoped>

</style>
