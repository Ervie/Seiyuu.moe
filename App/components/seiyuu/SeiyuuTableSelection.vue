<template>
  <div>
    <v-container hidden-md-and-down>
      <v-tabs
          centered
          fixed-tabs
          slot="extension"
          v-model="viewMode"
          color="primary"
          slider-color="secondary"
          grow
        >
          <v-tab :href="`#tab-expanded`">
            Expanded View
          </v-tab>
          <v-tab :href="`#tab-mixed`">
            Mixed View
          </v-tab>
          <v-tab :href="`#tab-compact`">
            Compact View
          </v-tab>
        </v-tabs>
        <v-tabs-items v-model="viewMode">
          <v-tab-item :value="`tab-expanded`" >
            <seiyuu-expanded-table
              :items="tableData" 
              :headers="headers"
              :seiyuuData="inputData" 
              :groupBySeries="groupBySeries"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-mixed`" >
            <seiyuu-mixed-table
              :items="tableData" 
              :headers="headers"
              :seiyuuData="inputData" 
              :groupBySeries="groupBySeries"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-compact`" >
            <seiyuu-compact-table
              :items="tableData" 
              :headers="headers"
              :seiyuuData="inputData" 
              :groupBySeries="groupBySeries"
            />
          </v-tab-item>
        </v-tabs-items>
    </v-container>
     <v-container hidden-lg-and-up>
       <seiyuu-data-iterator
          :items="tableData" 
        />
    </v-container>
  </div>
</template>

<script>
import decode from 'decode-html'
import SeiyuuCompactTable from '@/components/seiyuu/tables/SeiyuuCompactTable'
import SeiyuuDataIterator from '@/components/seiyuu/tables/SeiyuuDataIterator'
import SeiyuuExpandedTable from '@/components/seiyuu/tables/SeiyuuExpandedTable'
import SeiyuuMixedTable from '@/components/seiyuu/tables/SeiyuuMixedTable'

