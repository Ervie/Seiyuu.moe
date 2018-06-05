<template>
<div>
  <v-layout row wrap v-show="animeToCompare.length > 0" hidden-sm-and-down>
      <v-flex v-for="(anime, index) in animeToCompare" :key="anime.mal_id" xs2 class="animeCard"  >
        <anime-card :animeData="anime" :cardId="index" :avatarHeight="280"
         @animeRemoved="removeAnime"/>
      </v-flex>
      <!-- Dummy cards for anime to be filled-->
      <v-flex v-for="i in (maximumAnimeNumber - animeToCompare.length)" :key="`emptyLarge${i}`" xs2  class="animeCard">
        <anime-card :cardId="i + animeToCompare.length - 1"/>
      </v-flex>
  </v-layout>
  <v-layout row wrap v-show="animeToCompare.length > 0" hidden-md-and-up>
      <v-flex v-for="(anime, index) in animeToCompare" :key="anime.mal_id" xs4 class="animeCard">
        <anime-card :animeData="anime" :cardId="index" :avatarHeight="210"
         @animeRemoved="removeAnime"/>
      </v-flex>
      <!-- Dummy cards for anime to be filled-->
      <v-flex v-for="i in (maximumAnimeNumber - animeToCompare.length)" :key="`emptySmall${i}`" xs4 class="animeCard">
        <anime-card :cardId="i + animeToCompare.length - 1"/>
      </v-flex>
  </v-layout>
  </div>
</template>

<script>
import AnimeCard from '@/components/anime/AnimeCard.vue'

export default {
  name: 'AnimeCardList',
  props: ['animeToCompare', 'maximumAnimeNumber'],
  components: {
    'anime-card': AnimeCard
  },
  data () {
    return {
      showDialog: false
    }
  },
  methods: {
    removeAnime: function (animeId) {
      this.$emit('animeRemoved', animeId)
    }
  }
}
</script>
