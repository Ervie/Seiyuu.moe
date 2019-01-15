<template>
  <div>
    <v-container hidden-sm-and-down>
      <v-data-table :headers="headers" :items="tableData" hide-actions class="elevation-1">
        <template slot="headerCell" slot-scope="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" :avatarMode="avatarMode" />
        </template>
        <template slot="items" slot-scope="props">
          <td>
            <multi-record-cell :avatarMode="avatarMode" :items="props.item.seiyuu" />
          </td>
          <td v-for="role in props.item.roles" :key="role.anime">
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
  props: ['charactersData', 'avatarMode', 'counter', 'groupBySeiyuu', 'animeData'],
  data () {
    return {
      headers: [],
      tableData: []
    }
  },
  methods: {
    selectComputeMethod () {
      if (!this.groupBySeiyuu) {
        this.computeResultsSimple()
      } else {
        this.computeResultsSeiyuu()
      }
    },
    computeResultsSimple () {
      this.tableData = []
      this.headers = []

      for (var i = 0; i < this.charactersData.length; i++) {
        this.tableData.push({
          seiyuu: [{
            entry: {
              name: this.charactersData[i].seiyuu.name,
              image_url: this.charactersData[i].seiyuu.image_url,
              url: this.charactersData[i].seiyuu.url
            }
          }],
          roles: []
        })
        for (var l = 0; l < this.charactersData.length; l++) {
          this.tableData[this.tableData.length - 1].roles.push({
            anime: this.charactersData[l].title,
            characters: [{
              entry: this.charactersData[i].roles[l].character
            }]
          })
        }
      }

      this.headers.push({
        text: 'Seiyuu',
        align: 'left',
        value: 'seiyuu[0].entry.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.animeData.length; headerIndex++) {
        this.headers.push({
          text: this.animeData[headerIndex].title,
          value: 'roles[' + headerIndex + '].characters[0].entry.name',
          image: this.animeData[headerIndex].image_url})
      }
      this.showTables = true
    },
    computeResultsSeiyuu () {
      this.tableData = []
      this.headers = []
      var intersectSeiyuu = []
      var characterIndex = -1
      var seiyuuIndex = -1

      for (var i = 0; i < this.charactersData.length; i++) {
        if (intersectSeiyuu.length > 0) {
          seiyuuIndex = intersectSeiyuu.map(x => x.seiyuu[0].entry.name).indexOf(this.charactersData[i].seiyuu.name)
        }
        if (seiyuuIndex === -1) {
          intersectSeiyuu.push({
            seiyuu: [{
              entry: {
                name: this.charactersData[i].seiyuu.name,
                image_url: this.charactersData[i].seiyuu.image_url,
                url: this.charactersData[i].seiyuu.url
              }
            }],
            roles: []
          })
          for (var j = 0; j < this.charactersData[i].roles.length; j++) {
            intersectSeiyuu[intersectSeiyuu.length - 1].roles.push({
              anime: this.charactersData[i].roles[j].anime,
              characters: [{
                entry: this.charactersData[i].roles[j].character
              }]
            })
          }
        } else {
          for (var l = 0; l < this.animeData.length; l++) {
            console.log(intersectSeiyuu[seiyuuIndex])
            console.log(l)
            characterIndex = intersectSeiyuu[seiyuuIndex].roles[l].characters.map(x => x.entry.mal_id).indexOf(this.charactersData[i].roles[l].character.mal_id)
            if (characterIndex === -1) {
              intersectSeiyuu[seiyuuIndex].roles[l].characters.push({
                entry: this.charactersData[i].roles[l].character
              })
            }
          }
        }
      }

      this.tableData = intersectSeiyuu
      this.headers.push({
        text: 'Seiyuu',
        align: 'left',
        value: 'seiyuu[0].entry.name',
        image: ''
      })
      for (var headerIndex = 0; headerIndex < this.animeData.length; headerIndex++) {
        this.headers.push({
          text: this.animeData[headerIndex].title,
          value: 'roles[' + headerIndex + '].characters.length',
          image: this.animeData[headerIndex].image_url})
      }
      this.showTables = true
    }
  },
  watch: {
    counter: {
      handler: 'selectComputeMethod',
      immediate: true
    },
    groupBySeiyuu: {
      handler: 'selectComputeMethod',
      immediate: false
    }
  }
}
</script>

<style>

</style>
