<template>
    <v-container>
        <v-layout row v-for="entry in items" :key="entry.mal_id">
            <v-card-text class="title textRow" > * {{ swapNameSurname(decodeHtml(entry.entry.name))}}</v-card-text>
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
    }
  }
}
</script>

<style scoped>
    .textRow {
        padding: 4px;
    }
</style>

