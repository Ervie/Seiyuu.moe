<template>
    <v-data-table
        :headers="headers"
        :items="tableData"
        hide-actions
        class="elevation-1">
        <template slot="headerCell" slot-scope="props" >
        <!-- Headers -->
            <table-header
                :imageUrl="props.header.image"
                :text="props.header.text"
                :avatarMode="avatarMode"
            />
        </template>
        <template slot="items" slot-scope="props">
          <td class="text-xs-right">
              <multi-record-cell
              :avatarMode="avatarMode"
              :items="props.item.anime"
              />
        </td>
          <td class="text-xs-right" v-for="role in props.item.roles" :key="role.seiyuu">
            <multi-record-cell
            :avatarMode="avatarMode"
            :items="role.characters"
            />
        </td>
        </template>
        <template slot="no-data">
        <v-alert :value="true" color="error" icon="warning">
            Sorry, nothing to display here :(
        </v-alert>
        </template>
    </v-data-table>
</template>

<script>
import decode from 'decode-html'
import TableHeader from './cells/TableHeader'
import SingleRecordCell from './cells/SingleRecordCell'
import MultiRecordCell from './cells/MultiRecordCell'

export default {
  name: 'SimpleTable',
  components: {
    'table-header': TableHeader,
    'single-record-cell': SingleRecordCell,
    'multi-record-cell': MultiRecordCell
  },
  props: ['inputData', 'avatarMode', 'counter', 'seiyuuData'],
  data () {
    return {
      headers: [],
      tableData: []
    }
  },
  methods: {
    computeResults () {
      this.tableData = []
      this.headers = []

      for (var i = 0; i < this.inputData.length; i++) {
        this.tableData.push({
          anime: [{
            entry: {
              name: decode(this.inputData[i].anime.name),
              image_url: this.inputData[i].anime.image_url,
              url: this.inputData[i].anime.url
            }
          }],
          roles: []
        })
        for (var l = 0; l < this.seiyuuData.length; l++) {
          this.tableData[this.tableData.length - 1].roles.push({
            seiyuu: this.inputData[i].roles[l].seiyuu,
            characters: [{
              entry: this.inputData[i].roles[l].character
            }]
          })
        }
      }

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime[0].entry.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].characters[0].entry.name',
          image: this.seiyuuData[headerIndex].image_url})
      }
      this.showTables = true
    }
  },
  watch: {
    counter: {
      handler: 'computeResults',
      immediate: true
    }
  }
}
</script>

<style scoped>

</style>
