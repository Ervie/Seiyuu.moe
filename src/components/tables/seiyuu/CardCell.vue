<template>
<v-layout row>
  <v-flex>
    <v-card>
      <v-container fluid grid-list-lg>
        <v-layout class="primary" row v-for="(anime, j) in item.anime" v-bind:key="'j' + j">
          <v-flex xs12>
            <span>
              <p class="title text-xs-center"> {{ decodeHtml(anime.entry.name) }}</p>
            </span>
          </v-flex>
        </v-layout>
        <v-layout row v-for="(role, k) in item.roles" v-bind:key="'k' + k">
          <v-flex xs6>
            <span>
              <p class="body-2"> {{ decodeHtml(role.seiyuu) }}</p>
            </span>
          </v-flex>
          <v-flex xs6 class="accent">
            <span v-for="(character, l) in role.characters" v-bind:key="'l' + l">
              <a :href="character.entry.url">
                <p class="body-2"> {{ decodeHtml(character.entry.name) }}</p>
              </a>
            </span>
          </v-flex>
        </v-layout>
      </v-container>
    </v-card>
  </v-flex>
</v-layout>

</template>

<script>
import decode from 'decode-html'

export default {
  name: 'CardCell',
  props: ['item'],
  methods: {
    decodeHtml (inputValue) {
      inputValue = inputValue.replace('&#039;', '\'')
      return decode(inputValue)
    }
  }
}
</script>

<style>

.accent {
    color: black;
}

</style>
