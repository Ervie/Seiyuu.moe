const pkg = require('./package')

const nodeExternals = require('webpack-node-externals')

module.exports = {
  mode: 'spa',

  /*
  ** Headers of the page
  */
  head: {
    title: 'Seiyuu.Moe - Find Japanese voice actors joint works',
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { hid: 'description', name: 'description', content: 'Did you ever wonder if those two seiyū worked together? This website allows you to select them from the list and find joint works between Japanese voice actors' },
      { name:"keywords", content:"Seiyuu, joint works, list, comparison, 2018, find"},
      { name:"author", content:"Bartłomiej Buchała"},
      { name:"theme-color", content:"#4FC3F7"},
      { property:"og:image", content:"https://image.flaticon.com/icons/svg/186/186067.svg"},
      { property:"og:site_name", content:"Seiyuu.Moe"},
      { property:"og:type", content:"list.table"},
      { property:"og:url", content:"https://seiyuu.moe"},
    ],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/mic.ico' },
      { rel: 'manifest', href: '/assets/manifest.json' },
      { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Roboto:300,400,500,700|Material+Icons' },
      { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Lato|Merienda' }
    ],
  },

  /*
  ** Customize the progress-bar color
  */
  loading: { color: '#FFFFFF' },

  /*
  ** Global CSS
  */
  css: [
    'vuetify/src/stylus/main.styl'
  ],

  /*
  ** Plugins to load before mounting the App
  */
  plugins: [
    '@/plugins/vuetify',
    '@/plugins/vuelidate',
    '@/plugins/mixinCommonMethods'
  ],

  /*
  ** Nuxt.js modules
  */
  modules: [
    // Doc: https://github.com/nuxt-community/axios-module#usage
    '@nuxtjs/axios',
    '@nuxtjs/dotenv',
    '@nuxtjs/font-awesome',
    ['@nuxtjs/google-analytics', {
      id: 'UA-114739960-4'
    }]
  ],
  /*
  ** Axios module configuration
  */
  axios: {
    // See https://github.com/nuxt-community/axios-module#options
  },

  /*
  ** Build configuration
  */
  build: {
    /*
    ** You can extend webpack config here
    */
    extend(config, ctx) {
      // Run ESLint on save
      if (ctx.isDev && ctx.isClient) {
        config.module.rules.push({
          enforce: 'pre',
          test: /\.(js|vue)$/,
          loader: 'eslint-loader',
          exclude: /(node_modules)/
        })
      }
      if (ctx.isServer) {
        config.externals = [
          nodeExternals({
            whitelist: [/^vuetify/]
          })
        ]
      }
    }
  }
}
