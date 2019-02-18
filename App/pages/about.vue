<template>
    <div>
    <v-container grid-list-xl text-xs-center style="min-height: 0;">
        <changelog-list v-if="$vuetify.breakpoint.smAndUp"/>
      <v-layout row wrap fill-height>
        <v-flex v-for="(item, i) in cardItems" v-bind:key="i" lg4 md6 xs12>
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
    <v-container hidden-sm-and-down>
      <link-footer />
    </v-container>
  </div>
</template>

<script>
import Footer from '@/components/about/Footer'
import ChangelogList from '@/components/about/ChangelogList'

    export default {
      name:"AboutPage",
      components: {
            'link-footer': Footer,
            'changelog-list': ChangelogList
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
                "26th January 2019 - new viewing mode is enabled. You can select between this and old one.",
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
            ]
        })
    }
</script>

<style scoped>

.prepend-question-icon {
    margin: 0 20px 0 0;
}


</style>
