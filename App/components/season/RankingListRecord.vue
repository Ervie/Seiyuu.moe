<template>
  <v-list-tile
    avatar
    class="season-summary-ranking"
  >
    <v-list-tile-action v-if="!isMobile">
      <v-list-tile-title v-bind:class="{'styledHeader': !isMobile}">{{ rankingPosition }}</v-list-tile-title>
    </v-list-tile-action>
    <v-list-tile-avatar :size="isMobile ? '3em' : '5em'">
      <img class="ranking-avatar" :src="pathToImage(rankingItem.seiyuu.imageUrl)"/>
    </v-list-tile-avatar>
    <v-list-tile-content v-bind:class="{'season-summary-content': !isMobile}">
      <v-list-tile-title> {{ tileTitle }} </v-list-tile-title>
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
    },
    isMobile: {
      type: Boolean,
      required: true
    }
  },
  computed: {
    tileTitle() {
      return this.isMobile
        ? this.rankingPosition + '. ' + this.rankingItem.seiyuu.name
        : this.rankingItem.seiyuu.name;
    }
  },
}
</script>
