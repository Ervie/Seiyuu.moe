<template>
    <v-layout height="auto" class="styledFooter">
      <v-card flat tile class="flex primary">
        <v-card-text class="primary white--text">
          <v-layout hidden-sm-and-down>
            <v-flex v-for="(col, i) in footerLinks" :key="i" xs4>
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
      </v-card>
    </v-layout>
</template>

<script>
export default {
  name: 'Footer',
  data: () => ({
      footerLinks: [
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
  })
}
</script>
