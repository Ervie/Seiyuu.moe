<template>
  <div>
    <v-container hidden-sm-and-down>
      <v-data-table :headers="headers" :items="tableData" hide-actions class="elevation-1">
        <template slot="headerCell" slot-scope="props">
          <table-header :imageUrl="props.header.image" :text="props.header.text" :avatarMode="avatarMode" />
        </template>
        <template slot="items" slot-scope="props">
          <td>
            <multi-record-cell :avatarMode="avatarMode" :items="props.item.anime" />
          </td>
          <td v-for="role in props.item.roles" :key="role.seiyuu">
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
import decode from 'decode-html'
import TableHeader from '@/components/shared/tables/TableHeader'
import MultiRecordCell from '@/components/shared/tables/MultiRecordCell'
import CardCell from '@/components/shared/tables/seiyuu/CardCell'

export default {
  name: 'SeiyuuTable',
  components: {
    'table-header': TableHeader,
    'multi-record-cell': MultiRecordCell,
    'card-cell': CardCell
  },
  props: ['inputData', 'avatarMode', 'counter', 'seiyuuData', 'groupingMode'],
  data () {
    return {
      headers: [],
      tableData: []
    }
  },
  methods: {
    selectComputeMethod () {
      switch (this.groupingMode) {
        case 'None':
          this.computeResultsSimple()
          break
        case 'Series':
          this.computeResultsSeries()
          break
        case 'Characters':
          this.computeResultsCharacters()
          break
        case 'SeriesCharacters':
          this.computeResultsSeriesCharacters()
          break
      }
    },
    computeResultsSimple () {
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
    },
    computeResultsSeries () {
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
    },
    computeResultsCharacters () {
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
            roles: [],
            combinationCode: currentCombinationCode
          })
          for (var l = 0; l < this.seiyuuData.length; l++) {
            this.tableData[this.tableData.length - 1].roles.push({
              seiyuu: this.inputData[i].roles[l].seiyuu,
              characters: [{
                entry: this.inputData[i].roles[l].character
              }]
            })
          }
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
          value: 'roles[' + headerIndex + '].characters[0].entry.name',
          image: this.seiyuuData[headerIndex].image_url})
      }
      this.showTables = true
    },
    computeResultsSeriesCharacters () {
      this.tableData = []
      this.headers = []
      var animeIndex = -1
      var charactersIndex = -1
      var franchiseIndex = -1

      for (var i = 0; i < this.inputData.length; i++) {
        animeIndex = -1
        charactersIndex = -1
        if (this.tableData.length > 0) {
          for (var j = 0; j < this.tableData.length && animeIndex === -1; j++) {
            animeIndex = this.tableData[j].anime.map(x => x.entry.mal_id).indexOf(this.inputData[i].anime.mal_id)
            franchiseIndex = j
          }
          if (animeIndex === -1) {
            for (j = 0; j < this.tableData.length; j++) {
              for (var k = 0; k < this.seiyuuData.length && charactersIndex === -1; k++) {
                charactersIndex = this.tableData[j].roles[k].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[k].character.mal_id)
                franchiseIndex = j
              }
            }
          }
        }
        if (charactersIndex === -1 && animeIndex === -1) {
          this.tableData.push({
            anime: [{
              entry: {
                name: decode(this.inputData[i].anime.name),
                image_url: this.inputData[i].anime.image_url,
                url: this.inputData[i].anime.url,
                mal_id: this.inputData[i].anime.mal_id
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
        } else if (animeIndex === -1) {
          this.tableData[franchiseIndex].anime.push({
            entry: {
              name: decode(this.inputData[i].anime.name),
              image_url: this.inputData[i].anime.image_url,
              url: this.inputData[i].anime.url,
              mal_id: this.inputData[i].anime.mal_id
            }
          })
        } else {
          for (var seiyuuIndex = 0; seiyuuIndex < this.seiyuuData.length; seiyuuIndex++) {
            var roleIndex = this.tableData[franchiseIndex].roles[seiyuuIndex].characters.map(x => x.entry.mal_id).indexOf(this.inputData[i].roles[seiyuuIndex].character.mal_id)
            if (roleIndex === -1) {
              this.tableData[franchiseIndex].roles[seiyuuIndex].characters.push({ entry: this.inputData[i].roles[seiyuuIndex].character })
            }
          }
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
      handler: 'selectComputeMethod',
      immediate: true
    },
    groupingMode: {
      handler: 'selectComputeMethod',
      immediate: false
    }
  }
}
</script>

<style>

</style>
