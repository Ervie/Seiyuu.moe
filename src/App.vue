<template>
  <v-app id="v-app" dark>
    <toolbar @toggledDrawer="toggleDrawer"></toolbar>
    <drawer :drawerOn="drawerOn"></drawer>
    <v-content>
      <router-view id="router-view" />
    </v-content>
  </v-app>
</template>

<script>
import Toolbar from '@/components/skeleton/Toolbar.vue'
import Drawer from '@/components/skeleton/Drawer.vue'

export default {
  name: 'App',
  components: {
    'toolbar': Toolbar,
    'drawer': Drawer
  },
  data () {
    return {
      drawerOn: false
    }
  },
  methods: {
    toggleDrawer: function () {
      this.drawerOn = !this.drawerOn
    }
  }
}
</script>

<style>
#v-app {
  font-family: "OpenSans", Helvetica, Arial, sans-serif;
  text-align: center;
  color: #5a5a5a;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>