export default {
  name: 'SeiyuuTableSelection',
  components: {
    'seiyuu-compact-table': SeiyuuCompactTable,
    'seiyuu-data-iterator': SeiyuuDataIterator,
    'seiyuu-expanded-table': SeiyuuExpandedTable,
    'seiyuu-mixed-table': SeiyuuMixedTable
  },
  props: {
    inputData: {
      type: Array,
      required: false
    },
    counter: {
      type: Number,
      required: true
    },
    groupBySeries: {
      type: Boolean,
      required: true
    },
    seiyuuData: {
      type: Array,
      required: false
    }
  },
  data () {
    return {
      headers: [],
      tableData: [],
      viewMode: 'tab-expanded'
    }
  },
  methods: {
    selectComputeMethod () {
      if (!this.groupBySeries) {
        this.computeResultsSimple()
      } else {
        this.computeResultsSeries()
      }
    },
    computeResultsSimple () {
      this.tableData = []
      var intersectAnime = []
      var animeIndex = -1
      var roleIndex = -1
      var seiyuuIndex = -1

      for (var i = 0; i < this.inputData.length; i++) {
        if (intersectAnime.length > 0) {
          animeIndex = intersectAnime.map(x => x.anime[0].entry.name).indexOf(this.inputData[i].anime.name)
        }
        if (animeIndex === -1) {
          intersectAnime.push({
            anime: [{
              entry: {
                name: decode(this.inputData[i].anime.name),
                image_url: this.inputData[i].anime.image_url,
                url: this.inputData[i].anime.url
              }
            }],
            roles: []
          })
          for (var j = 0; j < this.inputData[i].roles.length; j++) {
            intersectAnime[intersectAnime.length - 1].roles.push({
              seiyuu: this.inputData[i].roles[j].seiyuu,
              characters: [{
                entry: this.inputData[i].roles[j].character
              }]
            })
          }
        } else {
          for (seiyuuIndex = 0; seiyuuIndex < this.seiyuuData.length; seiyuuIndex++) {
            roleIndex = intersectAnime[animeIndex].roles[seiyuuIndex].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[seiyuuIndex].character.mal_id)
            if (roleIndex === -1) {
              intersectAnime[animeIndex].roles[seiyuuIndex].characters.push({ entry: this.inputData[i].roles[seiyuuIndex].character })
            }
          }
        }
      }

      this.tableData = intersectAnime
      this.setTableHeaders();
    },
    computeResultsSeries () {
      this.tableData = []
      var animeIndex = -1
      var charactersIndex = -1
      var franchiseIndex = -1

      for (var i = 0; i < this.inputData.length; i++) {
        animeIndex = -1
        charactersIndex = -1
        if (this.tableData.length > 0) {
          for (var j = 0; j < this.tableData.length && animeIndex === -1; j++) {
            animeIndex = this.tableData[j].anime.map(x => x.entry.mal_id).indexOf(this.inputData[i].anime.mal_id)
            franchiseIndex = j
          }
          if (animeIndex === -1) {
            for (j = 0; j < this.tableData.length; j++) {
              for (var k = 0; k < this.seiyuuData.length && charactersIndex === -1; k++) {
                charactersIndex = this.tableData[j].roles[k].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[k].character.mal_id)
                franchiseIndex = j
              }
            }
          }
        }
        if (charactersIndex === -1 && animeIndex === -1) {
          this.tableData.push({
            anime: [{
              entry: {
                name: decode(this.inputData[i].anime.name),
                image_url: this.inputData[i].anime.image_url,
                url: this.inputData[i].anime.url,
                mal_id: this.inputData[i].anime.mal_id
              }
            }],
            roles: []
          })
          for (var l = 0; l < this.seiyuuData.length; l++) {
            this.tableData[this.tableData.length - 1].roles.push({
              seiyuu: this.inputData[i].roles[l].seiyuu,
              characters: [{
                entry: this.inputData[i].roles[l].character
              }]
            })
          }
        } else if (animeIndex === -1) {
          this.tableData[franchiseIndex].anime.push({
            entry: {
              name: decode(this.inputData[i].anime.name),
              image_url: this.inputData[i].anime.image_url,
              url: this.inputData[i].anime.url,
              mal_id: this.inputData[i].anime.mal_id
            }
          })
        } else {
          for (var seiyuuIndex = 0; seiyuuIndex < this.seiyuuData.length; seiyuuIndex++) {
            var roleIndex = this.tableData[franchiseIndex].roles[seiyuuIndex].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[seiyuuIndex].character.mal_id)
            if (roleIndex === -1) {
              this.tableData[franchiseIndex].roles[seiyuuIndex].characters.push({ entry: this.inputData[i].roles[seiyuuIndex].character })
            }
          }
        }
      }
      this.setTableHeaders();
    },
    combinationCode (data) {
      var returnCode = ''
      for (var index = 0; index < data.length; index++) {
        returnCode += this.zeroFill(data[index], 7)
      }

      return returnCode
    },
    zeroFill (number, width) {
      width -= number.toString().length
      if (width > 0) {
        return new Array(width + (/\./.test(number) ? 2 : 1)).join('0') + number
      }
      return number + ''
    },
    setTableHeaders () {
      this.headers = [];

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime[0].entry.name',
        image: ''
      });
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          sortable: false,
          image: this.seiyuuData[headerIndex].image_url});
      };
      if (this.viewMode === 'tab-compact') {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        });
      }
      this.showTables = true;
    },
    computeResultsSimpleNoGrouping () {
      // Old code - might be useful in the future
      this.tableData = []
      this.headers = []

      for (var i = 0; i < this.inputData.length; i++) {
        this.tableData.push({
          anime: [{
            entry: {
              name: decode(this.inputData[i].anime.name),
              image_url: this.inputData[i].anime.image_url,
              url: this.inputData[i].anime.url
            }
          }],
          roles: []
        })
        for (var l = 0; l < this.seiyuuData.length; l++) {
          this.tableData[this.tableData.length - 1].roles.push({
            seiyuu: this.inputData[i].roles[l].seiyuu,
            characters: [{
              entry: this.inputData[i].roles[l].character
            }]
          })
        }
      }

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime[0].entry.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].characters[0].entry.name',
          image: this.seiyuuData[headerIndex].image_url})
      }
      this.headers.push({
        text: '',
        sortable: false,
        value: 'name'
      })
      this.showTables = true
    },
    computeResultsCharactersGrouping () {
      // Old code - might be useful in the future
      this.tableData = []
      this.headers = []
      var combinationIndex = -1
      var currentCombinationCode = -1

      for (var i = 0; i < this.inputData.length; i++) {
        currentCombinationCode = this.combinationCode(this.inputData[i].roles.map(x => x.character.mal_id))
        if (this.tableData.length > 0) {
          combinationIndex = this.tableData.map(x => x.combinationCode).indexOf(currentCombinationCode)
        }
        if (combinationIndex === -1) {
          this.tableData.push({
            anime: [{
              entry: {
                name: decode(this.inputData[i].anime.name),
                image_url: this.inputData[i].anime.image_url,
                url: this.inputData[i].anime.url
              }
            }],
            roles: [],
            combinationCode: currentCombinationCode
          })
          for (var l = 0; l < this.seiyuuData.length; l++) {
            this.tableData[this.tableData.length - 1].roles.push({
              seiyuu: this.inputData[i].roles[l].seiyuu,
              characters: [{
                entry: this.inputData[i].roles[l].character
              }]
            })
          }
        } else {
          this.tableData[combinationIndex].anime.push({
            entry: {
              name: decode(this.inputData[i].anime.name),
              image_url: this.inputData[i].anime.image_url,
              url: this.inputData[i].anime.url
            }
          })
        }
      }

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime.length',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].characters[0].entry.name',
          image: this.seiyuuData[headerIndex].image_url})
      }
      if (this.viewMode === 'mixed') {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        })
      }
      this.showTables = true
    }
  },
  watch: {
    counter: {
      handler: 'selectComputeMethod',
      immediate: true
    },
    groupBySeries: {
      handler: 'selectComputeMethod',
      immediate: false
    },
    viewMode: {
      handler: 'setTableHeaders',
      immediate: false
    }
  }
}
</script>

<style>
.expandedRow {
  border-bottom: 1px solid rgba(255,255,255,0.12);
}
</style>
