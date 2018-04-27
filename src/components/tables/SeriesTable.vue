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
  name: 'SeriesTable',
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
      var intersectAnime = []
      var animeIndex = -1
      var roleIndex = -1
      var seiyuuIndex = -1

      for (var i = 0; i < this.inputData.length; i++) {
        if (intersectAnime.length > 0) {
          animeIndex = intersectAnime.map(x => x.anime[0].entry.name).indexOf(this.inputData[i].anime.name)
        }
        if (animeIndex === -1) {
          intersectAnime.push({
            anime: [{
              entry: {
                name: decode(this.inputData[i].anime.name),
                image_url: this.inputData[i].anime.image_url,
                url: this.inputData[i].anime.url
              }
            }],
            roles: []
          })
          for (var j = 0; j < this.inputData[i].roles.length; j++) {
            intersectAnime[intersectAnime.length - 1].roles.push({
              seiyuu: this.inputData[i].roles[j].seiyuu,
              characters: [{
                entry: this.inputData[i].roles[j].character
              }]
            })
          }
        } else {
          for (seiyuuIndex = 0; seiyuuIndex < this.seiyuuData.length; seiyuuIndex++) {
            roleIndex = intersectAnime[animeIndex].roles[seiyuuIndex].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[seiyuuIndex].character.mal_id)
            if (roleIndex === -1) {
              intersectAnime[animeIndex].roles[seiyuuIndex].characters.push({ entry: this.inputData[i].roles[seiyuuIndex].character })
            }
          }
        }
      }

      this.tableData = intersectAnime
      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime[0].entry.name'
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].characters.length',
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
