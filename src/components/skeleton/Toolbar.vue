<template>
<div>
    <v-toolbar app fixed clipped-left class="toolbar" color="primary" >
      <v-toolbar-side-icon @click="$emit('toggledDrawer')"/>
      <v-toolbar-title><h1 class="styledHeader">Seiyuu.Moe</h1></v-toolbar-title>
      <v-spacer></v-spacer>
      <v-toolbar-items>
          <v-btn class="hidden-xs-only" flat @click="aboutDialog = true">
              About
          </v-btn>
      </v-toolbar-items>
    </v-toolbar>
</div>
</template>

<script>

export default {
  name: 'Toolbar'
}
</script>

<style scoped></style>
