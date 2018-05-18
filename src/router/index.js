import Vue from 'vue'
import Router from 'vue-router'
import SeiyuuArea from '@/components/seiyuu/SeiyuuArea'
import AnimeArea from '@/components/anime/AnimeArea'
import EntryPage from '@/components/EntryPage'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/Seiyuu',
      name: 'SeiyuuArea',
      component: SeiyuuArea
    },
    {
      path: '/Anime',
      name: 'AnimeArea',
      component: AnimeArea
    },
    {
      path: '/',
      name: 'EntryPage',
      component: EntryPage
    }
  ]
})
