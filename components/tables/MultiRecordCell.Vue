<template>
<v-container fluid grid-list-xs v-if="!avatarMode">
<v-layout v-for="entry in items" :key="entry.mal_id" row>
    <v-flex xs8 align-content-center>
    <v-card-text class="subheading">{{ swapNameSurname(decodeHtml(entry.entry.name)) }}</v-card-text>
    </v-flex>
    <v-flex xs4>
    <a :href="entry.entry.url" target="_blank">
        <v-img
        height="70px"
        :src="changeUrlToOriginalSize(entry.entry.image_url)"
        contain
        ></v-img>
    </a>
    </v-flex>
</v-layout>
</v-container>
<v-container fluid grid-list-xs v-else>
    <v-layout v-for="entry in items" :key="entry.mal_id" row>
        <v-flex xs12 justify-center v-if="entry.entry.image_url">
        <a :href="entry.entry.url" target="_blank">
            <v-tooltip bottom>
            <img :src="changeUrlToOriginalSize(entry.entry.image_url)" slot="activator" class="miniav">
            <span>{{swapNameSurname(decodeHtml(entry.entry.name))}}</span>
            </v-tooltip>
        </a>
        </v-flex>
        <v-flex md1 justify-center v-else>
        <v-card-text>{{swapNameSurname(decodeHtml(entry.entry.name))}}</v-card-text>
        </v-flex>
    </v-layout>
</v-container>
</template>

<script>
export default {
  Name: 'MultiRecordCell',
  props: ['avatarMode', 'items'],
  methods: {
    changeUrlToOriginalSize (inputString) {
      return inputString.replace('/r/23x32','')
    }
  }
}
</script>
