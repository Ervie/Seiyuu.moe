<template>
    <v-data-table
        :headers="headers"
        :items="tableData"
        hide-actions
        class="elevation-1">
        <template slot="headerCell" slot-scope="props" >
        <!-- Headers -->
            <v-container grid-list-xs fluid>
            <v-layout row v-if="props.header.image">
                <v-flex xs6  offset-xs3 justify-center>
                <v-tooltip bottom>
                <img :src="props.header.image" slot="activator" class="av">
                <span>{{props.header.text}}</span>
                </v-tooltip>
                </v-flex>
            </v-layout>
            <v-layout v-else row>
                <v-flex md1 justify-center>
                <v-card-text class="display-1">{{props.header.text}}</v-card-text>
                </v-flex>
            </v-layout>
            </v-container>
        </template>
        <template slot="items" slot-scope="props">
        <td>
            <!-- Normal mode anime records -->
            <v-container fluid grid-list-xs v-if="!avatarMode">
            <v-layout row>
                <v-flex xs8 justify-center>
                <v-card-text class="subheading">{{ props.item.anime }}</v-card-text>
                </v-flex>
                <v-flex xs4>
                <a :href="props.item.animeUrl" target="_blank">
                    <v-card-media
                    height="70px"
                    :src="props.item.animeImg"
                    contain
                    ></v-card-media>
                </a>
                </v-flex>
            </v-layout>
            </v-container>
            <!-- Avatar mode anime records -->
            <v-container fluid grid-list-xs v-else>
            <v-layout class="subheading" row>
                <v-flex xs12 justify-center>
                <v-card-text>{{props.item.anime}}</v-card-text>
                </v-flex>
            </v-layout>
            </v-container>
        </td>
        <td class="text-xs-right" v-for="role in props.item.roles" :key="role.seiyuu">
            <!-- Normal mode character records -->
            <v-container fluid grid-list-xs v-if="!avatarMode">
            <v-layout v-for="character in role.characters" :key="character.mal_id" row>
                <v-flex xs8 align-content-center>
                <v-card-text class="subheading">{{ character.character.name }}</v-card-text>
                </v-flex>
                <v-flex xs4>
                <a :href="character.character.url" target="_blank">
                    <v-card-media
                    height="70px"
                    :src="character.character.image_url"
                    contain
                    ></v-card-media>
                </a>
                </v-flex>
            </v-layout>
            </v-container>
            <v-container fluid grid-list-xs v-else>
              <v-layout v-for="character in role.characters" :key="character.mal_id" row>
                  <v-flex xs12 justify-center v-if="character.character.image_url">
                    <a :href="character.character.url" target="_blank">
                        <v-tooltip bottom>
                        <img :src="character.character.image_url" slot="activator" class="miniav">
                        <span>{{character.character.name}}</span>
                        </v-tooltip>
                    </a>
                  </v-flex>
                  <v-flex md1 justify-center v-else>
                    <v-card-text>{{character.character.name}}</v-card-text>
                  </v-flex>
              </v-layout>
            </v-container>
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
export default {
  name: 'SeriesTable',
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
          animeIndex = intersectAnime.map(x => x.anime).indexOf(this.inputData[i].anime.name)
        }
        if (animeIndex === -1) {
          intersectAnime.push({
            anime: this.inputData[i].anime.name,
            animeImg: this.inputData[i].anime.image_url,
            animeUrl: this.inputData[i].anime.url,
            roles: []
          })
          for (var j = 0; j < this.inputData[i].roles.length; j++) {
            intersectAnime[intersectAnime.length - 1].roles.push({
              seiyuu: this.inputData[i].roles[j].seiyuu,
              characters: [{
                character: this.inputData[i].roles[j].character
              }]
            })
          }
        } else {
          for (seiyuuIndex = 0; seiyuuIndex < this.seiyuuData.length; seiyuuIndex++) {
            roleIndex = intersectAnime[animeIndex].roles[seiyuuIndex].characters.map(x => x.character.mal_id).indexOf(this.inputData[i].roles[seiyuuIndex].character.mal_id)
            if (roleIndex === -1) {
              intersectAnime[animeIndex].roles[seiyuuIndex].characters.push({ character: this.inputData[i].roles[seiyuuIndex].character })
            }
          }
        }
      }

      this.tableData = intersectAnime
      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime'
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].characters.length',
          image: this.seiyuuData[headerIndex].image_url})
      }
      this.showTables = true
    },
    pathToImage (initialPath) {
      if (initialPath) {
        return initialPath
      } else {
        return 'static/questionMark.png'
      }
    }
  },
  watch: {
    counter (oldVal, newVal) {
      this.computeResults()
    }
  },
  created () {
    this.computeResults()
  }
}
</script>

<style scoped>

</style>
