<template>
  <v-container fluid grid-list-xs v-if="!avatarMode">
    <v-layout row>
        <v-flex xs8 align-content-center>
        <v-card-text class="subheading">{{ decodeHtml(item.name) }}</v-card-text>
        </v-flex>
        <v-flex xs4>
        <a :href="item.url" target="_blank">
            <v-img
            height="70px"
            :src="item.image_url"
            contain
            ></v-img>
        </a>
        </v-flex>
    </v-layout>
  </v-container>
    <!-- Avatar mode - avatars -->
  <v-container fluid grid-list-xs v-else-if="!preferText">
    <v-layout row v-if="item.image_url">
        <v-flex xs12 justify-center>
        <a :href="item.url" target="_blank">
            <v-tooltip bottom>
            <img :src="item.image_url" slot="activator" class="miniav">
            <span>{{decodeHtml(item.name)}}</span>
            </v-tooltip>
        </a>
        </v-flex>
    </v-layout>
    <v-layout v-else row>
        <v-flex md1 justify-center>
        <v-card-text>{{decodeHtml(item.name)}}</v-card-text>
        </v-flex>
    </v-layout>
  </v-container>
    <!-- Avatar mode - text -->
  <v-container fluid grid-list-xs v-else>
    <v-layout class="subheading" row>
        <v-flex xs12 justify-center>
        <v-card-text>{{decodeHtml(item.name)}}</v-card-text>
        </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  Name: 'SingleRecordCell',
  props: ['avatarMode', 'item', 'preferText']
}
</script>
