<template>
  <v-card dark width="180px" class="seiyuuCard">
    <v-card-media :src="photoUrl" height="280px"></v-card-media>
    <v-card-text style="font-weight: bold"> {{ seiyuuName }}</v-card-text>
    </v-card>
</template>

<script>
export default {
  name: 'SeiyuuCard',
  props: ['seiyuuName', 'photoUrl']
}
</script>

<style>
.seiyuuCard {
    border: 1px solid rgb(20, 3, 95);
}
</style>
