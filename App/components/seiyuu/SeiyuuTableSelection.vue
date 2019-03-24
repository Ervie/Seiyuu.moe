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
            />
          </v-tab-item>
          <v-tab-item :value="`tab-mixed`" >
            <seiyuu-mixed-table
              :items="tableData" 
              :headers="headers"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-compact`" >
            <seiyuu-compact-table
              :items="tableData" 
              :headers="headers"
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
    }
  },
  watch: {
    viewMode: {
      handler: 'setTableHeaders',
      immediate: false
    },
    tableData: {
      handler: 'setTableHeaders',
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
