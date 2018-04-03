<template>
    <v-data-table
        :headers="headers"
        :items="tableData"
        hide-actions
        class="elevation-1">
        <template slot="headerCell" slot-scope="props" >
          <table-header
              :imageUrl="props.header.image"
              :text="props.header.text"
          />
        </template>
        <template slot="items" slot-scope="props">
          <td class="text-xs-right">
              <multi-record-cell
              :avatarMode="avatarMode"
              :items="props.item.anime"
              />
          </td>
          <td class="text-xs-right" v-for="character in props.item.roles" :key="character.mal_id">
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
import MultiRecordCell from './cells/MultiRecordCell'

export default {
  name: 'CharactersTable',
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
      var combinationIndex = -1
      var currentCombinationCode = -1

      for (var i = 0; i < this.inputData.length; i++) {
        currentCombinationCode = this.combinationCode(this.inputData[i].roles.map(x => x.character.mal_id))
        if (this.tableData.length > 0) {
          combinationIndex = this.tableData.map(x => x.combinationCode).indexOf(currentCombinationCode)
        }
        if (combinationIndex === -1) {
          this.tableData.push({
            anime: [{
              entry: {
                name: decode(this.inputData[i].anime.name),
                image_url: this.inputData[i].anime.image_url,
                url: this.inputData[i].anime.url
              }
            }],
            roles: this.inputData[i].roles,
            combinationCode: currentCombinationCode
          })
        } else {
          this.tableData[combinationIndex].anime.push({
            entry: {
              name: decode(this.inputData[i].anime.name),
              image_url: this.inputData[i].anime.image_url,
              url: this.inputData[i].anime.url
            }
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
          value: 'roles[' + headerIndex + '].character.name',
          image: this.seiyuuData[headerIndex].image_url})
      }
      this.showTables = true
    },
    combinationCode (data) {
      var returnCode = ''
      for (var index = 0; index < data.length; index++) {
        returnCode += this.zeroFill(data[index], 7)
      }

      return returnCode
    },
    zeroFill (number, width) {
      width -= number.toString().length
      if (width > 0) {
        return new Array(width + (/\./.test(number) ? 2 : 1)).join('0') + number
      }
      return number + ''
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
