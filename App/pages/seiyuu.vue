<template>
  <v-container grid-list-xs text-xs-center>
    <browser 
      @seiyuuReturned="addSeiyuu"
      @runImmediately="runImmediately = true"
      :searchedId="searchedId"/>
    <seiyuu-card-list 
      :seiyuuToCompare="seiyuuToCompare" 
      @seiyuuRemoved="removeSeiyuu"/>
    <seiyuu-result-area 
      :seiyuuIds="searchedId" 
      @resetList="resetList" 
      :runImmediately="runImmediately"/>
  </v-container>
</template>

<script>
import SeiyuuBrowser from '@/components/seiyuu/SeiyuuBrowser.vue'
import SeiyuuCardList from '@/components/seiyuu/SeiyuuCardList.vue'
import SeiyuuResultArea from '@/components/seiyuu/SeiyuuResultArea.vue'

export default {
  name: 'SeiyuuArea',
  components: {
    'browser': SeiyuuBrowser,
    'seiyuu-card-list': SeiyuuCardList,
    'seiyuu-result-area': SeiyuuResultArea
  },
  data () {
    return {
      seiyuuToCompare: [],
      maximumSeiyuuNumber: 6,
      runImmediately: false
    }
  },
  computed: {
    searchedId () {
      if (this.seiyuuToCompare.length > 0) {
        return this.seiyuuToCompare.map(seiyuu => seiyuu.malId)
      } else {
        return []
      }
    }
  },
  methods: {
    addSeiyuu (seiyuuData) {
      this.seiyuuToCompare.push(seiyuuData)
    },
    removeSeiyuu (seiyuuId) {
      this.seiyuuToCompare.splice(seiyuuId, 1)
    },
    resetList () {
      this.seiyuuToCompare = []
    }
  },
  head () {
    return {
      title: "Seiyuu.moe - Seiyuu comparison",
      meta: [
        { hid: 'description', name: 'description', content: 'Select up to 6 seiyuu (voice actors) and compare them. This site will display all the anime they have been working on together.' }
      ]
    }
  }
}
</script>

<style scoped>

.seiyuuCard {
    border: 1px dashed;
    border-color:grey;
}
</style>
