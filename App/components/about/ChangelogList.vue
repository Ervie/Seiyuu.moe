<template>
    <div>
    <v-toolbar color="primary">
        <v-toolbar-title>Changelog</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn color="error" v-on:click="showOlderUpdates = true">Show older updates</v-btn>
    </v-toolbar>
    <v-list>
        <v-list-group
        >
          <v-list-tile slot="activator">
        <v-list-tile-avatar>
            <font-awesome-icon size="lg" icon="question-circle" color="cyan" />
          </v-list-tile-avatar>
            <v-list-tile-content>
              <v-list-tile-title> {{ newestItem.title }} </v-list-tile-title>
              <v-list-tile-sub-title>{{ newestItem.date }}</v-list-tile-sub-title>
            </v-list-tile-content>
          </v-list-tile>

          <v-list-tile
            v-for="subItem in newestItem.items"
            :key="subItem.title"
          >
        <v-list-tile-avatar>
            <font-awesome-icon v-if="subItem.isFeature" size="lg" icon="star" color="gold"/>
            <font-awesome-icon v-else size="lg" icon="wrench" color="silver"/>
          </v-list-tile-avatar>
            <v-list-tile-content>
              <v-list-tile-title>{{ subItem.title }}</v-list-tile-title>
            </v-list-tile-content>

            <v-list-tile-action>
              <v-icon>{{ subItem.action }}</v-icon>
            </v-list-tile-action>
          </v-list-tile>
          <v-divider inset></v-divider>
        </v-list-group>
      </v-list>
     <v-divider></v-divider>
    <v-list v-show="showOlderUpdates">
        <v-list-group
          v-for="item in items"
          :key="item.title"
          v-model="item.active"
          avatar
        >
          <v-list-tile slot="activator">
        <v-list-tile-avatar>
            <font-awesome-icon size="lg" icon="question-circle" color="cyan" />
          </v-list-tile-avatar>
            <v-list-tile-content>
              <v-list-tile-title> {{ item.title }} </v-list-tile-title>
              <v-list-tile-sub-title>{{ item.date }}</v-list-tile-sub-title>
            </v-list-tile-content>
          </v-list-tile>

          <v-list-tile
            v-for="subItem in item.items"
            :key="subItem.title"
          >
        <v-list-tile-avatar>
            <font-awesome-icon v-if="subItem.isFeature" size="lg" icon="star" color="gold"/>
            <font-awesome-icon v-else size="lg" icon="wrench" color="silver"/>
          </v-list-tile-avatar>
            <v-list-tile-content>
              <v-list-tile-title>{{ subItem.title }}</v-list-tile-title>
            </v-list-tile-content>

            <v-list-tile-action>
              <v-icon>{{ subItem.action }}</v-icon>
            </v-list-tile-action>
          </v-list-tile>
          <v-divider inset></v-divider>
        </v-list-group>
      </v-list>
    </div>
</template>

<script>
    export default {
        name: "ChangelogList",
        data() {
            return {
                showOlderUpdates: false,
                newestItem: {
                  title: 'Version 2.1 - Season summary',
                  date: '11th May 2019',
                  items: [
                      { isFeature: true, title: 'Page showng seiyuu with most roles in season.' },
                      { isFeature: true, title: 'Custom error page.' },
                      { isFeature: true, title: 'New modal loader.' },
                      { isFeature: false, title: '"No results" placeholder on autocompletion.' },
                      { isFeature: false, title: 'Search working with alternative titles.' }
                  ]},
                items: [
                  {
                  title: 'Version 2.0 - Performance improvement',
                  date: '30th March 2019',
                  items: [
                      { isFeature: true, title: 'Instead sending requests to external API, all data is requested to backend.' },
                      { isFeature: false, title: 'Reduced amount of data transferred.' },
                      { isFeature: false, title: 'Massive speed boost to searching anime/seiyuu.' },
                      { isFeature: false, title: 'Removed "Too much requests" issue altogether.' }
                  ]},
                  {
                    title: 'Version 1.5 - Performance improvement',
                    date: '11th March 2019',
                    items: [
                        { isFeature: false, title: 'Limited amount of data sent.' },
                        { isFeature: false, title: 'Improved data loading time.' },
                        { isFeature: false, title: 'Lessened "too much requests" error amount on anime search.' },
                        { isFeature: false, title: 'Fixed (for most part) image loading.' },
                    ]},
                  {
                    title: 'Version 1.4 - Timeline view',
                    date: '22th February 2019',
                    items: [
                        { isFeature: true, title: 'Three possbile table views.' },
                        { isFeature: true, title: 'Timeline view for seiyuu comparison.' },
                        { isFeature: true, title: 'New about and home pages.' },
                        { isFeature: false, title: 'Auto re-render when changing "Main roles only" option.' },
                        { isFeature: false, title: 'Removed few errors from console.' },
                        { isFeature: false, title: 'Adjusted text display.' }
                    ]},
                  {
                    title: 'Version 1.3 - Expanded view for comparing',
                    date: '26th January 2019',
                    items: [
                        { isFeature: true, title: 'New display mode for tables.' },
                        { isFeature: true, title: 'Possibility to choose between new and old display mode.' },
                        { isFeature: true, title: 'Updated seiyuu database.' },
                        { isFeature: false, title: 'Added small delay to avoid 429 (too much requests) error.' },
                        { isFeature: false, title: 'Fixed incorrect data in Seiyuu cards.' },
                        { isFeature: false, title: 'Fixed sharelinks.' },
                        { isFeature: false, title: 'Remove anime from search results if already selected.' },
                    ]},
                  {
                    title: 'Version 1.2 - Share links',
                    date: '9th January 2019',
                    items: [
                        { isFeature: true, title: 'Share links (after search.)' },
                        { isFeature: false, title: 'Fixes seiyuu photo positioning in search list.' },
                        { isFeature: false, title: 'Massive speed boost for anime searching.' },
                        { isFeature: false, title: 'Adjusted application to work under Jikan REST API v3.' }
                    ]
                  },
                  {
                      title: 'Version 1.1 - Anime search',
                      date: '15th August 2018',
                      items: [
                          { isFeature: true, title: 'Initial version.' },
                          { isFeature: false, title: 'Working again after MAL breakdown.' }
                      ]
                  }
                ]
            }
        },
        
    }
</script>

<style scoped>

</style>