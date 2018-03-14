<template>
    <v-layout>
      <v-flex>
        <div>
          <v-btn raised large color="error" v-on:click="resetList" :disabled="inputData.length < 1">Reset</v-btn>
          <v-btn depressed large color="success" v-on:click="showResults" :disabled="inputData.length < 2">Compare</v-btn>
        </div>
        <v-data-table
            v-if="showTable"
            :headers="headers"
            :items="tableData"
            hide-actions
            class="elevation-1">
            <template slot="headerCell" slot-scope="props" >
            <!-- Avatar mode headers -->
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
                    <v-card-media
                      height="70px"
                      :src="props.item.animeImg"
                      contain
                    ></v-card-media>
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
            <td class="text-xs-right" v-for="character in props.item.roles" :key="character.mal_id">
              <!-- Normal mode character records -->
              <v-container fluid grid-list-xs v-if="!avatarMode">
                <v-layout row>
                  <v-flex xs8 align-content-center>
                    <v-card-text class="subheading">{{ character.character.name }}</v-card-text>
                  </v-flex>
                  <v-flex xs4>
                    <v-card-media
                      height="70px"
                      :src="character.character.image_url"
                      contain
                    ></v-card-media>
                  </v-flex>
                </v-layout>
              </v-container>
              <!-- Avatar mode character records -->
              <v-container fluid grid-list-xs v-else>
                <v-layout row v-if="character.character.image_url">
                  <v-flex xs12 justify-center>
                    <v-tooltip bottom>
                    <img :src="character.character.image_url" slot="activator" class="miniav">
                    <span>{{character.character.name}}</span>
                    </v-tooltip>
                  </v-flex>
                </v-layout>
                <v-layout v-else row>
                  <v-flex md1 justify-center>
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
      tableData: [],
      avatarMode: false,
      windowWidth: 0
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
          value: 'roles[' + headerIndex + '].character.name',
          image: this.inputData[headerIndex].image_url})
      }

      this.handleResize(this.windowWidth)
      this.showTable = true
    },
    pathToImage (initialPath) {
      if (initialPath) {
        return initialPath
      } else {
        return 'static/questionMark.png'
      }
    },
    resetList () {
      this.$emit('resetList')
    },
    handleResize (windowWidth) {
      if (windowWidth / this.inputData.length < 400) {
        this.avatarMode = true
      } else {
        this.avatarMode = false
      }
    }
  },
  watch: {
    inputData: function (newVal, oldVal) {
      if (this.inputData.length === 0) {
        this.showTable = false
      }
    },
    windowWidth: function (newWidth, oldWidth) {
      this.handleResize(newWidth)
    }
  },
  mounted () {
    let that = this
    this.$nextTick(function () {
      window.addEventListener('resize', function (e) {
        that.windowWidth = window.innerWidth
      })
    })
  },
  beforeDestroy: function () {
    window.removeEventListener('resize', this.handleResize)
  }
}
</script>

<style>
img.miniav {
    height: 98px;
    width: 63px;
}

img.av {
    height: 140px;
    width: 90px;
}
</style>
