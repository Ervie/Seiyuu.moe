import Vue from 'vue'
import decode from 'decode-html'

Vue.mixin({
  methods: {
    decodeHtml (inputValue) {
      var txt = document.createElement("textarea");
      txt.innerHTML = inputValue;
      return txt.value;
    },
    swapNameSurname(inputString, delimiter = ", ") {
      var parts = inputString.split(delimiter);
      
      if (parts.length < 2) {
        return inputString
      } else {  
        return parts[1] + ' ' + parts[0];
      }
    },
    pathToImage (path) {
      if (path) {
        return path
      } else {
        return 'questionMark.png'
      }
    }
  }
})