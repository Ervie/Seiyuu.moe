import Vue from 'vue'
import Router from 'vue-router'
import MainContent from '@/components/MainContent'
import About from '@/components/About'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'MainContent',
      component: MainContent
    },
    {
      path: '/About',
      name: 'About',
      component: About
    }
  ]
})
