<template>
    <v-layout height="auto" class="styled-footer">
      <v-card flat tile class="flex primary">
        <v-card-text class="primary white--text">
          <v-layout hidden-sm-and-down>
            <v-flex v-for="(col, i) in footerLinks" :key="i" xs4>
              <span class="subheading" v-text="col.title.toUpperCase()"></span>
              <div v-for="(child, i) in col.children" :key="i">
                <a :href="child.link" v-if="child.link" class="white--text">
                  <font-awesome-icon v-if="child.icon" size="lg" :icon="[child.iconPrefix, child.icon]"/>
                  {{ child.text }}
                </a>
                <div v-else>
                  <font-awesome-icon v-if="child.icon"  size="lg" :icon="[child.iconPrefix, child.icon]"/>
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
                iconPrefix: 'fab',
                icon: 'github',
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
                iconPrefix: '',
                icon: '',
                text: 'Jikan API',
                link: 'https://jikan.moe/'
              },
              {
                iconPrefix: '',
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
                iconPrefix: 'fa',
                icon: 'envelope',
                text: 'kontakt@bartlomiejbuchala.com',
                link: 'mailto:kontakt@bartlomiejbuchala.com'
              },
              {
                iconPrefix: 'fab',
                icon: 'discord',
                text: 'Ervie#8837',
                link: ''
              },
              {
                iconPrefix: 'fab',
                icon: 'twitter',
                text: 'ErvieBB',
                link: 'https://twitter.com/ErvieBB'
              }
            ]
          }
        ]
  })
}
</script>
