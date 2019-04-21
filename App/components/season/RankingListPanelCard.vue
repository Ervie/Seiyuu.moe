<template>
  <v-card dark>
    <v-layout>
      <v-flex xs5 sm2 >
        <v-img
          :src="animeCharacterPair.item2.imageUrl"
          height="125px"
          contain
        ></v-img>
      </v-flex>
      <v-flex xs7 sm10>
        <v-card-title primary-title>
          <div>
            <div class="headline">{{ animeCharacterPair.item2.name }}</div>
            <div>{{ animeCharacterPair.item1.title }}</div>
          </div>
        </v-card-title>
      </v-flex>
    </v-layout>
  </v-card>
</template>

<script>
export default {
  name: 'RankingListPanelCard',
  props: {
    animeCharacterPair: {
      type: Object,
      required: true
    },
  },
}
</script>

