<template>
    <v-container fluid grid-list-xs>
        <v-layout v-for="entry in items" :key="entry.malId" row>
            <v-flex xs8 align-content-center>
                <v-card-text class="subheading">{{ swapNameSurname(decodeHtml(entry.name)) }}</v-card-text>
            </v-flex>
            <v-flex xs4>
            <a :href="entry.url" target="_blank">
                <v-img
                height="70px"
                :src="changeUrlToOriginalSize(entry.imageUrl)"
                contain
                ></v-img>
            </a>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {
  Name: 'AvatarRecordCell',
  props: {
    items: {
      type: Array,
      required: true
    }
  },
  methods: {
    changeUrlToOriginalSize (inputString) {
      return inputString.replace('/r/23x32','')
    }
  }
}
</script>