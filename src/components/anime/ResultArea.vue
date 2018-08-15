<template>
    <v-layout>
      <v-flex>
        <v-expansion-panel popout>
          <v-expansion-panel-content>
            <div slot="header" class="title">Compare Options</div>
            <v-card>
              <v-layout row wrap hidden-sm-and-down>
                <v-flex xs4>
                  <v-checkbox label="Group by seiyuu" v-model="searchOption.groupBySeiyuu" color="secondary"></v-checkbox>
                </v-flex>
              </v-layout>
              <v-layout row wrap hidden-md-and-up>
                <v-flex xs12>
                  <v-checkbox label="Group by seiyuu" v-model="searchOption.groupBySeiyuu" color="secondary"></v-checkbox>
                </v-flex>
              </v-layout>
            </v-card>
          </v-expansion-panel-content>
        </v-expansion-panel>
        <div>
          <v-btn raised large color="error" v-on:click="resetList" :disabled="inputData.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" v-on:click="computeResults" :disabled="inputData.length < 2">Compare</v-btn>
        </div>
        <div>
          <anime-table v-if="showTables" :inputData="outputData" :avatarMode="avatarMode" :counter="counter" :animeData="inputData" :groupBySeiyuu="searchOption.groupBySeiyuu"></anime-table>
        </div>
      </v-flex>
    </v-layout>
</template>

<script>
import AnimeTable from '@/components/anime/AnimeTable.vue'

export default {
  name: 'ResultArea',
  components: {
    'anime-table': AnimeTable
  },
  props: ['inputData'],
  data () {
    return {
      showTables: false,
      avatarMode: false,
      counter: 0,
      outputData: [],
      searchOption: {
        groupBySeiyuu: true
      }
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    computeResults () {
      this.outputData = []
      var filteredData = new Array(this.inputData.length)
      var partialResults = new Array(this.inputData.length)

      for (var i = 0; i < this.inputData.length; i++) {
        partialResults[i] = []
        filteredData[i] = []
      }

      for (var k = 0; k < this.inputData.length; k++) {
        // filteredData[i] = this.animeCharacters[i]
        for (i = 0; i < this.animeCharacters[k].length; i++) {
          for (var j = 0; j < this.animeCharacters[k][i].voice_actor.length; j++) {
            if (this.animeCharacters[k][i].voice_actor[j].language === 'Japanese') {
              filteredData[k].push({
                seiyuu: this.animeCharacters[k][i].voice_actor[j],
                roles: [{
                  anime: this.inputData[k].title,
                  character: this.animeCharacters[k][i]
                }]
              })
            }
          }
        }
      }

      partialResults[0] = filteredData[0]

      for (var animeIndex = 1; animeIndex < filteredData.length; animeIndex++) {
        for (i = 0; i < filteredData[animeIndex].length; i++) {
          for (j = 0; j < partialResults[animeIndex - 1].length; j++) {
            if (partialResults[animeIndex - 1][j].seiyuu.mal_id === filteredData[animeIndex][i].seiyuu.mal_id) {
              // Weird deep clone of object
              var cloneObject = JSON.parse(JSON.stringify(partialResults[animeIndex - 1][j]))
              partialResults[animeIndex].push(cloneObject)
              partialResults[animeIndex][partialResults[animeIndex].length - 1].roles.push({
                anime: this.inputData[animeIndex].title,
                character: filteredData[animeIndex][i].roles[0].character
              })
            }
          }
        }
        partialResults[animeIndex] = partialResults[animeIndex].filter(x => x.roles.length === (animeIndex + 1))
      }
      this.outputData = partialResults[this.inputData.length - 1]
      this.counter++
      this.showTables = true
    }
  },
  computed: {
    animeCharacters () {
      return this.inputData.map(x => x.character)
    }
  },
  watch: {
    inputData: function (newVal, oldVal) {
      if (this.inputData.length === 0) {
        this.showTables = false
      }
    }
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
