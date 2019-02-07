<template>
    <v-timeline dense>
    <v-timeline-item
      v-for="entry in items"
      :key="entry.mal_id"
      color="accent"
      large
    >
      <span slot="opposite">Tus eu perfecto</span>
      <v-card class="elevation-2">
        <v-card-title class="headline"> {{ entry.title }} ( {{ entry.airedDate}} )</v-card-title>
        <v-image> {{ entry.imageUrl }}</v-image>
        <v-card-text>
          Lorem ipsum dolor sit amet, no nam oblique veritus. Commune scaevola imperdiet nec ut, sed euismod convenire principes at. Est et nobis iisque percipit, an vim zril disputando voluptatibus, vix an salutandi sententiae.
        </v-card-text>
      </v-card>
    </v-timeline-item>
  </v-timeline>
</template>

<script>
export default {
    name: 'SeiyuuTimeline',
    props: {
        items: {
            type: Array,
            required: false
        },
    }
}
</script>

