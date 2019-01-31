<template>
  <v-container grid-list-md text-xs-center>
    <v-alert type="info" :value="!seiyuuExtraDataFetched">
      The extra data is loading, please be patient...
    </v-alert>
    <browser 
      @seiyuuReturned="addSeiyuu"
      @runImmediately="runImmediately = true"
      @dataFetched="seiyuuExtraDataFetched = true"
      :searchedId="searchedId"/>
    <seiyuu-card-list 
      :seiyuuToCompare="seiyuuToCompare" 
      @seiyuuRemoved="removeSeiyuu"/>
    <seiyuu-result-area 
      :inputData="seiyuuToCompare" 
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
      seiyuuExtraDataFetched: false,
      runImmediately: false
    }
  },
  computed: {
    searchedId () {
      if (this.seiyuuToCompare.length > 0) {
        return this.seiyuuToCompare.map(seiyuu => seiyuu.mal_id)
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
  }
}
</script>

<style scoped>

.seiyuuCard {
    border: 1px dashed;
    border-color:grey;
}
</style>
