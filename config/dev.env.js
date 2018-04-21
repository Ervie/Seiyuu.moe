'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  API_URL: '"http://seiyuuinterlinkapi.azurewebsites.net/"',
  JIKAN_URL: "'http://api.jikan.moe/'"
})
