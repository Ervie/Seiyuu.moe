<template>
    <v-data-table 
        :headers="headers" 
        :items="items" 
        :expand="true"
        hide-headers
        hide-actions
        item-key="seiyuu[0].entry.name"
        class="elevation-1">
        <template v-slot:headerCell="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" />
        </template>
        <template v-slot:items="props">
          <expanded-panel
              :mainColumnItems="singleObjectToArray(props.item.seiyuu)" 
              :subColumnsItems="props.item.animeCharacters" 
              :tableType="'Anime'"
              class="expandedRow"/>
        </template>
        <template slot="no-data">
          <no-data-placeholder
            :isLoadingData="loadingComparison"
          />  
        </template>
      </v-data-table>
</template>

<script>
import ExpandedPanel from '@/components/shared/tables/ExpandedPanel'
import TableHeader from '@/components/shared/tables/TableHeader'
import TextRecordCell from '@/components/shared/tables/TextRecordCell'
import NoDataPlaceholder from '@/components/shared/tables/NoDataPlaceholder'

export default {
    name: 'AnimeExpandedTable',
    components: {
        'table-header': TableHeader,
        'expanded-panel': ExpandedPanel,
        'no-data-placeholder': NoDataPlaceholder
    },
    props: {
        headers: {
            type: Array,
            required: false
        },
        items: {
            type: Array,
            required: false
        },
        loadingComparison: {
          type: Boolean,
          required: true
        }
    }
}
</script>

