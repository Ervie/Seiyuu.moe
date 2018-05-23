<template>
  <v-container grid-list-md text-xs-center>
    <browser />
  </v-container>
</template>

<script>
import AnimeBrowser from '@/components/anime/AnimeBrowser.vue'

export default {
  name: 'AnimeArea',
  components: {
    'browser': AnimeBrowser
  }
}
</script>

<style scoped>

</style>
