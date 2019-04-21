<template>
  <v-list-tile
    avatar
    class="season-summary-ranking"
  >
    <v-list-tile-action>
      <v-list-tile-title class='styledHeader'>{{ rankingPosition }}</v-list-tile-title>
    </v-list-tile-action>
    <v-list-tile-avatar size="5em">
      <img class="ranking-avatar" :src="pathToImage(rankingItem.seiyuu.imageUrl)"/>
    </v-list-tile-avatar>
    <v-list-tile-content class="season-summary-content">
      <v-list-tile-title> {{ rankingItem.seiyuu.name }}</v-list-tile-title>
      <v-list-tile-sub-title> {{rankingItem.animeCharacterPairs.length}} roles</v-list-tile-sub-title>
    </v-list-tile-content>
  </v-list-tile>
</template>

<script>
export default {
  name: 'RankingListRecord',
  props: {
    rankingItem: {
      type: Object,
      required: true 
    },
    rankingPosition: {
      type: Number,
      required: true
    }
  }
}
</script>
