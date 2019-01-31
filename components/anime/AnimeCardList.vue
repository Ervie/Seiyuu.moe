<template>
<div>
  <v-layout row wrap v-show="animeToCompare.length > 0" v-if="$vuetify.breakpoint.lgAndUp">
      <v-flex v-for="(anime, index) in animeToCompare" :key="anime.mal_id" xs2 class="animeCard"  >
        <anime-card :animeData="anime" :cardId="index" :avatarHeight="280"
         @animeRemoved="removeAnime"/>
      </v-flex>
  </v-layout>
  <v-layout row wrap v-show="animeToCompare.length > 0" v-if="$vuetify.breakpoint.mdAndDown">
      <v-flex v-for="(anime, index) in animeToCompare" :key="anime.mal_id" xs4 class="animeCard">
        <anime-card :animeData="anime" :cardId="index" :avatarHeight="210"
         @animeRemoved="removeAnime"/>
      </v-flex>
  </v-layout>
  </div>
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
