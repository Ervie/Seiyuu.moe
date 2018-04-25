<template>
  <v-app id="v-app" dark>
    <skeleton></skeleton>
    <v-content>
      <router-view id="router-view" />
    </v-content>
  </v-app>
</template>

<script>
import Skeleton from '@/components/skeleton/Skeleton.vue'

export default {
  name: 'App',
  components: {
    'skeleton': Skeleton
  }
}
</script>

<style>
#v-app {
  font-family: 'Lato', Helvetica, Arial, sans-serif;
  text-align: center;
  color: #5a5a5a;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.styledHeader {
  font-family: 'Merienda', cursive;
}
</style>
