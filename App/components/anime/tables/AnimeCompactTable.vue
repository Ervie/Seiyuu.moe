<template>
    <v-data-table 
        :headers="headers" 
        :items="items" 
        :expand="true"
        hide-actions
        item-key="seiyuu[0].entry.name"
        class="elevation-1">
        <template slot="headerCell" slot-scope="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" />
        </template>
        <template slot="items" slot-scope="props">
          <tr>
            <td>
              <text-record-cell :items="props.item.seiyuu" />
            </td>
            <td v-for="role in props.item.roles" :key="role.anime">
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
        </template>
        <template slot="expand" slot-scope="props">
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
    name: 'AnimeCompactTable',
    components: {
        'table-header': TableHeader,
        'expanded-panel': ExpandedPanel,
        'text-record-cell': TextRecordCell
    },
    props: {
        headers: {
            type: Array,
            required: true
        },
        items: {
            type: Array,
            required: true
        }
    }
}
</script>

