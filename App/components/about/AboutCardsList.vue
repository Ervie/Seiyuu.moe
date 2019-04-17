<template>
    <v-layout row wrap fill-height>
        <v-flex v-for="(item, i) in cardItems" v-bind:key="i" lg4 sm6 xs12>
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
</template>

<script>
    export default {
        name: 'AboutCardsList',
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
                header: 'About this project',
                imageSrc: '/namikawa.png',
                imageAltText: 'Seiyuu Namikawa Daisuke during recording',
                expanded: false,
                paragraphs: [
                'Hello, I am developer from Silesia region, Poland. This website is a result of connecting my hobbies (anime and programming), non-profit project for general use.',
                'Source code can be found in link on the footer (desktop version only). Have in mind, that website is in its alpha stage and still under development. Suggestion or bugs can be issued via Github.'
                ]
            },
            {
                header: 'Special mentions',
                imageSrc: '/microBig.jpg',
                imageAltText: 'Special mentions ',
                expanded: false,
                paragraphs: [
                'Project uses Jikan API, an unofficial MyAnimeList API, courtesy of Nekomata (Irfan Dahir). Project can be found on the footer.',
                'Website icon made by Icon Monk from www.flaticon.com is licensed by CC 3.0 BY.',
                'Art used on the main page were made by haimerejzero, プシュケー and dingyiyi respectively - you can find them on pixiv.'
                ]
            }
            ]
        })
    }
</script>

<style scoped>
p, card__text {	
  text-align: justify;	
  text-justify: inter-word;	
}	
</style>