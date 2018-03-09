<template>
  <v-container grid-list-md text-xs-center>
    <browser @seiyuuReturned="addSeiyuu" />
    <v-layout row wrap>
      <v-text-field v-model="returnedData" disabled></v-text-field>
    </v-layout>
  </v-container>
</template>

<script>
import SeiyuuBrowser from '@/components/SeiyuuBrowser.vue'

export default {
  name: 'MainContent',
  components: {
    'browser': SeiyuuBrowser
  },
  data () {
    return {
      returnedData: []
    }
  },
  methods: {
    addSeiyuu (seiyuuName) {
      this.returnedData = seiyuuName
    }
  }
}
</script>

<style scoped>
</style>
