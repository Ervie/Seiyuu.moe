<template>
    <v-container fluid grid-list-xs class="recordCell">
        <v-layout v-for="entry in items" :key="entry.malId" row>
            <v-flex xs8 align-content-center>
                <v-card-text class="subheading">{{ decodeHtml(getCaption(entry)) }}</v-card-text>
            </v-flex>
            <v-flex xs4>
            <a :href="entry.url" target="_blank">
                <v-img
                height="70px"
                :src="pathToImage(entry.imageUrl)"
                contain
                ></v-img>
            </a>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {
  Name: 'AvatarRecordCell',
  props: {
    items: {
      type: Array,
      required: true
    },
    tableType: {
      type: String,
      required: true
    }
  },
  methods: {
    getCaption(entry) {
      return this.tableType === "Anime" ? entry.title : this.swapNameSurname(entry.name);
    }
  }
}
</script>

<style>
  .recordCell {
    padding: 4px;
  }
</style>
