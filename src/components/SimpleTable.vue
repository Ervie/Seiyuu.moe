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
        <td class="text-xs-right" v-for="character in props.item.roles" :key="character.mal_id">
            <!-- Normal mode character records -->
            <v-container fluid grid-list-xs v-if="!avatarMode">
            <v-layout row>
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
            <!-- Avatar mode character records -->
            <v-container fluid grid-list-xs v-else>
            <v-layout row v-if="character.character.image_url">
                <v-flex xs12 justify-center>
                <a :href="character.character.url" target="_blank">
                    <v-tooltip bottom>
                    <img :src="character.character.image_url" slot="activator" class="miniav">
                    <span>{{character.character.name}}</span>
                    </v-tooltip>
                </a>
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
</template>

<script>
import decode from 'decode-html'

export default {
  name: 'SimpleTable',
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
          anime: decode(this.inputData[i].anime.name),
          animeImg: this.inputData[i].anime.image_url,
          animeUrl: this.inputData[i].anime.url,
          roles: this.inputData[i].roles
        })
      }

      this.headers.push({
        text: 'Anime',
        align: 'left',
        value: 'anime'
      })
      for (var headerIndex = 0; headerIndex < this.seiyuuData.length; headerIndex++) {
        this.headers.push({
          text: this.seiyuuData[headerIndex].name,
          value: 'roles[' + headerIndex + '].character.name',
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
