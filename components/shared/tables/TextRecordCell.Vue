<template>
    <v-container>
        <v-layout row v-for="entry in items" :key="entry.mal_id">
            <div class="title" >{{ swapNameSurname(decodeHtml(entry.entry.name))}}</div>
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

