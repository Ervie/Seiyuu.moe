<template>
  <v-container fluid grid-list-xs v-if="!preferText">
    <v-layout row>
        <v-flex xs8 align-content-center>
        <v-card-text class="subheading">{{ decodeHtml(item.entry.name) }}</v-card-text>
        </v-flex>
        <v-flex xs4>
        <a :href="item.entry.url" target="_blank">
            <v-img
            height="70px"
            :src="item.entry.image_url"
            contain
            ></v-img>
        </a>
        </v-flex>
    </v-layout>
  </v-container>
    <!-- Text mode -->
    <v-layout v-else row>
        <div class="title">{{ swapNameSurname(decodeHtml(item.entry.name))}}</div>
    </v-layout>
</template>

<script>
export default {
  Name: 'SingleRecordCell',
  props: {
    item: {
      type: Object,
      required: true
    },
    preferText: {
      type: Boolean,
      required: true
    }
  }
}
</script>
