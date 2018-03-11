<template>
<v-flex xs12>
<v-card >
    <v-container fluid grid-list-lg>
    <v-layout row>
        <v-flex xs4>
        <v-card-media
            :src="item.image_url"
            height="70px"
            contain
        ></v-card-media>
        </v-flex>
        <v-flex xs8>
        <div class="title">{{ item.name }}</div>
        </v-flex>
    </v-layout>
    </v-container>
</v-card>
</v-flex>
</template>

<script>
export default {
  props: {
    item: { required: true },
    searchText: { required: true }
  }
}
</script>
