<template>
    <v-container>
        <v-layout row v-for="entry in items" :key="entry.malId">
            <v-card-text class="title textRow" > * {{ decodeHtml(getCaption(entry)) }}</v-card-text>
        </v-layout>
    </v-container>
</template>
<script>
export default {
  name: 'TextRecordCell',
  props : {
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

<style scoped>
    .textRow {
        padding: 4px;
    }
</style>

