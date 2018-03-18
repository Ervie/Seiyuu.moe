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
            <v-container fluid grid-list-xs>
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
  props: ['inputData', 'avatarMode', 'counter'],
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
      var foundAnimeIndex = -1
      var foundSeiyuuIndex = -1

      for (var seiyuuIndex = 0; seiyuuIndex < this.seiyuuRoles.length; seiyuuIndex++) {
        for (var animeIndex = 0; animeIndex < this.seiyuuRoles[seiyuuIndex].length; animeIndex++) {
          foundAnimeIndex = intersectAnime.map(x => x.anime.mal_id).indexOf(this.seiyuuRoles[seiyuuIndex][animeIndex].anime.mal_id)
          if (foundAnimeIndex === -1) {
            intersectAnime.push({
              anime: this.seiyuuRoles[seiyuuIndex][animeIndex].anime,
              roles: [{
                seiyuu: this.inputData[seiyuuIndex].name,
                characters: [{
                  character: this.seiyuuRoles[seiyuuIndex][animeIndex].character
                }]
              }]
            })
          } else {
            foundSeiyuuIndex = intersectAnime[foundAnimeIndex].roles.map(x => x.seiyuu).indexOf(this.inputData[seiyuuIndex].name)
            if (foundSeiyuuIndex === -1) {
              intersectAnime[foundAnimeIndex].roles.push({
                seiyuu: this.inputData[seiyuuIndex].name,
                characters: [{
                  character: this.seiyuuRoles[seiyuuIndex][animeIndex].character
                }]
              })
            } else {
              intersectAnime[foundAnimeIndex].roles[foundSeiyuuIndex].characters.push({
                character: this.seiyuuRoles[seiyuuIndex][animeIndex].character
              })
            }
          }
        }
      }

      intersectAnime = intersectAnime.filter(x => x.roles.length === this.inputData.length)

      console.log(intersectAnime)
      // for (var seiyuuIndex = 1; seiyuuIndex < this.seiyuuRoles.length; seiyuuIndex++) {
      //   for (var animeIndex = 0; animeIndex < intersectAnime.length; animeIndex++) {
      //     var roleIndex = this.seiyuuRoles[seiyuuIndex].map(x => x.anime.mal_id).indexOf(intersectAnime[animeIndex].anime.mal_id)
      //     if (roleIndex === -1) {
      //       intersectAnime.splice(animeIndex, 1)
      //       animeIndex--
      //     } else {
      //       helperIndex = intersectAnime[animeIndex].roles.map(x => x.seiyuu).indexOf(this.inputData[seiyuuIndex].name)
      //       if (helperIndex === -1) {
      //         intersectAnime[animeIndex].roles.push({
      //           seiyuu: this.inputData[seiyuuIndex].name,
      //           characters: [{
      //             character: this.seiyuuRoles[seiyuuIndex][roleIndex].character
      //           }]
      //         })
      //       } else {
      //         console.log('Inserting ' + this.seiyuuRoles[seiyuuIndex][roleIndex].character.name)
      //         intersectAnime[animeIndex].roles[helperIndex].characters.push({
      //           character: this.seiyuuRoles[seiyuuIndex][roleIndex].character
      //         })
      //       }
      //     }
      //   }
      // }
      for (var i = 0; i < intersectAnime.length; i++) {
        this.tableData.push({
          anime: intersectAnime[i].anime.name,
          animeImg: intersectAnime[i].anime.image_url,
          animeUrl: intersectAnime[i].anime.url,
          roles: intersectAnime[i].roles
        })
      }

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime'
      })
      for (var headerIndex = 0; headerIndex < this.inputData.length; headerIndex++) {
        this.headers.push({
          text: this.inputData[headerIndex].name,
          value: 'roles[' + headerIndex + '].length',
          image: this.inputData[headerIndex].image_url})
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
  computed: {
    seiyuuRoles () {
      return this.inputData.map(x => x.voice_acting_role)
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
