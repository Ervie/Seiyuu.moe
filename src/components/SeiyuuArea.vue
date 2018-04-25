<template>
  <v-container grid-list-md text-xs-center>
    <browser @seiyuuReturned="addSeiyuu" @alreadyOnTheList="alreadyOnTheList = true" @resetList="resetList" :searchedIdCache="searchedId"/>
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
import SeiyuuBrowser from '@/components/SeiyuuBrowser.vue'
import SeiyuuCardList from '@/components/SeiyuuCardList.vue'
import ResultArea from '@/components/ResultArea.vue'

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
      alreadyOnTheList: false
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
