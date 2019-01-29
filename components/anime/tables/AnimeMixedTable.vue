<template>
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
          <tr>
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
</template>

<script>
import TableHeader from '@/components/shared/tables/TableHeader'
import TextRecordCell from '@/components/shared/tables/TextRecordCell'

export default {
    name: 'AnimeCompactTable',
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

