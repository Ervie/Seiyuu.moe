<template>
  <v-app dark>
    <v-navigation-drawer
      :mini-variant="miniVariant"
      v-model="drawer"
      app
      clipped
      fixed
    >
      <v-list>
        <v-list-tile
          router
          :to="item.to"
          :key="i"
          v-for="(item, i) in items"
        >
          <v-list-tile-action>
            <v-icon medium v-html="item.icon"></v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title v-text="item.title"></v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>
    <v-toolbar fixed app clipped-left color="primary">
      <v-toolbar-side-icon @click="drawer = !drawer"></v-toolbar-side-icon>
      <v-btn
        icon
        @click.stop="miniVariant = !miniVariant"
      >
        <v-icon v-html="miniVariant ? 'chevron_right' : 'chevron_left'"></v-icon>
      </v-btn>
      <v-toolbar-title v-text="title" class="styledHeader"></v-toolbar-title>
    </v-toolbar>
    <v-content>
      <v-container>
        <nuxt />
      </v-container>
    </v-content>
    <v-footer height="auto" app fixed class="styledFooter">
      <!-- Remove when npm Font Awesome update to 5.0.0 at least -->
      <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.0.10/css/all.css" integrity="sha384-+d0P83n9kaQMCwj8F4RJB66tzIwOKmrdb46+porD/OvrJ+37WqIM7UoBtwHO6Nlg"
        crossorigin="anonymous">
       <v-card flat tile class="flex primary" height="40px">
        <span class="primary justify-center ">
          &copy;2018 —
          <strong>Seiyuu.Moe</strong>
        </span>
      </v-card>
    </v-footer>
  </v-app>
</template>

<script>
  export default {
    data() {
      return {
        clipped: false,
        drawer: true,
        fixed: false,
        items: [
          { icon: 'fa-users', title: 'Compare Seiyuu', to: '/Seiyuu' },
          { icon: 'fa-tv', title: 'Compare Anime', to: '/Anime' },
          { icon: 'fa-info-circle', title: 'About', to: '/about' }
        ],
        miniVariant: true,
        title: 'Seiyuu.Moe',
      }
    },
    mounted: function() {
      window.dataLayer = window.dataLayer || [];  
      function gtag(){dataLayer.push(arguments);}  
      gtag('js', new Date());  

      gtag('config', 'UA-114739960-4');  
    }
  }
</script>

<style>

.styledHeader {
  font-family: 'Merienda', cursive;
  font-size: 34px;
}

.styledFooter {
  display: flex;
  text-align: center;
}

</style>
