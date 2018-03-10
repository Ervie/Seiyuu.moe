<template>
  <v-card width="180px" class="seiyuuCard">
    <v-card-text style="font-weight: bold"> {{ seiyuuData.name }}</v-card-text>
    <v-card-media :src="pathToImage" height="280px"></v-card-media>
    <v-card-actions>
      <v-btn icon value="removeSeiyuu" color="red" v-on:click="removeSeiyuu()">
        <v-icon>delete</v-icon>
      </v-btn>
      <v-spacer></v-spacer>
      <v-btn icon value="showDetails" color="teal">
        <v-icon>info</v-icon>
      </v-btn>
    </v-card-actions>
    </v-card>
</template>

<script>
export default {
  name: 'SeiyuuCard',
  props: ['seiyuuData', 'cardId'],
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
    }
  }
}
</script>

<style>
.seiyuuCard {
    border: 1px solid rgb(20, 3, 95);
}
</style>
