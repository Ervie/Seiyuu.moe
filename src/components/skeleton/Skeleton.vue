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
  <v-footer height="auto" app fixed absolute>
    <!-- Remove when npm Font Awesome update to 5.0.0 at least -->
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.0.10/css/all.css" integrity="sha384-+d0P83n9kaQMCwj8F4RJB66tzIwOKmrdb46+porD/OvrJ+37WqIM7UoBtwHO6Nlg"
      crossorigin="anonymous">
    <v-card flat tile class="flex primary">
      <v-card-text class="primary white--text">
        <v-layout hidden-sm-and-down>
          <v-flex v-for="(col, i) in rows" :key="i" xs4>
            <span class="subheading" v-text="col.title.toUpperCase()"></span>
            <div v-for="(child, i) in col.children" :key="i">
              <a :href="child.link" v-if="child.link" class="white--text">
                <v-icon small> {{ child.icon }} </v-icon>
                {{ child.text }}
              </a>
              <div v-else>
                <v-icon small> {{ child.icon }} </v-icon>
                {{ child.text }}
              </div>
            </div>
          </v-flex>
        </v-layout>
      </v-card-text>
      <span class="primary justify-center">
        &copy;2018 —
        <strong>Seiyuu.Moe</strong>
      </span>
    </v-card>
  </v-footer>
</div>
</template>

<script>

export default {
  name: 'Skeleton',
  data () {
    return {
      drawer: false,
      tilesList: [
        {icon: 'fa-users', title: 'Compare Seiyuu', link: '/Seiyuu'},
        {icon: 'fa-tv', title: 'Compare Anime', link: '/Anime'},
        {icon: 'fa-info-circle', title: 'About', link: '/'}
      ],
      rows: [
        {
          title: 'Find us also on:',
          description: 'Social Media',
          children: [
            {
              icon: 'fab fa-github',
              text: 'Github',
              link: 'https://github.com/Ervie/Seiyuu.moe'
            }
          ]
        },
        {
          title: 'Important links:',
          description: 'Related sites',
          children: [
            {
              icon: '',
              text: 'Jikan API',
              link: 'https://jikan.moe/'
            },
            {
              icon: '',
              text: 'MyAnimeList',
              link: 'https://myanimelist.net/'
            }
          ]
        },
        {
          title: 'Contact me:',
          description: 'Related sites',
          children: [
            {
              icon: 'fa-envelope',
              text: 'bbuchala93@gmail.com',
              link: 'mailto:bbuchala93@gmail.com'
            },
            {
              icon: 'fab fa-discord',
              text: 'Ervie#8837',
              link: ''
            },
            {
              icon: 'fab fa-reddit',
              text: 'Ervelan',
              link: 'https://www.reddit.com/message/compose/?to=Ervelan'
            }
          ]
        }
      ]
    }
  }
}
</script>

<style scoped></style>
