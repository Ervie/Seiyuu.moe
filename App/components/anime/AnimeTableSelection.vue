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
            <anime-expanded-table
              :items="tableData" 
              :headers="headers"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-mixed`" >
            <anime-mixed-table
              :items="tableData" 
              :headers="headers"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-compact`" >
            <anime-compact-table
              :items="tableData" 
              :headers="headers"
            />
          </v-tab-item>
        </v-tabs-items>
    </v-container>
    <v-container hidden-lg-and-up>
       <anime-data-iterator
          :items="tableData" 
        />
    </v-container>
  </div>
</template>

<script>
import AnimeCompactTable from '@/components/anime/tables/AnimeCompactTable'
import AnimeDataIterator from '@/components/anime/tables/AnimeDataIterator'
import AnimeExpandedTable from '@/components/anime/tables/AnimeExpandedTable'
import AnimeMixedTable from '@/components/anime/tables/AnimeMixedTable'

export default {
  name: 'AnimeTableSelection',
  components: {
    'anime-compact-table': AnimeCompactTable,
    'anime-data-iterator': AnimeDataIterator,
    'anime-expanded-table': AnimeExpandedTable,
    'anime-mixed-table': AnimeMixedTable
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
      viewMode: 'tab-expanded'
    }
  },
  methods: {
    computeResults () {
      this.tableData = []
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

      this.tableData = intersectSeiyuu;
      this.setTableHeaders();
    },
    setTableHeaders () {
      this.headers = [];

      this.headers.push({
        text: 'Seiyuu',
        align: 'left',
        value: 'seiyuu[0].entry.name',
        image: ''
      });
      for (var headerIndex = 0; headerIndex < this.animeData.length; headerIndex++) {
        this.headers.push({
          text: this.animeData[headerIndex].title,
          sortable: false,
          image: this.animeData[headerIndex].imageUrl});
      }
      if (this.viewMode === 'tab-compact') {
        this.headers.push({
          text: '',
          sortable: false,
          value: 'name'
        });
      }
      this.showTables = true;
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
          image: this.animeData[headerIndex].imageUrl})
      }
      if (this.viewMode === 'tab-compact') {
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

