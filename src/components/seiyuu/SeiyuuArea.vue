<template>
  <v-container grid-list-md text-xs-center>
    <v-alert type="info" :value="!seiyuuDataFetched">
      The data is loading, please be patient...
    </v-alert>
    <v-alert dismissible color="error" v-model="reloadNeeded">
      Network error occured during loading seiyuu list. Please consider refresh the page.
    </v-alert>
    <browser @seiyuuReturned="addSeiyuu" @alreadyOnTheList="alreadyOnTheList = true" @reloadNeeded="reloadNeeded = true" @resetList="resetList" @dataFetched="seiyuuDataFetched = true" :searchedIdCache="searchedId"/>
    <v-alert dismissible color="error" v-model="tooMuchRecords">
      You can choose {{ maximumSeiyuuNumber }} seiyuu at max.
    </v-alert>
    <v-alert dismissible color="error" v-model="alreadyOnTheList">
      This seiyuu is already selected.
    </v-alert>
    <seiyuu-card-list :seiyuuToCompare="seiyuuToCompare" :maximumSeiyuuNumber="maximumSeiyuuNumber" @seiyuuRemoved="removeSeiyuu"/>
    <result-area :inputData="seiyuuToCompare" @resetList="resetList"/>
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
      alreadyOnTheList: false,
      reloadNeeded: false,
      seiyuuDataFetched: false
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
