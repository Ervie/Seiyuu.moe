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
                <div v-if="props.header.value" class="av">
                <v-tooltip left>
                    <img :src="pathToImage(props.header.value)" slot="activator">
                <span>{{props.header.text}}</span>
                </v-tooltip>
                </div>
                <div v-else>{{props.header.text}}</div>
            </template>
            <template slot="items" slot-scope="props">
            <td>
                <v-container fluid grid-list-lg>
                <v-layout row>
                  <v-flex xs9 justify-center="">
                    <v-card-text class="headline">{{ props.item.anime }}</v-card-text>
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
            <td class="text-xs-right">
                <v-container fluid grid-list-lg>
                <v-layout row>
                  <v-flex xs9 align-content-center>
                    <v-card-text class="headline">{{ props.item.firstSeiyuuCharacterName }}</v-card-text>
                  </v-flex>
                  <v-flex xs3>
                    <v-card-media
                      height="70px"
                      :src="props.item.firstSeiyuuCharacterUrl"
                      contain
                    ></v-card-media>
                  </v-flex>
                </v-layout>
              </v-container>
            </td>
            <td class="text-xs-right">
                <v-container fluid grid-list-lg>
                <v-layout row>
                  <v-flex xs9 align-content-center>
                    <v-card-text class="headline">{{ props.item.secondSeiyuuCharacterName }}</v-card-text>
                  </v-flex>
                  <v-flex xs3>
                    <v-card-media
                      height="70px"
                      :src="props.item.secondSeiyuuCharacterUrl"
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
  methods: {
    showResults () {
      var firstSeiyuuRoles = this.inputData[0].voice_acting_role
      var secondSeiyuuRoles = this.inputData[1].voice_acting_role

      this.tableData = []
      this.headers = []

      for (var i = 0; i < firstSeiyuuRoles.length; i++) {
        for (var j = 0; j < secondSeiyuuRoles.length; j++) {
          if (firstSeiyuuRoles[i].anime.mal_id === secondSeiyuuRoles[j].anime.mal_id) {
            this.tableData.push({
              anime: firstSeiyuuRoles[i].anime.name,
              animeImg: firstSeiyuuRoles[i].anime.image_url,
              firstSeiyuuCharacterName: firstSeiyuuRoles[i].character.name,
              firstSeiyuuCharacterUrl: firstSeiyuuRoles[i].character.image_url,
              secondSeiyuuCharacterName: secondSeiyuuRoles[j].character.name,
              secondSeiyuuCharacterUrl: secondSeiyuuRoles[j].character.image_url
            })
          }
        }
      }
      this.headers.push({
        text: 'Title',
        align: 'left',
        sortable: false,
        value: ''
      })
      this.headers.push({ text: this.inputData[0].name, value: this.inputData[0].image_url })
      this.headers.push({ text: this.inputData[1].name, value: this.inputData[1].image_url })
      this.showTable = true
    },
    pathToImage (initialPath) {
      if (initialPath) {
        return initialPath
      } else {
        return 'static/questionMark.png'
      }
    }
  }
}
</script>

<style>
div.av {
    height: 50px;
    width: 200px;
    overflow: hidden;
}

div.av img {
    height: 350px;
    width: 100%;
    margin: -70px 0 0 0;
}

miniav {
    height: 70px;
    width: 45px
}
</style>
