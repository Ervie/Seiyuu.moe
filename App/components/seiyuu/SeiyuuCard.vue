<template>
  <v-card v-if="seiyuuData">
    <v-card-text class="card-title subheading"> {{ seiyuuData.name }}</v-card-text>
    <v-img :src="pathToImage(this.seiyuuData.imageUrl)" :height="avatarHeight" v-on:click="showDialog = true" hidden-sm-and-down></v-img>
    <v-card-actions>
      <v-btn icon value="removeSeiyuu" v-on:click="removeSeiyuu()">
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
                      :src="pathToImage(this.seiyuuData.imageUrl)"
                      height="350px"
                      contain
                    ></v-img>
                  </v-flex>
                  <v-flex xs8>
                    <div>
                      <div class="headline">{{ seiyuuData.name }}</div>
                      <v-card-text>
                        <p class="text-sm-left">
                          <b>Given name:</b> {{ seiyuuData.japaneseName }}
                        </p>
                        <p class="text-sm-left">
                          <b>Birthday:</b> {{ formatDate(seiyuuData.birthday) }}
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
  props: {
    seiyuuData: {
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
    removeSeiyuu: function () {
      this.$emit('seiyuuRemoved', this.cardId)
    }
  },
  computed: {
    moreDetails () {
      if (this.seiyuuData.about != null) {
        var detailsToEncode = String(this.seiyuuData.about).replace(/\\n/g, '');
        return this.decodeHtml(detailsToEncode);
      } else {
        return "Nothing yet.";
      }
    }
  }
}
</script>

<style>

.white-space-pre {
    white-space: pre-wrap;
}
</style>
