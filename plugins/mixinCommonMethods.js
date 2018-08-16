import Vue from 'vue'
import decode from 'decode-html'

Vue.mixin({
  methods: {
    decodeHtml (inputValue) {
      var txt = document.createElement("textarea");
      txt.innerHTML = inputValue;
      return txt.value;
    },
    swapNameSurname(inputString) {
      var parts = inputString.split(", ");
      
      if (parts.length < 2) {
        return inputString
      } else {  
        return parts[1] + ' ' + parts[0];
      }
    }
  }
})