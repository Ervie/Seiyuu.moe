<template>
<v-layout row>
  <v-flex>
    <v-card>
      <v-container fluid grid-list-lg>
        <v-layout class="primary" row v-for="(seiyuu, j) in item.seiyuu" v-bind:key="'j' + j">
          <v-flex xs12>
            <span>
              <p class="title text-xs-center"> {{ swapNameSurname(decodeHtml(seiyuu.entry.name)) }}</p>
            </span>
          </v-flex>
        </v-layout>
        <v-layout row v-for="(role, k) in item.roles" v-bind:key="'k' + k">
          <v-flex xs6>
            <span>
              <p class="body-2"> {{ role.anime }}</p>
            </span>
          </v-flex>
          <v-flex xs6 class="accent">
            <span v-for="(character, l) in role.characters" v-bind:key="'l' + l">
              <a :href="character.entry.url">
                <p class="body-2"> {{ swapNameSurname(decodeHtml(character.entry.name)) }}</p>
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
export default {
  name: 'CardCell',
  props: {
    item: {
      type: Object,
      required: true
    }
  }
}
</script>

<style>

.accent {
    color: black;
}

</style>
