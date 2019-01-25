<template>
  <div>
    <v-container hidden-md-and-down>
      <v-toolbar light color="primary">
        <v-spacer/>
        <v-switch
          color="white"
          class="white--text"
          label="Compact Mode"
          v-model="compactMode"
        ></v-switch>
      </v-toolbar>
      <v-data-table 
        :headers="headers" 
        :items="tableData" 
        :expand="true"
        hide-actions
        item-key="seiyuu[0].entry.name"
        class="elevation-1">
        <template slot="headerCell" slot-scope="props">
          <table-header 
          :imageUrl="props.header.image" 
          :text="props.header.text" 
          />
        </template>
        <template slot="items" slot-scope="props">
          <tr v-if="compactMode">
            <td>
              <multi-record-cell :preferText="true" :items="props.item.seiyuu" />
            </td>
            <td v-for="role in props.item.roles" :key="role.anime">
              <multi-record-cell :preferText="true" :items="role.characters" />
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
          <tr v-else>
            <td>
              <multi-record-cell :preferText="false" :items="props.item.seiyuu" />
            </td>
            <td v-for="role in props.item.roles" :key="role.anime">
              <multi-record-cell :preferText="false" :items="role.characters" />
            </td>
          </tr>
        </template>
        <template slot="expand" slot-scope="props">
          <expanded-panel 
            v-if="compactMode" 
            :mainColumnItems="props.item.seiyuu" 
            :subColumnsItems="props.item.roles" 
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
import TableHeader from '@/components/shared/tables/TableHeader'
import ExpandedPanel from '@/components/shared/tables/ExpandedPanel'
import MultiRecordCell from '@/components/shared/tables/MultiRecordCell'
import CardCell from '@/components/shared/tables/anime/CardCell'

export default {
  name: 'AnimeTable',
  components: {
    'table-header': TableHeader,
    'expanded-panel': ExpandedPanel,
    'multi-record-cell': MultiRecordCell,
    'card-cell': CardCell
  },
  props: {
    charactersData: {
      type: Array,
      required: false
    },
    counter: {
      type: Number,
      required: true
    },
    animeData: {
      type: Array,
      required: false
    }
  },
  data () {
    return {
      headers: [],
      tableData: [],
      compactMode: true
    }
  },
  methods: {
    computeResults () {
      this.tableData = []
      this.headers = []
      var intersectSeiyuu = []
      var characterIndex = -1
      var seiyuuIndex = -1

      for (var i = 0; i < this.charactersData.length; i++) {
        if (intersectSeiyuu.length > 0) {
          seiyuuIndex = intersectSeiyuu.map(x => x.seiyuu[0].entry.name).indexOf(this.charactersData[i].seiyuu.name)
        }
        if (seiyuuIndex === -1) {
          intersectSeiyuu.push({
            seiyuu: [{
              entry: {
                name: this.charactersData[i].seiyuu.name,
                image_url: this.charactersData[i].seiyuu.image_url,
                url: this.charactersData[i].seiyuu.url
              }
            }],
            roles: []
          })
          for (var j = 0; j < this.charactersData[i].roles.length; j++) {
            intersectSeiyuu[intersectSeiyuu.length - 1].roles.push({
              anime: this.charactersData[i].roles[j].anime,
              characters: [{
                entry: this.charactersData[i].roles[j].character
              }]
            })
          }
        } else {
          for (var l = 0; l < this.animeData.length; l++) {
            characterIndex = intersectSeiyuu[seiyuuIndex].roles[l].characters.map(x => x.entry.mal_id).indexOf(this.charactersData[i].roles[l].character.mal_id)
            if (characterIndex === -1) {
              intersectSeiyuu[seiyuuIndex].roles[l].characters.push({
                entry: this.charactersData[i].roles[l].character
              })
            }
          }
        }
      }

      this.tableData = intersectSeiyuu
      this.headers.push({
        text: 'Seiyuu',
        align: 'left',
        value: 'seiyuu[0].entry.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.animeData.length; headerIndex++) {
        this.headers.push({
          text: this.animeData[headerIndex].title,
          value: 'roles[' + headerIndex + '].characters.length',
          image: this.animeData[headerIndex].image_url})
      }
      if (this.compactMode) {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        })
      }
      this.showTables = true
    },
    computeResultsSimple () {
      // Old code - might be reused in future
      this.tableData = []
      this.headers = []
      for (var i = 0; i < this.charactersData.length; i++) {
        this.tableData.push({
          seiyuu: [{
            entry: {
              name: this.charactersData[i].seiyuu.name,
              image_url: this.charactersData[i].seiyuu.image_url,
              url: this.charactersData[i].seiyuu.url
            }
          }],
          roles: []
        })
        for (var l = 0; l < this.charactersData[i].roles.length; l++) {
          this.tableData[this.tableData.length - 1].roles.push({
            anime: this.charactersData[i].roles[l].anime,
            characters: [{
              entry: this.charactersData[i].roles[l].character
            }]
          })
        }
      }

      this.headers.push({
        text: 'Seiyuu',
        align: 'left',
        value: 'seiyuu[0].entry.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.animeData.length; headerIndex++) {
        this.headers.push({
          text: this.animeData[headerIndex].title,
          value: 'roles[' + headerIndex + '].characters[0].entry.name',
          image: this.animeData[headerIndex].image_url})
      }
      if (this.compactMode) {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        })
      }
      this.showTables = true
    },
  },
  watch: {
    counter: {
      handler: 'computeResults',
      immediate: true
    }
  }
}
</script>

<style>
.expandedRow {
  border-bottom: 1px solid rgba(255,255,255,0.12);
}
</style>

