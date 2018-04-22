<template>
<div>
    <v-toolbar class="toolbar" color="primary" >
      <v-toolbar-side-icon @click="$emit('toggledDrawer')"/>
      <v-toolbar-title><h1>Seiyuu.Moe</h1></v-toolbar-title>
      <v-spacer></v-spacer>
      <v-toolbar-items>
          <v-btn class="hidden-xs-only" flat @click="aboutDialog = true">
              About
          </v-btn>
      </v-toolbar-items>
    </v-toolbar>

    <v-dialog v-model="aboutDialog" max-width="400">
      <v-card>
        <v-card-title class="headline" style="font-weight: bold">Author: Bartłomiej Buchała (Ervie)</v-card-title>
        <v-card-media src="static/tanya.png" height="350px"></v-card-media>
        <v-card-text>
          <p>This website is created as a hobby, non-profit project for general use.</p>
          <p>Source code can be found <a href="https://github.com/Ervie/Seiyuu.Moe">here.</a></p>
          <p>Have in mind, that website is in its alpha stage and still under development. Suggestion or bugs can be issued via Github <a href="https://github.com/Ervie/Seiyuu.moe/issues/new>">here.</a></p>
        </v-card-text>
      </v-card>
    </v-dialog>
</div>
</template>

<script>

export default {
  name: 'Toolbar',
  data () {
    return {
      aboutDialog: false
    }
  }
}
</script>

<style scoped></style>
