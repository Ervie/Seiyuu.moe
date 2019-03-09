<template>
    <v-data-table 
        :headers="headers" 
        :items="items" 
        :expand="true"
        hide-actions
        item-key="seiyuu[0].entry.name"
        class="elevation-1">
        <template v-slot:headerCell="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" />
        </template>
        <template v-slot:items="props">
          <tr>
            <td>
              <avatar-record-cell :items="props.item.seiyuu" />
            </td>
            <td v-for="role in props.item.roles" :key="role.anime">
              <avatar-record-cell :items="role.characters" />
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
    name: 'AnimeMixedTable',
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

