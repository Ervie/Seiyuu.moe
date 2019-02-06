import Vue from 'vue'
import { library, config } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { fas } from '@fortawesome/free-solid-svg-icons'
import { fab } from '@fortawesome/free-brands-svg-icons'

config.autoAddCss = false

library.add(fas)
library.add(fab)

Vue.component('font-awesome-icon', FontAwesomeIcon)