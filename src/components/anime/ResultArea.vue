<template>
    <v-layout>
      <v-flex>
        <div>
          <v-btn raised large color="error" v-on:click="resetList" :disabled="inputData.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" v-on:click="computeResults" :disabled="inputData.length < 2">Compare</v-btn>
        </div>
      </v-flex>
    </v-layout>
</template>

<script>
import SeiyuuTable from '@/components/seiyuu/SeiyuuTable.vue'

export default {
  name: 'ResultArea',
  components: {
    'seiyuu-table': SeiyuuTable
  },
  props: ['inputData'],
  data () {
    return {
      showTables: false,
      avatarMode: false,
      counter: 0,
      outputData: []
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    computeResults () {
      this.outputData = []
      var intersectAnime = []
      var filteredData = new Array(this.inputData.length)
      var partialResults = new Array(this.inputData.length)

      for (var i = 0; i < this.inputData.length; i++) {
        partialResults[i] = []
        filteredData[i] = []
      }

      if (this.searchOption.mainRolesOnly) {
        for (i = 0; i < this.inputData.length; i++) {
          filteredData[i] = this.seiyuuRoles[i].filter(x => x.character.role === 'Main')
        }
      } else {
        for (i = 0; i < this.inputData.length; i++) {
          filteredData[i] = this.seiyuuRoles[i]
        }
      }

      for (i = 0; i < filteredData[0].length; i++) {
        partialResults[0].push({
          anime: filteredData[0][i].anime,
          roles: [{
            seiyuu: this.inputData[0].name,
            character: filteredData[0][i].character
          }]
        })
      }

      for (var seiyuuIndex = 1; seiyuuIndex < filteredData.length; seiyuuIndex++) {
        for (i = 0; i < filteredData[seiyuuIndex].length; i++) {
          for (var j = 0; j < partialResults[seiyuuIndex - 1].length; j++) {
            if (partialResults[seiyuuIndex - 1][j].anime.mal_id === filteredData[seiyuuIndex][i].anime.mal_id) {
              // Weird deep clone of object
              var cloneObject = JSON.parse(JSON.stringify(partialResults[seiyuuIndex - 1][j]))
              partialResults[seiyuuIndex].push(cloneObject)
              partialResults[seiyuuIndex][partialResults[seiyuuIndex].length - 1].roles.push({
                seiyuu: this.inputData[seiyuuIndex].name,
                character: filteredData[seiyuuIndex][i].character
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
    animeCharacters () {
      return this.inputData.map(x => x.characters)
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
