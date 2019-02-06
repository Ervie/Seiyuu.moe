<template>
    <v-layout>
      <v-flex>
        <v-expansion-panel popout>
          <v-expansion-panel-content>
            <div slot="header" class="title">Compare Options</div>
            <v-card>
              <v-layout row wrap>
                <v-flex xs12 md6 lg4>
                  <v-checkbox label="Group by series" v-model="groupBySeries" color="secondary"></v-checkbox>
                </v-flex>
                <v-flex xs12 md6 lg4>
                  <v-checkbox label="Main roles only" v-model="mainRolesOnly" color="secondary"></v-checkbox>
                </v-flex>
              </v-layout>
            </v-card>
          </v-expansion-panel-content>
        </v-expansion-panel>
        <div>
          <v-btn raised large color="error" class="optionButton" 
            @click="resetList" 
            :disabled="inputData.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" class="optionButton" 
            @click="computeResults" 
            :disabled="inputData.length < 2">Compare</v-btn>
          <v-btn depressed large color="secondary" class="optionButton" 
            @click="generateShareLink" 
            :disabled="!showTables || inputData.length < 2">Share Link</v-btn>
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
          <v-tab :href="`#tab-table`">
            Table
              <font-awesome-icon size="3x" :icon="['fa', 'table']"/>
          </v-tab>
        </v-tabs>
        <v-tabs-items v-model="tabs" v-if="showTables">
          <v-tab-item :value="`tab-table`" >
            <seiyuu-table-selection 
              :inputData="outputData" 
              :counter="counter" 
              :seiyuuData="inputData" 
              :groupBySeries="groupBySeries"
            />
          </v-tab-item>
        </v-tabs-items>
      </v-flex>
      <share-link-snackbar
        :showSnackbar="snackbar"
        @hideSnackBar="snackbar = false"/>
    </v-layout>
</template>

<script>
import SeiyuuTableSelection from '@/components/seiyuu/SeiyuuTableSelection.vue'
import ShareLinkSnackbar from '@/components/shared/ui-components/ShareLinkSnackbar.vue';

export default {
  name: 'SeiyuuResultArea',
  components: {
    'seiyuu-table-selection': SeiyuuTableSelection,
    'share-link-snackbar': ShareLinkSnackbar
  },
  props: {
    inputData: {
      type: Array,
      required: false
    },
    runImmediately: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      showTables: false,
      counter: 0,
      tabs: 'tab-table',
      outputData: [],
      groupBySeries: true,
      mainRolesOnly: false,
      snackbar: false
    }
  },
  computed: {
    seiyuuRoles () {
      return this.inputData.map(x => x.voice_acting_roles)
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    generateShareLink () {
      var seiyuuIds = ''
      this.inputData.forEach(element => {
        seiyuuIds += element.mal_id + ';'
      });
      
      seiyuuIds = seiyuuIds.slice(0, -1)
      seiyuuIds = this.encodeURL(seiyuuIds)

      var shareLink = process.env.baseUrl + $nuxt.$route.path.toLowerCase() + '?seiyuuIds=' + seiyuuIds

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

      if (this.mainRolesOnly) {
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
  watch: {
    inputData: function (newVal, oldVal) {
      if (this.inputData.length === 0) {
        this.showTables = false
      }
    },
    runImmediately: function (val) {
      if (val === true) {
        this.computeResults()
      }
    },
    mainRolesOnly: {
      handler: 'computeResults',
      immediate: false
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
