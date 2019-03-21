<template>
    <v-data-table 
        :headers="headers" 
        :items="items" 
        :expand="true"
        hide-actions
        item-key="anime.malId"
        class="elevation-1">
        <template v-slot:headerCell="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" />
        </template>
        <template v-slot:items="props">
          <tr>
            <td>
              <text-record-cell :items="singleObjectToArray(props.item.anime)" :tableType="'Anime'" />
            </td>
            <td v-for="role in props.item.seiyuuCharacters" :key="role.seiyuu.malId">
              <text-record-cell :items="role.characters" :tableType="'Character'"  />
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
        <template v-slot:expand="props">
            <expanded-panel 
              :mainColumnItems="singleObjectToArray(props.item.anime)" 
              :subColumnsItems="props.item.seiyuuCharacters"
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
import ExpandedPanel from '@/components/shared/tables/ExpandedPanel'
import TableHeader from '@/components/shared/tables/TableHeader'
import TextRecordCell from '@/components/shared/tables/TextRecordCell'

export default {
    name: 'SeiyuuCompactTable',
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

