<template>
  <v-card width="180px" class="seiyuuCard">
    <v-card-text style="font-weight: bold"> {{ seiyuuName }}</v-card-text>
    <v-card-media :src="photoUrl" height="280px"></v-card-media>
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
  props: ['seiyuuName', 'photoUrl', 'seiyuuMalId', 'cardId'],
  methods: {
    removeSeiyuu: function () {
      console.log('Removing seiyuu with id ' + this.seiyuuMalId)
      this.$emit('seiyuuRemoved', this.cardId)
    }
  }
}
</script>

<style>
.seiyuuCard {
    border: 1px solid rgb(20, 3, 95);
}
</style>
