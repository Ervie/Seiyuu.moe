<template>
  <v-layout row wrap v-show="animeToCompare.length > 0">
      <v-flex v-for="(anime, index) in animeToCompare" :key="anime.mal_id" lg2 xs4 class="animeCard"  >
        <anime-card :animeData="anime" :cardId="index" :avatarHeight="280"
         @animeRemoved="removeAnime"/>
      </v-flex>
  </v-layout>
</template>

<script>
import AnimeCard from '@/components/anime/AnimeCard.vue'

export default {
  name: 'AnimeCardList',
  props: {
    animeToCompare: {
      type: Array,
      required: false
    },
    maximumAnimeNumber: {
      type: Number,
      required: true
    }
  },
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
