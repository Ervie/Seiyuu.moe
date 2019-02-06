import Vue from 'vue'

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
        return path;
      } else {
        return 'questionMark.png';
      }
    },
    encodeURL (inputURL) {
      return encodeURIComponent(inputURL)
    },
    isEmpty (obj) {
        for(var prop in obj) {
            if(obj.hasOwnProperty(prop))
                return false;
        }
        return JSON.stringify(obj) === JSON.stringify({});
    },
    sleep (ms) {
      return new Promise(resolve => setTimeout(resolve, ms));
    }
  }
})