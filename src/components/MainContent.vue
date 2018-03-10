<template>
  <v-container grid-list-md text-xs-center>
    <browser @seiyuuReturned="addSeiyuu" />
    <v-alert dismissible color="error" v-model="tooMuchRecords">
      You can choose {{ maximumSeiyuuNumber }} seiyuu at max.
    </v-alert>
    <v-layout row justify-space-around>
      <v-flex v-for="(seiyuu, index) in seiyuuToCompare" :key="seiyuu.mal_id" xs2>
        <seiyuuCard :seiyuuName="seiyuu.name" :photoUrl="seiyuu.image_url" :seiyuuMalId="seiyuu.mal_id" :cardId="index"
         @seiyuuRemoved="removeSeiyuu"/>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import SeiyuuBrowser from '@/components/SeiyuuBrowser.vue'
import SeiyuuCard from '@/components/SeiyuuCard.vue'

export default {
  name: 'MainContent',
  components: {
    'browser': SeiyuuBrowser,
    'seiyuuCard': SeiyuuCard
  },
  data () {
    return {
      seiyuuToCompare: [],
      maximumSeiyuuNumber: 2,
      tooMuchRecords: false
    }
  },
  methods: {
    addSeiyuu (seiyuuData) {
      if (this.seiyuuToCompare.length >= this.maximumSeiyuuNumber) {
        this.tooMuchRecords = true
      } else {
        this.tooMuchRecords = false
        this.seiyuuToCompare.push(seiyuuData)
      }
    },
    removeSeiyuu (seiyuuId) {
      this.tooMuchRecords = false
      this.seiyuuToCompare.splice(seiyuuId, 1)
    }
  }
}
</script>

<style scoped>
</style>
