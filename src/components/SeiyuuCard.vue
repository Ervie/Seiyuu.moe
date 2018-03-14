<template>
  <v-card v-if="seiyuuData">
    <v-card-text style="font-weight: bold"> {{ seiyuuData.name }}</v-card-text>
    <v-card-media :src="pathToImage" height="280px" v-on:click="showDialog = true"  ></v-card-media>
    <v-card-actions>
      <v-btn icon value="removeSeiyuu" v-on:click="removeSeiyuu()">
        <v-icon color="red">delete</v-icon>
      </v-btn>
      <v-spacer></v-spacer>
      <v-btn icon value="showDetails" v-on:click="showDialog = true">
        <v-icon color="teal">info</v-icon>
      </v-btn>
    </v-card-actions>
    <!--Details card-->
    <v-dialog v-model="showDialog" max-width="700">
      <v-flex xs12>
            <v-card color="blue darken-4" class="white--text">
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
                      <div class="headline">{{ seiyuuData.name }}</div>
                      <v-card-text>
                        <p class="text-sm-left">
                          <b>Given name:</b> {{ seiyuuData.given_name }}
                        </p>
                        <p class="text-sm-left">
                          <b>Birthday:</b> {{ seiyuuData.birthday }}
                        </p>
                        <p class="text-sm-left white-space-pre">
                          <b>More:</b> {{ moreDetails }}
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
  name: 'SeiyuuCard',
  props: ['seiyuuData', 'cardId'],
  data () {
    return {
      showDialog: false
    }
  },
  methods: {
    removeSeiyuu: function () {
      this.$emit('seiyuuRemoved', this.cardId)
    }
  },
  computed: {
    pathToImage () {
      if (this.seiyuuData.image_url) {
        return this.seiyuuData.image_url
      } else {
        return 'static/questionMark.png'
      }
    },
    moreDetails () {
      var detailsToEncode = String(this.seiyuuData.more).replace(/\\n/g, '\n')
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
