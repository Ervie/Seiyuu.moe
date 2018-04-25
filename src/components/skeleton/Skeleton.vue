<template>
<div>
    <v-toolbar app fixed clipped-left class="toolbar" color="primary" >
      <v-toolbar-side-icon @click="drawer = !drawer"/>
      <v-toolbar-title><h1 class="styledHeader">Seiyuu.Moe</h1></v-toolbar-title>
    </v-toolbar>
    <v-navigation-drawer
      v-model="drawer"
      app
      fixed clipped>
      <v-list>
        <v-list-tile
        v-for="(tile, x) in tilesList"
        v-bind:key="x"
        router :to="tile.link">
            <v-list-tile-action>
              <v-icon medium>{{tile.icon}}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>{{tile.title}}</v-list-tile-title>
            </v-list-tile-content>
        </v-list-tile>
      </v-list>
  </v-navigation-drawer>
</div>
</template>

<script>

export default {
  name: 'Skeleton',
  data () {
    return {
      drawer: false,
      tilesList: [
        {icon: 'fa-users', title: 'Search Seiyuu', link: '/'},
        {icon: 'fa-info-circle', title: 'About', link: '/About'},
        {icon: 'fa-github', title: 'Repository', link: 'https://github.com/Ervie/Seiyuu.Moe'}
      ]
    }
  }
}
</script>

<style scoped></style>
