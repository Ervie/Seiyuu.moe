<template>
  <div>
    <v-container grid-list-xl text-xs-center style="min-height: 0;" >	
      <v-carousel name="router-carousel" style="height: 100%" >	
        <v-carousel-item v-for="(slide, i) in slides" :src="slide.imageSrc" :alt="slide.altText" :key="i">	
          <router-link	
          :to="slide.link"	
          tag="span"	
          class="display-2"	
          style="cursor: pointer"	
          >	
            <v-container fill-height>	
              <v-layout align-center>	
                <v-flex>	
                  <div class="slideTitle">	
                      {{ slide.title }}	
                  </div>	
                  <span class="subheading">{{ slide.text }}</span>	
                </v-flex>	
              </v-layout>	
            </v-container>	
          </router-link>	
        </v-carousel-item>	
      </v-carousel>	
    </v-container>
    <v-container grid-list-xl text-xs-center style="min-height: 0;" hidden-sm-and-down>
    <v-layout row wrap>
      <v-flex v-for="(item, i) in cardItems" v-bind:key="i" xs4>
        <v-card color="primary" class="white--text">
            <v-img
              :src="item.imageSrc"
              :alt="item.imageAltText"
              height="300px"/>
            <v-card-actions>
            <v-card-title primary-title class="styledHeader" v-html="item.header">>
            </v-card-title>
            <v-spacer></v-spacer>
            <v-btn icon @click.native="item.expanded = !item.expanded" :aria-label="item.expanded ? 'Hide text' : 'Show text'">
              <v-icon>{{ item.expanded ? 'keyboard_arrow_up' : 'keyboard_arrow_down' }}</v-icon>
            </v-btn>
          </v-card-actions>
          <v-slide-y-transition>
            <v-card-text v-show="item.expanded" class="subheading">
              <p v-for="(paragraph, j) in item.paragraphs" v-bind:key="j">
                  {{ paragraph }}
              </p>
          </v-card-text>
          </v-slide-y-transition>
        </v-card>
      </v-flex>
    </v-layout>
    </v-container>
    <v-container grid-list-xl text-xs-center style="min-height: 0;" hidden-md-and-up>
        <v-layout row wrap>
          <v-flex v-for="(item, i) in cardItems" v-bind:key="i" xs12>
            <v-card color="primary" class="white--text">
              <v-img :src="item.imageSrc" height="250px" :alt="item.imageAltText"></v-img>
              <v-card-actions>
                <v-card-title class="styledHeader" v-html="item.header">>
                </v-card-title>
                <v-spacer></v-spacer>
                <v-btn icon @click.native="item.expanded = !item.expanded" :aria-label="item.expanded ? 'Hide text' : 'Show text'">
                  <v-icon>{{ item.expanded ? 'keyboard_arrow_up' : 'keyboard_arrow_down' }}</v-icon>
                </v-btn>
              </v-card-actions>
              <v-card-text v-show="item.expanded" class="subheading">
                <p v-for="(paragraph, j) in item.paragraphs" v-bind:key="j">
                  {{ paragraph }}
                </p>
              </v-card-text>
            </v-card>
          </v-flex>
        </v-layout>
    </v-container>
    <v-container hidden-sm-and-down>
      <link-footer />
    </v-container>
  </div>
</template>

<script>
import Footer from '@/components/about/Footer'

export default {
  name: 'EntryPage',
  components: {
    'link-footer': Footer
  },
  data: () => ({
    cardItems: [
      {
        header: 'Who is a seiyuu?',
        imageSrc: '/rie.jpg',
        imageAltText: 'Seiyuu Kugimiya Rie during recording',
        expanded: false,
        paragraphs: [
          'Seiyuu (often also written as seiyū) is a person acting as a narrator or as an actor in radio plays or as a character actor in anime and video games. It also involves performing voice-overs for non-Japanese movies and television programs.',
          'Because Japan\'s large animation industry produces 60% of the animated series in the world, voice acting in Japan has a far greater prominence than voice acting in most other countries.'
        ]
      },
      {
        header: 'What\'s new?',
        imageSrc: '/namikawa.png',
        imageAltText: 'Seiyuu Namikawa Daisuke during recording',
        expanded: false,
        paragraphs: [
          "26th January 2018 - new viewing mode is enabled. You can select between this and old one.",
          '9th January 2018 - You can now share links of your selected combinations using "Share Link" buttons! Also searching anime should be working again.',
          '15th August 2018 - Anime search is up! Select anime, find seiyuu which collaborated on selected works.'
        ]
      },
      {
        header: 'About project',
        imageSrc: '/microBig.jpg',
        imageAltText: 'About Seiyuu.Moe project',
        expanded: false,
        paragraphs: [
          'Hello, I am developer from Silesia region, Poland. This website is a result of connecting my hobbies (anime and programming), non-profit project for general use.',
          'Source code can be found in link on the footer (desktop version only). Have in mind, that website is in its alpha stage and still under development. Suggestion or bugs can be issued via Github.'
        ]
      }
    ],
    slides: [	
      {	
        imageSrc: '/carousel.jpg',	
        title: 'Compare seiyuu',	
        text: 'Get started - select and compare seiyuu.',	
        altText: 'Select and compare seiyuu.',	
        link: '/seiyuu'	
      },	
      {	
        imageSrc: '/carousel.jpg',	
        title: 'Compare anime',	
        text: 'Get started - select and compare anime.',	
        altText: 'Select and compare anime.',	
        link: '/anime'	
      }
    ]
  })
}
</script>

<style>
p, card__text {
  text-align: justify;
  text-justify: inter-word;
}

.slideTitle {
  font-size: 50px;
}
</style>
