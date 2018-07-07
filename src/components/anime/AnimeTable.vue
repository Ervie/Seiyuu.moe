<template>
  <div>
    <v-container hidden-sm-and-down>
      <v-data-table :headers="headers" :items="tableData" hide-actions class="elevation-1">
        <template slot="headerCell" slot-scope="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" :avatarMode="avatarMode" />
        </template>
        <template slot="items" slot-scope="props">
          <td class="text-xs-right">
            <multi-record-cell :avatarMode="avatarMode" :items="props.item.seiyuu" />
          </td>
          <td class="text-xs-right" v-for="role in props.item.roles" :key="role.anime">
            <multi-record-cell :avatarMode="avatarMode" :items="role.characters" />
          </td>
        </template>
        <template slot="no-data">
          <v-alert :value="true" color="error" icon="warning">
            Sorry, nothing to display here :(
          </v-alert>
        </template>
      </v-data-table>
    </v-container>
    <v-container hidden-md-and-up>
       <card-cell v-for="(item, i) in tableData" v-bind:key="i" :item="item"/>
    </v-container>
  </div>
</template>

<script>
import TableHeader from '@/components/tables/TableHeader'
import SingleRecordCell from '@/components/tables/SingleRecordCell'
import MultiRecordCell from '@/components/tables/MultiRecordCell'
import CardCell from '@/components/tables/anime/CardCell'

export default {
  name: 'AnimeTable',
  components: {
    'table-header': TableHeader,
    'single-record-cell': SingleRecordCell,
    'multi-record-cell': MultiRecordCell,
    'card-cell': CardCell
  },
  props: ['inputData', 'avatarMode', 'counter', 'animeData'],
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
          seiyuu: [{
            entry: {
              name: this.inputData[i].seiyuu.name,
              image_url: this.inputData[i].seiyuu.image_url,
              url: this.inputData[i].seiyuu.url
            }
          }],
          roles: []
        })
        for (var l = 0; l < this.animeData.length; l++) {
          this.tableData[this.tableData.length - 1].roles.push({
            anime: this.animeData[l].title,
            characters: [{
              entry: this.inputData[i].roles[l].character
            }]
          })
        }
      }

      console.log(this.tableData)

      this.headers.push({
        text: 'Seiyuu',
        align: 'left',
        value: 'seiyuu.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.animeData.length; headerIndex++) {
        this.headers.push({
          text: this.animeData[headerIndex].title,
          value: this.animeData[headerIndex].title,
          image: this.animeData[headerIndex].image_url})
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

<style>

</style>
