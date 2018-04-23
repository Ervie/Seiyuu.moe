<template>
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
            <v-icon>{{tile.icon}}</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>{{tile.title}}</v-list-tile-title>
          </v-list-tile-content>
      </v-list-tile>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
export default {
  name: 'Drawer',
  props: ['drawerOn'],
  data () {
    return {
      drawer: true,
      tilesList: [
        {icon: 'people', title: 'Search Seiyuu', link: '/#'},
        {icon: 'info_outline', title: 'About', link: '/About'}
      ]
    }
  },
  methods: {
    toggleDrawer () {
      this.drawer = this.drawerOn
    }
  },
  watch: {
    drawerOn: {
      handler: 'toggleDrawer',
      immediate: false
    }
  }
}
</script>

<style>
  #drawer a {
    text-decoration: none;
  }
</style>
