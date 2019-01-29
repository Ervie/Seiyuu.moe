<template>
  <v-container fluid>
    <v-layout wrap align-center justify-center fill-height>
        <v-flex sm6>
          <v-layout align-center justify-center fill-height row wrap>
            <v-card
              v-for="entry in mainColumnItems" :key="entry.mal_id"
              flat
              tile
              class="tableCard">
              <a :href="entry.entry.url" target="_blank">
                  <v-img
                  height="225px"
                  aspect-ratio="1.5"
                  :src="changeUrlToOriginalSize(entry.entry.image_url)"
                  ></v-img>
              </a>
              <v-card-text class="caption">{{ swapNameSurname(decodeHtml(entry.entry.name)) }}</v-card-text>
            </v-card>
          </v-layout>
        </v-flex>
        <v-flex sm6 >
          <v-layout align-center justify-center fill-height row wrap>
                <v-card
                v-for="entry in flattenedData" :key="entry.mal_id"
                flat
                tile
                class="tableCard">
                  <a :href="entry.url" target="_blank">
                      <v-img
                      height="225px"
                      aspect-ratio="1.5"
                      :src="changeUrlToOriginalSize(entry.image_url)"
                      ></v-img>
                  </a>
                <div class="body-2">{{ swapNameSurname(decodeHtml(entry.characterName)) }}</div>
                <div class="body-2 accentedText">{{ decodeHtml(entry.extraData) }}</div>
                </v-card>
          </v-layout>
        </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  Name: 'ExpandedPanel',
  props: {
    mainColumnItems: {
      type: Array,
      required: true
    },
    subColumnsItems: {
      type: Array,
      required: true
    },
    tableType: {
      type: String,
      required: true
    }
  },
  computed: {
    flattenedData () {
      var flattened = [];
      this.subColumnsItems.forEach(element => {
        element.characters.forEach(character => {
          flattened.push({
            extraData: this.tableType === "Anime" ? element.anime : element.seiyuu,
            url: character.entry.url,
            mal_id: character.entry.mal_id,
            image_url: character.entry.image_url,
            characterName: character.entry.name
          });
        });
      });
      return flattened;
    }
  },
  methods: {
    changeUrlToOriginalSize (inputString) {
      return inputString.replace('/r/23x32','')
    }
  }
}
</script>

<style scoped>
    .tableCard {
        margin: 15px 5px;
        height: 270px;
        width: 175px;
    }
</style>
