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
    tableData: {
      type: Array,
      required: false
    }
  },
  data () {
    return {
      headers: [],
      viewMode: 'tab-expanded'
    }
  },
  methods: {
    setTableHeaders () {
      this.headers = [];

      this.headers.push({
        text: 'Seiyuu',
        align: 'left',
        value: 'seiyuu.name',
        image: ''
      });
      
      if (this.tableData != null && this.tableData.length > 0)
      {
        for (var headerIndex = 0; headerIndex < this.tableData[0].animeCharacters.length; headerIndex++) {
          this.headers.push({
            text: this.tableData[0].animeCharacters[headerIndex].anime.title,
            sortable: false,
            image: this.tableData[0].animeCharacters[headerIndex].anime.imageUrl});
        }
        if (this.viewMode === 'tab-compact') {
          this.headers.push({
            text: '',
            sortable: false,
            value: 'name'
          });
        }
      }
    }
  },
  watch: {
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

