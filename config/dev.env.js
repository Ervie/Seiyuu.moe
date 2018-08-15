'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  API_URL: '"https://seiyuu.moe:5000"',
  API_URL2: '"https://seiyuuinterlinkapi.azurewebsites.net/"',
  JIKAN_URL: "'https://api.jikan.moe/'"
})
