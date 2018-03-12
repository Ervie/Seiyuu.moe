<template>
  <v-container grid-list-md text-xs-center>
    <browser @seiyuuReturned="addSeiyuu" @alreadyOnTheList="alreadyOnTheList = true" @resetList="resetList" :searchedIdCache="searchedId"/>
    <v-alert dismissible color="error" v-model="tooMuchRecords">
      You can choose {{ maximumSeiyuuNumber }} seiyuu at max.
    </v-alert>
    <v-alert dismissible color="error" v-model="alreadyOnTheList">
      This seiyuu is already selected.
    </v-alert>
    <v-layout row wrap>
      <v-flex v-for="(seiyuu, index) in seiyuuToCompare" :key="seiyuu.mal_id" xs2>
        <seiyuuCard :seiyuuData="seiyuu" :cardId="index"
         @seiyuuRemoved="removeSeiyuu"/>
      </v-flex>
    </v-layout>
    <v-btn raised large color="error" v-on:click="resetList" :disabled="seiyuuToCompare.length < 1">Reset</v-btn>
    <relationMatrix :inputData="seiyuuToCompare"/>
  </v-container>
</template>

<script>
import SeiyuuBrowser from '@/components/SeiyuuBrowser.vue'
import SeiyuuCard from '@/components/SeiyuuCard.vue'
import RelationMatrix from '@/components/RelationMatrix.vue'

export default {
  name: 'MainContent',
  components: {
    'browser': SeiyuuBrowser,
    'seiyuuCard': SeiyuuCard,
    'relationMatrix': RelationMatrix
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
</style>
