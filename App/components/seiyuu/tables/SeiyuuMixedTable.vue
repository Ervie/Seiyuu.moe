<template>
    <v-data-table 
        :headers="headers" 
        :items="items" 
        :expand="true"
        hide-actions
        item-key="anime[0].entry.name"
        class="elevation-1">
        <template v-slot:headerCell="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" />
        </template>
        <template v-slot:items="props">
          <tr>
            <td>
              <avatar-record-cell :items="singleObjectToArray(props.item.anime)" :tableType="'Anime'"  />
            </td>
            <td v-for="role in props.item.seiyuuCharacters" :key="role.seiyuu.malId">
              <avatar-record-cell :items="role.characters" :tableType="'Character'"  />
            </td>
          </tr>
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
import AvatarRecordCell from '@/components/shared/tables/AvatarRecordCell'

export default {
    name: 'SeiyuuMixedTable',
    components: {
        'table-header': TableHeader,
        'avatar-record-cell': AvatarRecordCell
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

