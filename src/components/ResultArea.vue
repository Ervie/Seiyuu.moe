<template>
    <v-layout>
      <v-flex>
        <div>
          <v-btn raised large color="error" v-on:click="resetList" :disabled="inputData.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" v-on:click="computeResults" :disabled="inputData.length < 2">Compare</v-btn>
        </div>
        <v-tabs
          v-if="showTables"
          centered
          icons-and-text
          slot="extension"
          v-model="tabs"
          slider-color="blue darken-2"
          grow
        >
          <v-tab :href="`#tab-simple`">
            Simple table
            <v-icon large>fa-list</v-icon>
          </v-tab>
          <v-tab :href="`#tab-series`">
            By series
            <v-icon large>fa-tv</v-icon>
          </v-tab>
          <v-tab :href="`#tab-character`">
            By character
            <v-icon large>fa-users</v-icon>
          </v-tab>
          <v-tab :href="`#tab-franchise`">
            By franchise
            <v-icon large>fa-flag</v-icon>
          </v-tab>
        </v-tabs>
        <v-tabs-items v-model="tabs" v-if="showTables" color="primary">
          <v-tab-item :id="`tab-simple`">
            <simple-table :inputData="outputData" :avatarMode="avatarMode" :counter="counter" :seiyuuData="inputData"></simple-table>
          </v-tab-item>
          <v-tab-item :id="`tab-series`">
            <series-table :inputData="outputData" :avatarMode="avatarMode" :counter="counter" :seiyuuData="inputData"></series-table>
          </v-tab-item>
          <v-tab-item :id="`tab-character`">
            <character-table :inputData="outputData" :avatarMode="avatarMode" :counter="counter" :seiyuuData="inputData"></character-table>
          </v-tab-item>
          <v-tab-item :id="`tab-franchise`">
            <franchise-table :inputData="outputData" :avatarMode="avatarMode" :counter="counter" :seiyuuData="inputData"></franchise-table>
          </v-tab-item>
        </v-tabs-items>
      </v-flex>
    </v-layout>
</template>

<script>
import SimpleTable from '@/components/tables/SimpleTable.vue'
import SeriesTable from '@/components/tables/SeriesTable.vue'
import CharactersTable from '@/components/tables/CharactersTable.vue'
import FranchiseTable from '@/components/tables/FranchiseTable.vue'

export default {
  name: 'ResultArea',
  components: {
    'simple-table': SimpleTable,
    'series-table': SeriesTable,
    'character-table': CharactersTable,
    'franchise-table': FranchiseTable
  },
  props: ['inputData'],
  data () {
    return {
      showTables: false,
      avatarMode: false,
      windowWidth: 0,
      counter: 0,
      tabs: 'tab-simple',
      outputData: []
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    handleResize (windowWidth) {
      if (windowWidth / this.inputData.length < 400) {
        this.avatarMode = true
      } else {
        this.avatarMode = false
      }
    },
    computeResults () {
      this.outputData = []
      var intersectAnime = []
      var partialResults = new Array(this.inputData.length)

      for (var i = 0; i < this.inputData.length; i++) {
        partialResults[i] = []
      }

      for (i = 0; i < this.seiyuuRoles[0].length; i++) {
        partialResults[0].push({
          anime: this.seiyuuRoles[0][i].anime,
          roles: [{
            seiyuu: this.inputData[0].name,
            character: this.seiyuuRoles[0][i].character
          }]
        })
      }

      for (var seiyuuIndex = 1; seiyuuIndex < this.seiyuuRoles.length; seiyuuIndex++) {
        for (i = 0; i < this.seiyuuRoles[seiyuuIndex].length; i++) {
          for (var j = 0; j < partialResults[seiyuuIndex - 1].length; j++) {
            if (partialResults[seiyuuIndex - 1][j].anime.mal_id === this.seiyuuRoles[seiyuuIndex][i].anime.mal_id) {
              // Weird deep clone of object
              var cloneObject = JSON.parse(JSON.stringify(partialResults[seiyuuIndex - 1][j]))
              partialResults[seiyuuIndex].push(cloneObject)
              partialResults[seiyuuIndex][partialResults[seiyuuIndex].length - 1].roles.push({
                seiyuu: this.inputData[seiyuuIndex].name,
                character: this.seiyuuRoles[seiyuuIndex][i].character
              })
            }
          }
        }
        partialResults[seiyuuIndex] = partialResults[seiyuuIndex].filter(x => x.roles.length === (seiyuuIndex + 1))
      }
      intersectAnime = partialResults[this.inputData.length - 1]

      this.outputData = intersectAnime
      this.counter++
      this.showTables = true
    }
  },
  computed: {
    seiyuuRoles () {
      return this.inputData.map(x => x.voice_acting_role)
    }
  },
  watch: {
    inputData: function (newVal, oldVal) {
      if (this.inputData.length === 0) {
        this.showTables = false
      }
    },
    windowWidth: function (newWidth, oldWidth) {
      this.handleResize(newWidth)
    }
  },
  mounted () {
    let that = this
    this.$nextTick(function () {
      window.addEventListener('resize', function (e) {
        that.windowWidth = window.innerWidth
        that.handleResize(this.windowWidth)
      })
    })
  },
  beforeDestroy: function () {
    window.removeEventListener('resize', this.handleResize)
  }
}
</script>

<style>
img.miniav {
    height: 98px;
    width: 63px;
}

img.av {
    height: 140px;
    width: 90px;
}
</style>
