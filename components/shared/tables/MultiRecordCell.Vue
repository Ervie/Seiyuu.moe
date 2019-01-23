<template>
<v-container fluid >
    <v-layout wrap align-center justify-center fill-height>
    <div v-for="entry in items" :key="entry.mal_id" width="225px">
        <v-card
        class="tableCard">
        <a :href="entry.entry.url" target="_blank">
            <v-img
            height="195px"
            aspect-ratio="1.5"
            :src="changeUrlToOriginalSize(entry.entry.image_url)"
            ></v-img>
        </a>

        <v-card-text class="caption title">{{ swapNameSurname(decodeHtml(entry.entry.name)) }}</v-card-text>
        </v-card>
    </div>
    </v-layout>
<!-- <v-layout v-for="entry in items" :key="entry.mal_id">
    <v-card-text class="subheading">{{ swapNameSurname(decodeHtml(entry.entry.name)) }}</v-card-text>
</v-layout> -->
</v-container>
</template>

<script>
export default {
  Name: 'MultiRecordCell',
  props: {
    items: {
      type: Array,
      required: true
    }
  },
  methods: {
    changeUrlToOriginalSize (inputString) {
      return inputString.replace('/r/23x32','')
    }
  }
}
</script>

<style scoped>
    .tableCard {
        /* display: inline-flex; */
        margin: 5px;
        height: 265px;
        width: 195px;
    }
</style>
