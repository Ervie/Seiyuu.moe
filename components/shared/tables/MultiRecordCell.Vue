<template>
<v-container fluid >
    <v-layout wrap align-center justify-center fill-height>
        <v-flex :class="gridSize">
                <v-layout align-center justify-center fill-height row wrap>
                <div
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
                <v-card-text class="caption title">{{ swapNameSurname(decodeHtml(entry.entry.name)) }}</v-card-text>
                </div>
                    </v-layout>
        </v-flex>
        <v-flex :class="gridSize" v-for="role in subColumnsItems" :key="role.seiyuu">
                <v-layout align-center justify-center fill-height row wrap>
                <div
                v-for="entry in role.characters" :key="entry.mal_id"
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
                <v-card-text class="caption title">{{ swapNameSurname(decodeHtml(entry.entry.name)) }}</v-card-text>
                </div>
                    </v-layout>
        </v-flex>
    </v-layout>
</v-container>
</template>

<script>
export default {
  Name: 'MultiRecordCell',
  props: {
    mainColumnItems: {
      type: Array,
      required: true
    },
    subColumnsItems: {
      type: Array,
      required: true
    }
  },
  methods: {
    changeUrlToOriginalSize (inputString) {
      return inputString.replace('/r/23x32','')
    }
  },
  computed: {
    gridSize () {
      if (this.subColumnsItems.length > 5) {
        return 'xs2'
      } else if (this.subColumnsItems.length > 3) {
        return 'xs2'
      } else if (this.subColumnsItems.length > 2) {
        return 'xs3'
      } else {
        return 'xs4'
      }
    }
  }
}
</script>

<style scoped>
    .tableCard {
        /* display: inline-flex; */
        margin: 5px 2px;
        height: 270px;
        width: 175px;
    }
</style>
