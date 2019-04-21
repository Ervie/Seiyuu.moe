<template>
  <v-container grid-list-md text-xs-left>
    <v-layout row wrap>
      <v-flex xs12 lg6 v-for="(animeCharacterPair, i) in rankingItem.animeCharacterPairs" :key="i">
        <ranking-list-panel-card 
          :animeCharacterPair="animeCharacterPair"
          />
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import RankingListPanelCard from '@/components/season/RankingListPanelCard.vue'

export default {
  name: 'RankingListPanel',
  components: {
    'ranking-list-panel-card': RankingListPanelCard
  },
  props: {
    rankingItem: {
      type: Object,
      required: true
    },
  }
}
</script>

