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
              :groupBySeries="groupBySeries"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-mixed`" >
            <seiyuu-mixed-table
              :items="tableData" 
              :headers="headers"
              :groupBySeries="groupBySeries"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-compact`" >
            <seiyuu-compact-table
              :items="tableData" 
              :headers="headers"
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
    tableData: {
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
    }
  },
  data () {
    return {
      headers: [],
      viewMode: 'tab-expanded'
    }
  },
  methods: {
    selectComputeMethod () {
      if (!this.groupBySeries) {
        this.computeResultsSimple()
      } else {
        this.computeResultsSimple()
      }
    },
    computeResultsSimple () {
      this.setTableHeaders();
    },
    computeResultsSeries () {
      // this.tableData = []
      // var animeIndex = -1
      // var charactersIndex = -1
      // var franchiseIndex = -1

      // for (var i = 0; i < this.inputData.length; i++) {
      //   animeIndex = -1
      //   charactersIndex = -1
      //   if (this.tableData.length > 0) {
      //     for (var j = 0; j < this.tableData.length && animeIndex === -1; j++) {
      //       animeIndex = this.tableData[j].anime.map(x => x.entry.mal_id).indexOf(this.inputData[i].anime.mal_id)
      //       franchiseIndex = j
      //     }
      //     if (animeIndex === -1) {
      //       for (j = 0; j < this.tableData.length; j++) {
      //         for (var k = 0; k < this.seiyuuData.length && charactersIndex === -1; k++) {
      //           charactersIndex = this.tableData[j].roles[k].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[k].character.mal_id)
      //           franchiseIndex = j
      //         }
      //       }
      //     }
      //   }
      //   if (charactersIndex === -1 && animeIndex === -1) {
      //     this.tableData.push({
      //       anime: [{
      //         entry: {
      //           name: decode(this.inputData[i].anime.name),
      //           image_url: this.inputData[i].anime.image_url,
      //           url: this.inputData[i].anime.url,
      //           mal_id: this.inputData[i].anime.mal_id
      //         }
      //       }],
      //       roles: []
      //     })
      //     for (var l = 0; l < this.seiyuuData.length; l++) {
      //       this.tableData[this.tableData.length - 1].roles.push({
      //         seiyuu: this.inputData[i].roles[l].seiyuu,
      //         characters: [{
      //           entry: this.inputData[i].roles[l].character
      //         }]
      //       })
      //     }
      //   } else if (animeIndex === -1) {
      //     this.tableData[franchiseIndex].anime.push({
      //       entry: {
      //         name: decode(this.inputData[i].anime.name),
      //         image_url: this.inputData[i].anime.image_url,
      //         url: this.inputData[i].anime.url,
      //         mal_id: this.inputData[i].anime.mal_id
      //       }
      //     })
      //   } else {
      //     for (var seiyuuIndex = 0; seiyuuIndex < this.seiyuuData.length; seiyuuIndex++) {
      //       var roleIndex = this.tableData[franchiseIndex].roles[seiyuuIndex].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[seiyuuIndex].character.mal_id)
      //       if (roleIndex === -1) {
      //         this.tableData[franchiseIndex].roles[seiyuuIndex].characters.push({ entry: this.inputData[i].roles[seiyuuIndex].character })
      //       }
      //     }
      //   }
      // }
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
        value: 'anime.title',
        image: ''
      });
      for (var headerIndex = 0; headerIndex < this.tableData[0].seiyuuCharacters.length; headerIndex++) {
        this.headers.push({
          text: this.tableData[0].seiyuuCharacters[headerIndex].seiyuu.name,
          sortable: false,
          image: this.tableData[0].seiyuuCharacters[headerIndex].seiyuu.imageUrl
        });
      };
      if (this.viewMode === 'tab-compact') {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        });
      }
      this.showTables = true;
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
