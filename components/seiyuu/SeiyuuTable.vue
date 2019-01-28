<template>
  <div>
    <v-container hidden-md-and-down>
      <v-expansion-panel>
        <v-expansion-panel-content>
          <div slot="header" class="title">Display mode</div>
          <v-radio-group v-model="viewMode" row>
            <v-radio label="Expanded (avatars)" value="expanded"></v-radio>
            <v-radio label="Mixed (small avatars)" value="mixed"></v-radio>
            <v-radio label="Compact (text)" value="compact"></v-radio>
          </v-radio-group>
        </v-expansion-panel-content>
      </v-expansion-panel>
      <v-data-table 
        :headers="headers" 
        :items="tableData" 
        :expand="true"
        :hide-headers="viewMode === 'expanded'"
        hide-actions
        item-key="anime[0].entry.name"
        class="elevation-1">
        <template slot="headerCell" slot-scope="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" />
        </template>
        <template slot="items" slot-scope="props">
          <tr v-if="viewMode === 'compact'">
            <td>
              <text-record-cell :items="props.item.anime" />
            </td>
            <td v-for="role in props.item.roles" :key="role.seiyuu">
              <text-record-cell :items="role.characters" />
            </td>
            <td>
              <v-btn fab dark small
                color="secondary"
                @click="props.expanded = !props.expanded"
              >
                <v-icon>{{ props.expanded ? 'keyboard_arrow_up' : 'keyboard_arrow_down' }}</v-icon>
              </v-btn>
          </td>
          </tr>
          <tr v-else-if="viewMode === 'mixed'">
            <td>
              <avatar-record-cell :items="props.item.anime" />
            </td>
            <td v-for="role in props.item.roles" :key="role.seiyuu">
              <avatar-record-cell :items="role.characters" />
            </td>
          </tr>
          <expanded-panel v-else
              :mainColumnItems="props.item.anime" 
              :subColumnsItems="props.item.roles"
              :tableType="'Seiyuu'"
              class="expandedRow"/>
        </template>
        <template slot="expand" slot-scope="props">
            <expanded-panel 
              v-if="viewMode === 'compact'" 
              :mainColumnItems="props.item.anime" 
              :subColumnsItems="props.item.roles"
              :tableType="'Seiyuu'"
              class="expandedRow"/>
        </template>
        <template slot="no-data">
          <v-alert :value="true" color="error" icon="warning">
            Sorry, nothing to display here :(
          </v-alert>
        </template>
      </v-data-table>
    </v-container>
     <v-container hidden-lg-and-up>
       <card-cell v-for="(item, i) in tableData" v-bind:key="i" :item="item"/>
    </v-container>
  </div>
</template>

<script>
import decode from 'decode-html'
import TableHeader from '@/components/shared/tables/TableHeader'
import ExpandedPanel from '@/components/shared/tables/ExpandedPanel'
import AvatarRecordCell from '@/components/shared/tables/AvatarRecordCell'
import TextRecordCell from '@/components/shared/tables/TextRecordCell'
import CardCell from '@/components/shared/tables/seiyuu/CardCell'

export default {
  name: 'SeiyuuTable',
  components: {
    'table-header': TableHeader,
    'expanded-panel': ExpandedPanel,
    'avatar-record-cell': AvatarRecordCell,
    'text-record-cell': TextRecordCell,
    'card-cell': CardCell
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
      viewMode: 'expanded'
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
      this.headers = []
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
      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime[0].entry.name'
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].characters.length',
          image: this.seiyuuData[headerIndex].image_url})
      }
      if (this.viewMode === 'compact') {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        })
      }
      this.showTables = true
    },
    computeResultsSeries () {
      this.tableData = []
      this.headers = []
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

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime[0].entry.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].characters.length',
          image: this.seiyuuData[headerIndex].image_url})
      }
      if (this.viewMode === 'compact') {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        })
      }
      this.showTables = true
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
      handler: 'selectComputeMethod',
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
