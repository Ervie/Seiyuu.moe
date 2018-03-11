<template>
    <v-layout>
      <v-flex>
        <div>
          <v-btn depressed large color="success" v-on:click="showResults" :disabled="inputData.length < 2">Compare</v-btn>
        </div>
        <v-data-table
            v-if="showTable"
            :headers="headers"
            :items="tableData"
            hide-actions
            class="elevation-1">
            <template slot="headerCell" slot-scope="props">
              <v-container fluid grid-list-lg>
                <v-layout v-if="props.header.image" row>
                  <v-flex xs9 align-content-center>
                    <v-card-text class="title">{{ props.header.text }}</v-card-text>
                  </v-flex>
                  <v-flex xs3>
                    <v-card-media
                      height="70px"
                      :src="props.header.image"
                      contain
                    ></v-card-media>
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
              <v-container fluid grid-list-lg>
                <v-layout row>
                  <v-flex xs9 justify-center>
                    <v-card-text class="title">{{ props.item.anime }}</v-card-text>
                  </v-flex>
                  <v-flex xs3>
                    <v-card-media
                      height="70px"
                      :src="props.item.animeImg"
                      contain
                    ></v-card-media>
                  </v-flex>
                </v-layout>
              </v-container>
            </td>
            <td class="text-xs-right" v-for="character in props.item.roles" :key="character.mal_id">
              <v-container fluid grid-list-lg>
                <v-layout row>
                  <v-flex xs9 align-content-center>
                    <v-card-text class="title">{{ character.character.name }}</v-card-text>
                  </v-flex>
                  <v-flex xs3>
                    <v-card-media
                      height="70px"
                      :src="character.character.image_url"
                      contain
                    ></v-card-media>
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
      </v-flex>
    </v-layout>
</template>

<script>
export default {
  name: 'RelationMatrix',
  props: ['inputData'],
  data () {
    return {
      showTable: false,
      headers: [],
      tableData: []
    }
  },
  computed: {
    seiyuuRoles () {
      return this.inputData.map(x => x.voice_acting_role)
    }
  },
  methods: {
    showResults () {
      this.tableData = []
      this.headers = []
      var intersectAnime = []

      for (var k = 0; k < this.seiyuuRoles[0].length; k++) {
        intersectAnime.push({
          anime: this.seiyuuRoles[0][k].anime,
          roles: [{
            seiyuu: this.inputData[0].name,
            character: this.seiyuuRoles[0][k].character
          }]
        })
      }

      for (var seiyuuIndex = 1; seiyuuIndex < this.seiyuuRoles.length; seiyuuIndex++) {
        for (var animeIndex = 0; animeIndex < intersectAnime.length; animeIndex++) {
          var roleIndex = this.seiyuuRoles[seiyuuIndex].map(x => x.anime.mal_id).indexOf(intersectAnime[animeIndex].anime.mal_id)
          if (roleIndex === -1) {
            intersectAnime.splice(animeIndex, 1)
            animeIndex--
          } else {
            intersectAnime[animeIndex].roles.push({
              seiyuu: this.inputData[seiyuuIndex].name,
              character: this.seiyuuRoles[seiyuuIndex][roleIndex].character
            })
          }
        }
      }

      for (var i = 0; i < intersectAnime.length; i++) {
        this.tableData.push({
          anime: intersectAnime[i].anime.name,
          animeImg: intersectAnime[i].anime.image_url,
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
          value: 'seiyuuCharacterName' + headerIndex,
          image: this.inputData[headerIndex].image_url})
      }

      this.showTable = true
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
    inputData: function (newVal, oldVal) {
      if (this.inputData.length === 0) {
        this.showTable = false
      }
    }
  }
}
</script>

<style>
div.av {
    height: 200px;
    width: 200px;
    overflow: hidden;
}

div.av img {
    height: 350px;
    width: 100%;
    margin: -30px 0 0 0;
}

miniav {
    height: 70px;
    width: 45px
}
</style>
