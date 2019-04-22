<template>
  <v-card v-if="animeData">
    <v-card-text class="card-title"> {{ animeData.title }}</v-card-text>
    <v-img :src="pathToImage(this.animeData.imageUrl)" :height="avatarHeight" v-on:click="showDialog = true" hidden-sm-and-down></v-img>
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
                    <v-img
                      :src="pathToImage(this.animeData.imageUrl)"
                      height="350px"
                      contain
                    ></v-img>
                  </v-flex>
                  <v-flex xs8>
                    <div>
                      <div class="headline">{{ animeData.title }}</div>
                      <v-card-text>
                        <p class="text-sm-left">
                          <b>Original title:</b> {{ animeData.japaneseTitle }}
                        </p>
                         <div class="text-sm-left alternativeTitles">
                          <b>Other titles:</b> <p class="alternativeTitle" v-for="(title, i) in alternativeTitles" :key="i"> {{ title }} </p>
                        </div>
                        <p class="text-sm-left">
                          <b>Premiered:</b> {{ formatDate(animeData.airingDate) }}
                        </p>
                        <p class="text-sm-left">
                          <b>Type:</b> {{ animeData.type }}
                        </p>
                        <p class="text-sm-left">
                          <b>Status:</b> {{ animeData.status }}
                        </p>
                        <p class="text-sm-left">
                          <b>Season:</b> {{ animeData.season }}
                        </p>
                        <p class="text-sm-left white-space-pre">
                          <b>Synopsis:</b> {{ moreDetails }}
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
  props: {
    animeData: {
      type: Object,
      required: false
    },
    cardId: {
      type: Number,
      required: true
    },
    avatarHeight : {
      type: Number,
      required: false
    }
  },
  data () {
    return {
      showDialog: false
    }
  },
  methods: {
    removeAnime: function () {
      this.$emit('animeRemoved', this.cardId);
    }
  },
  computed: {
    moreDetails () {
      if (this.animeData.about != null) {
        var detailsToEncode = String(this.animeData.about).replace(/\\n/g, '\n');
        return this.decodeHtml(detailsToEncode.replace(/\\n/g, '<br/>'));
      } else {
        return 'Nothing yet.';
      }
    },
    alternativeTitles () {
      if (this.animeData && this.animeData.titleSynonyms && this.animeData.titleSynonyms.length > 0) {
        return this.animeData.titleSynonyms.split(';');
      } else {
        return ['None'];
      }
    }
  }
}
</script>

<style>

.white-space-pre {
    white-space: pre-wrap;
}

.alternativeTitles {
  margin-bottom: 16px;
}

.alternativeTitle {
  margin: 0 16px;
}
</style>
