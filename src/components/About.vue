<template>
  <v-container grid-list-lg text-xs-center fluid style="min-height: 0;">
    <v-expansion-panel>
      <v-expansion-panel-content>
      <div slot="header" class="headline">Who is a seiyū?</div>
            <v-layout row wrap
            hidden-sm-and-down>
              <v-flex x8>
                <v-card color="primary" class="white--text">
                  <v-container fluid grid-list-lg>
                    <v-layout row>
                      <v-flex xs8>
                        <v-card-text>
                          <div class="regular">
                            <p>Seiyuu is a person acting as a narrator or as an actor in radio plays or as a character actor in anime and video games. It also involves performing voice-overs for non-Japanese movies and television programs. Because Japan's large animation industry produces 60% of the animated series in the world, voice acting in Japan has a far greater prominence than voice acting in most other countries.</p>
                            <p>Initially, dubbing and doing voice-overs was a performance of actors who used only their voice, who were called "voice actors" (声の俳優 koe no haiyū). For convenience, the term was shortened to a new compound consisting of the first and last kanji to make seiyū (声優).</p>
                          </div>
                        </v-card-text>
                      </v-flex>
                      <v-flex xs5>
                        <v-card-media
                          src="/static/rie.jpg"
                          alt="Seiyuu Kugimiya Rie during recording"
                          height="300px"
                          contain
                        ></v-card-media>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card>
              </v-flex>
            </v-layout>
            <v-layout row wrap
            hidden-md-and-up>
              <v-flex xs12>
                <v-card color="primary" class="white--text">
                      <v-card-media src="static/rie.jpg" height="250px" alt="Seiyuu Kugimiya Rie during recording"></v-card-media>
                      <v-card-text>
                            <p>Seiyuu is a person acting as a narrator or as an actor in radio plays or as a character actor in anime and video games. It also involves performing voice-overs for non-Japanese movies and television programs. Because Japan's large animation industry produces 60% of the animated series in the world, voice acting in Japan has a far greater prominence than voice acting in most other countries.</p>
                            <p>Initially, dubbing and doing voice-overs was a performance of actors who used only their voice, who were called "voice actors" (声の俳優 koe no haiyū). For convenience, the term was shortened to a new compound consisting of the first and last kanji to make seiyū (声優).</p>
                      </v-card-text>
                </v-card>
              </v-flex>
            </v-layout>
      </v-expansion-panel-content>
    </v-expansion-panel>
  </v-container>
</template>

<script>
export default {
  name: 'About'
}
</script>
