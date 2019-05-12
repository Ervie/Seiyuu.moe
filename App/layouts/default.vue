<template>
  <v-app dark>
    <v-toolbar fixed app clipped-left color="primary">
      <v-toolbar-side-icon @click="drawer = !drawer" :aria-label="drawer ? 'Hide drawer' : 'Show drawer'"></v-toolbar-side-icon>
      <v-btn
        v-if="drawer"
        icon
        @click.stop="miniVariant = !miniVariant"
        :aria-label="miniVariant ? 'Show text' : 'Hide text'"
      >
        <v-icon v-html="miniVariant ? 'chevron_right' : 'chevron_left'"></v-icon>
      </v-btn>
      <v-toolbar-title v-text="title" class="styled-header"></v-toolbar-title>
    </v-toolbar>
    <v-content>
      <v-container>
        <nuxt />
      </v-container>
    </v-content>
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
            <font-awesome-icon size="2x" :icon="[item.iconPrefix, item.icon]"/>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title v-text="item.title"></v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>
    <v-footer height="auto" app fixed class="styled-footer" v-show="$vuetify.breakpoint.mdAndUp">
       <v-card flat tile class="flex primary" height="40px">
        <span class="primary justify-center ">
          &copy;2018 — 2019 -
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
        drawer: false,
        fixed: false,
        items: [
          { iconPrefix: 'fa', icon: 'home', title: 'Home', to: '/' },
          { iconPrefix: 'fa', icon: 'users', title: 'Compare Seiyuu', to: '/seiyuu' },
          { iconPrefix: 'fa', icon: 'tv', title: 'Compare Anime', to: '/anime' },
          { iconPrefix: 'fa', icon: 'calendar', title: 'Season Summary', to: '/season' },
          { iconPrefix: 'fa', icon: 'info-circle', title: 'About', to: '/about' }
        ],
        miniVariant: false,
        title: 'Seiyuu.Moe',
      }
    }
  }
</script>

<style>

a {
  text-decoration: none;
}

.styled-header {
  font-family: 'Merienda', Georgia, 'Times New Roman', Times, cursive, serif;
  font-size: 34px;
}

.styled-footer {
  display: flex;
  text-align: center;
}

.option-button {
  width: 100px;
}

 .dropdown-avatar {
   object-fit: cover;
   object-position: 100% 20%;
 }

.accented-text {
  font-style: italic;
  color: #B2DFDB;
}

.card-title {
  padding: 8px;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
  font-weight: bold;
  height: 36px;
  position: relative;
}    

.disabled-link {
    pointer-events:none; 
    opacity:0.6;        
 }

</style>
