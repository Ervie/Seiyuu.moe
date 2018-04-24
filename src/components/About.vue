<template>
<div>
  <v-container grid-list-xl text-xs-center style="min-height: 0;" hidden-sm-and-down>
        <v-layout row wrap
        >
          <v-flex v-for="(item, i) in items" v-bind:key="i" xs4>
            <v-card color="primary" class="white--text">
                <v-card-media
                  :src="item.imagePath"
                  :alt="item.imageAltText"
                  height="300px"/>
                <v-card-actions>
                <v-card-title primary-title class="display-1 styledHeader" v-html="item.header">>
                </v-card-title>
                <v-spacer></v-spacer>
                <v-btn icon @click.native="item.expanded = !item.expanded">
                  <v-icon>{{ item.expanded ? 'keyboard_arrow_down' : 'keyboard_arrow_up' }}</v-icon>
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
       <v-layout row wrap
            >
              <v-flex v-for="(item, i) in items" v-bind:key="i" xs12>
                <v-card color="primary" class="white--text">
                  <v-card-media :src="item.imagePath" height="250px" :alt="item.imageAltText"></v-card-media>
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
  name: 'About',
  data: () => ({
    items: [
      {
        header: 'Who is a seiyuu?',
        imagePath: '/static/rie.jpg',
        imageAltText: 'Seiyuu Kugimiya Rie during recording',
        expanded: false,
        paragraphs: [
          'Seiyuu (often also written as seiyū) is a person acting as a narrator or as an actor in radio plays or as a character actor in anime and video games. It also involves performing voice-overs for non-Japanese movies and television programs.',
          'Because Japan\'s large animation industry produces 60% of the animated series in the world, voice acting in Japan has a far greater prominence than voice acting in most other countries.'
        ]
      },
      {
        header: 'About project',
        imagePath: '/static/lain.jpg',
        imageAltText: 'About Seiyuu.Moe project',
        expanded: false,
        paragraphs: [
          'Hello, I am developer from Silesia region, Poland. This website is a result of connecting my hobbies (anime and programming), non-profit project for general use.',
          'Source code can be found in link on the sidebar. Have in mind, that website is in its alpha stage and still under development. Suggestion or bugs can be issued via Github.'
        ]
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
</style>
