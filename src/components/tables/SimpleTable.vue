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
        <td>
            <single-record-cell
            :avatarMode="avatarMode"
            :item="props.item.anime"
            :preferText="true"
            />
        </td>
        <td class="text-xs-right" v-for="character in props.item.roles" :key="character.mal_id">
            <!-- Normal mode character records -->
            <single-record-cell
            :avatarMode="avatarMode"
            :item="character.character"
            :preferText="false"
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

export default {
  name: 'SimpleTable',
  components: {
    'table-header': TableHeader,
    'single-record-cell': SingleRecordCell
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
          anime: {
            name: decode(this.inputData[i].anime.name),
            image_url: this.inputData[i].anime.image_url,
            url: this.inputData[i].anime.url
          },
          roles: this.inputData[i].roles
        })
      }

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].character.name',
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
