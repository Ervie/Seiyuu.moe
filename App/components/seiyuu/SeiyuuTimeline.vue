<template>
    <v-timeline>
    <v-timeline-item
      v-for="entry in items"
      :key="entry.anime.mal_id"
      color="accent"
      large
    >
      <v-avatar slot="icon">
        <img :src="entry.anime.image_url" class="dropdownAvatar timeline-dot" >
      </v-avatar>
      <span slot="opposite"> {{ entry.anime.airedDate}} </span>
      <v-card class="elevation-2">
        <v-card-title class="headline"> {{ entry.anime.name }} </v-card-title>
        <!-- <v-img :src="pathToImage(entry.anime.image_url)" width="150px" height="250px"/> -->
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

<style>
  .timeline-dot {
    width: 144px;
    height: 144px;
  }
</style>


