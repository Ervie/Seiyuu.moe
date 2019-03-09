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
              :mainColumnItems="props.item.seiyuu" 
              :subColumnsItems="props.item.roles" 
              :tableType="'Anime'"
              class="expandedRow"/>
        </template>
        <template slot="no-data">
          <v-alert :value="true" color="error" icon="warning">
            Sorry, nothing to display here :(
          </v-alert>
        </template>
      </v-data-table>
</template>

<script>
import ExpandedPanel from '@/components/shared/tables/ExpandedPanel'
import TableHeader from '@/components/shared/tables/TableHeader'
import TextRecordCell from '@/components/shared/tables/TextRecordCell'

export default {
    name: 'AnimeExpandedTable',
    components: {
        'table-header': TableHeader,
        'expanded-panel': ExpandedPanel
    },
    props: {
        headers: {
            type: Array,
            required: false
        },
        items: {
            type: Array,
            required: false
        }
    }
}
</script>

