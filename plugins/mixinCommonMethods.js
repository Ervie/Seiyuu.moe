import Vue from 'vue'
import decode from 'decode-html'

Vue.mixin({
  methods: {
    decodeHtml (inputValue) {
      inputValue = inputValue.replace('&#039;', '\'')
      return decode(inputValue)
    }
  }
})