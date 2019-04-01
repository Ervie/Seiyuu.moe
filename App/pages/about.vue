<template>
    <div>
    <v-container grid-list-xl text-xs-center style="min-height: 0;">
        <changelog-list v-if="$vuetify.breakpoint.smAndUp"/>
      <about-cards-list/>
    </v-container>
    <v-container hidden-sm-and-down>
      <link-footer />
    </v-container>
  </div>
</template>

<script>
import AboutCardsList from '@/components/about/AboutCardsList'
import ChangelogList from '@/components/about/ChangelogList'
import Footer from '@/components/about/Footer'

    export default {
      name:"AboutPage",
      components: {
          'link-footer': Footer,
          'about-cards-list': AboutCardsList,
          'changelog-list': ChangelogList
      },
      head () {
        return {
          title: "Seiyuu.moe - About Seiyuu.moe",
          meta: [
            { hid: 'description', name: 'description', content: 'Quick rundown on the last features implemented and additional information about Seiyuu.moe project.' }
          ]
        }
      }

    }
</script>

<style scoped>

.prepend-question-icon {
    margin: 0 20px 0 0;
}


</style>
