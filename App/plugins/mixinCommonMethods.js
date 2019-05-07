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
    formatDate(inputDate) {
      if (inputDate == null) {
        return 'unknown';
      } else {
      var m = new Date(inputDate);
      return m.getUTCFullYear() + "." +
          ("0" + (m.getUTCMonth()+1)).slice(-2) + "." +
          ("0" + m.getUTCDate()).slice(-2);
      }
    },
    pathToImage (path) {
      if (path && path !== 'https://cdn.myanimelist.net/images/questionmark_23.gif' 
        && path !== 'https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png') {
        return path;
      } else {
        return process.env.baseUrl +'/questionMark.png';
      }
    },
    encodeURL (inputURL) {
      return encodeURIComponent(inputURL);
    },
    isEmpty (obj) {
        for(var prop in obj) {
            if(obj.hasOwnProperty(prop))
                return false;
        }
        return JSON.stringify(obj) === JSON.stringify({});
    },
    singleObjectToArray(obj)
    {
      if (Array.isArray(obj)) {
        return obj;
      } else {
        return [ obj ];
      }
    },
    capitalizeFirstLetter(inputString) {
        return inputString.charAt(0).toUpperCase() + inputString.slice(1);
    },
    mod(n, m) {
      return ((n % m) + m) % m;
    },
    sleep (ms) {
      return new Promise(resolve => setTimeout(resolve, ms));
    }
  }
})