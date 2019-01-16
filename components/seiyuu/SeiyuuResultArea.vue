<template>
    <v-layout>
      <v-flex>
        <v-expansion-panel popout>
          <v-expansion-panel-content>
            <div slot="header" class="title">Compare Options</div>
            <v-card>
              <v-layout row wrap hidden-sm-and-down>
                <v-flex xs4>
                  <v-checkbox label="Group by series" v-model="searchOption.groupBySeries" color="secondary"></v-checkbox>
                </v-flex>
                <v-flex xs4>
                  <v-checkbox label="Group by character" v-model="searchOption.groupByCharacter" color="secondary"></v-checkbox>
                </v-flex>
                <v-flex xs4>
                  <v-checkbox label="Main roles only" v-model="searchOption.mainRolesOnly" color="secondary"></v-checkbox>
                </v-flex>
              </v-layout>
              <v-layout row wrap hidden-md-and-up>
                <v-flex xs12>
                  <v-checkbox label="Group by series" v-model="searchOption.groupBySeries" color="secondary"></v-checkbox>
                </v-flex>
                <v-flex xs12>
                  <v-checkbox label="Group by character" v-model="searchOption.groupByCharacter" color="secondary"></v-checkbox>
                </v-flex>
                <v-flex xs12>
                  <v-checkbox label="Main roles only" v-model="searchOption.mainRolesOnly" color="secondary"></v-checkbox>
                </v-flex>
              </v-layout>
            </v-card>
          </v-expansion-panel-content>
        </v-expansion-panel>
        <div>
          <v-btn raised large color="error" class="optionButton" v-on:click="resetList" :disabled="inputData.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" class="optionButton" v-on:click="computeResults" :disabled="inputData.length < 2">Compare</v-btn>
          <v-btn depressed large color="secondary" class="optionButton" v-on:click="generateShareLink" :disabled="!showTables || inputData.length < 2">Share Link</v-btn>
        </div>
        <v-tabs
          v-if="showTables"
          centered
          icons-and-text
          slot="extension"
          v-model="tabs"
          color="primary"
          slider-color="secondary"
          grow
        >
          <v-tab :href="`#tab-anime`">
            Anime
            <v-icon large>fa-tv</v-icon>
          </v-tab>
        </v-tabs>
        <v-tabs-items v-model="tabs" v-if="showTables">
          <v-tab-item :value="`tab-anime`" >
            <seiyuu-table 
              :inputData="outputData" 
              :avatarMode="avatarMode" 
              :counter="counter" 
              :seiyuuData="inputData" 
              :groupingMode="selectedTable"
            />
          </v-tab-item>
        </v-tabs-items>
      </v-flex>
      <v-snackbar
        v-model="snackbar"
        color="secondary"
        :timeout="3000"
        right top
      >
        Sharelink has been copied to the clipboard.
        <v-btn
          dark
          flat
          @click="snackbar = false"
        >
          Close
        </v-btn>
      </v-snackbar>
    </v-layout>
</template>

<script>
import SeiyuuTable from '@/components/seiyuu/SeiyuuTable.vue'

export default {
  name: 'ResultArea',
  components: {
    'seiyuu-table': SeiyuuTable
  },
  props: ['inputData', 'runImmediately'],
  data () {
    return {
      showTables: false,
      avatarMode: false,
      windowWidth: 0,
      counter: 0,
      tabs: 'tab-anime',
      outputData: [],
      searchOption: {
        groupBySeries: true,
        groupByCharacter: true,
        mainRolesOnly: false
      },
      snackbar: false
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
    generateShareLink () {
      var seiyuuIds = ''
      this.inputData.forEach(element => {
        seiyuuIds += element.mal_id + ';'
      });
      
      seiyuuIds = seiyuuIds.slice(0, -1)
      seiyuuIds = this.encodeURL(seiyuuIds)

      var shareLink = process.env.baseUrl + $nuxt.$route.path + '?seiyuuIds=' + seiyuuIds

      this.$copyText(shareLink)
      this.snackbar = true
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
          filteredData[i] = this.seiyuuRoles[i].filter(x => x.role === 'Main')
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
    seiyuuRoles () {
      return this.inputData.map(x => x.voice_acting_roles)
    },
    selectedTable () {
      if (this.searchOption.groupBySeries) {
        if (this.searchOption.groupByCharacter) {
          return 'SeriesCharacters'
        } else {
          return 'Series'
        }
      } else {
        if (this.searchOption.groupByCharacter) {
          return 'Characters'
        } else {
          return 'None'
        }
      }
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
    },
    runImmediately: function (val) {
      if (val === true) {
        this.computeResults()
      }
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
