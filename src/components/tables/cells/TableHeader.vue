<template>
<v-container grid-list-xs fluid>
    <v-layout row v-if="imageUrl">
        <v-flex xs6  offset-xs3 justify-center>
            <v-tooltip bottom>
                <img :src="imageUrl" slot="activator" class="av">
                <span>{{decodeHtml(text)}}</span>
            </v-tooltip>
        </v-flex>
    </v-layout>
    <v-layout v-else row>
        <v-flex md1 justify-center>
            <v-card-text class="display-1">{{decodeHtml(text)}}</v-card-text>
        </v-flex>
    </v-layout>
</v-container>
</template>

<script>
import decode from 'decode-html'

export default {
  name: 'TableHeader',
  props: ['imageUrl', 'text'],
  methods: {
    decodeHtml (inputValue) {
      inputValue = inputValue.replace('&#039;', '\'')
      return decode(inputValue)
    }
  }
}
</script>
