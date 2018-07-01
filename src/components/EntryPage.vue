<template>
  <div>
    <v-container grid-list-xl text-xs-center style="min-height: 0;" >
      <v-carousel name="router-carousel" style="height: 100%" >
        <v-carousel-item v-for="(slide, i) in slides" :src="slide.imageSrc" :key="i">
          <router-link
          :to="slide.link"
          tag="span"
          class="display-2"
          style="cursor: pointer"
          >
            <v-jumbotron dark>
              <v-container fill-height>
                <v-layout align-center>
                  <v-flex>
                    <h3>
                        {{ slide.title }}
                    </h3>
                    <span class="subheading">{{ slide.text }}</span>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-jumbotron>
          </router-link>
        </v-carousel-item>
      </v-carousel>
    </v-container>
    <v-container grid-list-xl text-xs-center style="min-height: 0;" hidden-sm-and-down>
    <v-layout row wrap>
      <v-flex v-for="(item, i) in cardItems" v-bind:key="i" xs4>
        <v-card color="primary" class="white--text">
            <v-card-media
              :src="item.imageSrc"
              :alt="item.imageAltText"
              height="300px"/>
            <v-card-actions>
            <v-card-title primary-title class="display-1 styledHeader" v-html="item.header">>
            </v-card-title>
            <v-spacer></v-spacer>
            <v-btn icon @click.native="item.expanded = !item.expanded">
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
              <v-card-media :src="item.imageSrc" height="250px" :alt="item.imageAltText"></v-card-media>
              <v-card-actions>
                <v-card-title primary-title class="headline styledHeader" v-html="item.header">>
                </v-card-title>
                <v-spacer></v-spacer>
                <v-btn icon @click.native="item.expanded = !item.expanded">
                  <v-icon>{{ item.expanded ? 'keyboard_arrow_down' : 'keyboard_arrow_up' }}</v-icon>
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
  </div>
</template>

<script>
export default {
  name: 'EntryPage',
  data: () => ({
    cardItems: [
      {
        header: 'Who is a seiyuu?',
        imageSrc: '/static/rie.jpg',
        imageAltText: 'Seiyuu Kugimiya Rie during recording',
        expanded: false,
        paragraphs: [
          'Seiyuu (often also written as seiyū) is a person acting as a narrator or as an actor in radio plays or as a character actor in anime and video games. It also involves performing voice-overs for non-Japanese movies and television programs.',
          'Because Japan\'s large animation industry produces 60% of the animated series in the world, voice acting in Japan has a far greater prominence than voice acting in most other countries.'
        ]
      },
      {
        header: 'What\'s new?',
        imageSrc: '/static/namikawa.png',
        imageAltText: 'About Seiyuu.Moe project',
        expanded: false,
        paragraphs: [
          '1st July 2018 - MAL is coming back, site functionality is expected to work in one week.',
          '28th May 2018 - MAL is currently under maintenance :( The functionalities of Seiyuu.moe will be down until MAL will be back (the website will not be offline thought).',
          '20th May 2018 - added filtering by main roles.'
        ]
      },
      {
        header: 'About project',
        imageSrc: '/static/microBig.jpg',
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
        imageSrc: '/static/carousel.jpg',
        title: 'Get Started',
        text: 'Select and compare seiyuu.',
        link: '/Seiyuu'
      }
      // {
      //   imageSrc: '/static/carousel.jpg',
      //   title: 'Get Started',
      //   text: 'Select and compare anime.',
      //   link: '/Anime'
      // }
    ]
  })
}
</script>

<style>
p, card__text {
  text-align: justify;
  text-justify: inter-word;
}
</style>
