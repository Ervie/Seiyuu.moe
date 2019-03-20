<template>
  <v-container fluid>
    <v-layout wrap align-center justify-center fill-height>
        <v-flex xs6>
          <v-layout align-center justify-center fill-height row wrap>
            <v-card
              v-for="entry in mainColumnItems" :key="entry.malId"
              flat
              tile
              class="tableCard">
              <a :href="entry.url" target="_blank">
                  <v-img
                  height="225px"
                  aspect-ratio="1.5"
                  :src="pathToImage(changeUrlToOriginalSize(entry.imageUrl))"
                  ></v-img>
              </a>
              <v-card-text class="caption">{{ swapNameSurname(decodeHtml(entry.name)) }}</v-card-text>
            </v-card>
          </v-layout>
        </v-flex>
        <v-flex xs6>
          <v-layout align-center justify-center fill-height row wrap>
            <v-card
            v-for="entry in flattenedData" :key="entry.id"
            flat
            tile
            class="tableCard">
              <a :href="entry.url" target="_blank">
                  <v-img
                  height="225px"
                  aspect-ratio="1.5"
                  :src="pathToImage(changeUrlToOriginalSize(entry.imageUrl))"
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
      this.subColumnsItems.forEach((element, i) => {
        element.characters.forEach(character => {
          flattened.push({
            extraData: this.tableType === "Anime" ? element.anime.title : element.seiyuu.name,
            url: character.url,
            malId: character.malId,
            id: i + 'm' + character.malId,
            imageUrl: character.imageUrl,
            characterName: character.name
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
