import Vue from 'vue'
import decode from 'decode-html'

Vue.mixin({
  methods: {
    decodeHtml (inputValue) {
        var txt = document.createElement("textarea");
        txt.innerHTML = inputValue;
        return txt.value;
    }
  }
})