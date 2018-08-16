<template>
  <v-card v-if="animeData">
    <v-card-text style="font-weight: bold"> {{ animeData.title }}</v-card-text>
    <v-card-media :src="pathToImage" :height="avatarHeight" v-on:click="showDialog = true" hidden-sm-and-down></v-card-media>
    <v-card-actions>
      <v-btn icon value="removeAnime" v-on:click="removeAnime()">
        <v-icon color="red">delete</v-icon>
      </v-btn>
      <v-spacer></v-spacer>
      <v-btn icon value="showDetails" v-on:click="showDialog = true">
        <v-icon color="secondary">info</v-icon>
      </v-btn>
    </v-card-actions>
    <!--Details card-->
    <v-dialog v-model="showDialog" max-width="700">
      <v-flex xs12>
            <v-card color="primary" class="white--text">
              <v-container fluid grid-list-lg>
                <v-layout row>
                  <v-flex xs4>
                    <v-card-media
                      :src="pathToImage"
                      height="350px"
                      contain
                    ></v-card-media>
                  </v-flex>
                  <v-flex xs8>
                    <div>
                      <div class="headline">{{ animeData.title }}</div>
                      <v-card-text>
                        <p class="text-sm-left">
                          <b>Original title:</b> {{ animeData.title_japanese }}
                        </p>
                        <p class="text-sm-left">
                          <b>Aired:</b> {{ animeData.aired_string }}
                        </p>
                        <p class="text-sm-left">
                          <b>Type:</b> {{ animeData.type }}
                        </p>
                        <p class="text-sm-left">
                          <b>Status:</b> {{ animeData.status }}
                        </p>
                        <p class="text-sm-left">
                          <b>Score:</b> {{ animeData.score }}
                        </p>
                        <p class="text-sm-left white-space-pre">
                          <b>Synopsis:</b> {{ decodeHtml(moreDetails) }}
                        </p>
                      </v-card-text>
                    </div>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-card>
          </v-flex>
    </v-dialog>
  </v-card>
</template>

<script>
export default {
  name: 'AnimeCard',
  props: ['animeData', 'cardId', 'avatarHeight'],
  data () {
    return {
      showDialog: false
    }
  },
  methods: {
    removeAnime: function () {
      this.$emit('animeRemoved', this.cardId)
    }
  },
  computed: {
    pathToImage () {
      if (this.animeData.image_url) {
        return this.animeData.image_url
      } else {
        return 'static/questionMark.png'
      }
    },
    moreDetails () {
      var detailsToEncode = String(this.animeData.synopsis).replace(/\\n/g, '\n')
      return detailsToEncode.replace(/\\n/g, '<br/>')
    }
  }
}
</script>

<style>

.white-space-pre {
    white-space: pre-wrap;
}
</style>
