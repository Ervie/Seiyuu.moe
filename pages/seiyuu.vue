<template>
  <v-container grid-list-md text-xs-center>
    <v-alert type="info" :value="!seiyuuExtraDataFetched">
      The extra data is loading, please be patient...
    </v-alert>
    <v-alert dismissible color="error" v-model="reloadNeeded">
      Network error occured during loading additional seiyuu list. Please consider refreshing the page.
    </v-alert>
    <browser 
      @seiyuuReturned="addSeiyuu"
      @resetList="resetList"
      @runImmediately="runImmediately = true"
      @alreadyOnTheList="alreadyOnTheList = true"
      @reloadNeeded="reloadNeeded = true"
      @dataFetched="seiyuuExtraDataFetched = true"
      @apiIsDown="dataUnobtainable = true"
      @tooManyRequests="tooManyRequests = true"
      :searchedIdCache="searchedId"/>
    <v-alert dismissible color="error" v-model="tooMuchRecords">
      You can choose {{ maximumSeiyuuNumber }} seiyuu at max.
    </v-alert>
    <v-alert dismissible color="error" v-model="alreadyOnTheList">
      This seiyuu is already selected.
    </v-alert>
    <v-alert dismissible color="error" v-model="tooManyRequests">
      The Jikan API has too many requests to send. Wait a little and try again.
    </v-alert>
    <v-alert dismissible color="error" v-model="dataUnobtainable">
      This data is currently not obtainable :(
    </v-alert>
    <seiyuu-card-list 
      :seiyuuToCompare="seiyuuToCompare" 
      :maximumSeiyuuNumber="maximumSeiyuuNumber" 
      @seiyuuRemoved="removeSeiyuu"/>
    <result-area 
      :inputData="seiyuuToCompare" 
      @resetList="resetList" 
      :runImmediately="runImmediately"/>
  </v-container>
</template>

<script>
import SeiyuuBrowser from '@/components/seiyuu/SeiyuuBrowser.vue'
import SeiyuuCardList from '@/components/seiyuu/SeiyuuCardList.vue'
import ResultArea from '@/components/seiyuu/ResultArea.vue'

export default {
  name: 'SeiyuuArea',
  components: {
    'browser': SeiyuuBrowser,
    'seiyuu-card-list': SeiyuuCardList,
    'result-area': ResultArea
  },
  data () {
    return {
      seiyuuToCompare: [],
      maximumSeiyuuNumber: 6,
      tooMuchRecords: false,
      tooManyRequests: false,
      alreadyOnTheList: false,
      reloadNeeded: false,
      dataUnobtainable: false,
      seiyuuExtraDataFetched: false,
      runImmediately: false
    }
  },
  computed: {
    searchedId () {
      return this.seiyuuToCompare.map(seiyuu => seiyuu.mal_id)
    }
  },
  methods: {
    addSeiyuu (seiyuuData) {
      if (this.seiyuuToCompare.length >= this.maximumSeiyuuNumber) {
        this.tooMuchRecords = true
      } else {
        this.tooMuchRecords = false
        this.tooManyRequests = false
        this.seiyuuToCompare.push(seiyuuData)
      }
    },
    removeSeiyuu (seiyuuId) {
      this.tooMuchRecords = false
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
