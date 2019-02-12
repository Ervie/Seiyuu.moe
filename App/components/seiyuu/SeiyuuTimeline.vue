<template>
<div>
  <v-container hidden-md-and-down>
    <v-timeline>
      <v-timeline-item
        v-for="(anime,i) in animeData"
        :key="i"
        color="accent"
      >
        <span slot="opposite" class="title"> {{ anime.aired_date }} </span>
          <v-layout align-center justify-center row fill-height v-if="i % 2 === 0">
            <v-flex xs8>
              <div class="headline"> {{ anime.name }} </div>
            </v-flex>
            <v-flex xs4>
              <v-img :src="pathToImage(anime.image_url)" width="145px" height="210px"/>
            </v-flex>
          </v-layout>
          <v-layout align-center justify-center row fill-height v-else>
            <v-flex xs4>
              <v-img :src="pathToImage(anime.image_url)" width="145px" height="210px"/>
            </v-flex>
            <v-flex xs8>
              <div class="headline"> {{ anime.name }} </div>
            </v-flex>
          </v-layout>
      </v-timeline-item>
    </v-timeline>
  </v-container>
  <v-container hidden-lg-and-up>
    <v-timeline
      align-top
      dense>
      <v-timeline-item
        v-for="(anime,i) in animeData"
        :key="i"
        color="accent"
      >
        <v-layout align-center justify-center row fill-height >
            <v-flex xs4>
        <div class="caption"> {{ anime.aired_date }} </div>
            </v-flex>
            <v-flex xs8>
        <div class="subheading"> {{ anime.name }}  </div>
            </v-flex>
          </v-layout>
      </v-timeline-item>
    </v-timeline>
  </v-container>
</div>
</template>

<script>
export default {
    name: 'SeiyuuTimeline',
    props: {
        items: {
          type: Array,
          required: false,
          default: () => {
            return [];
          }
        },
        dates: {
          type: Array,
          required: false,
          default: () => {
            return [];
          }
        }
    },
    computed: {
      animeData () {
        if (this.items != null) {
          return this.items.map(x => x.anime);
        } else {
          return [];
        }
      }
    },
    methods: {
      formatDate(inputDate) {
        var m = new Date(inputDate);
        return m.getUTCFullYear() + "." +
            ("0" + (m.getUTCMonth()+1)).slice(-2) + "." +
            ("0" + m.getUTCDate()).slice(-2);
      },
      matchDatesWithAnime() {
        var newAnimeData = this.items;

        newAnimeData.map(x => x.anime).forEach(anime => {
          anime.aired_date = this.formatDate(this.dates[this.dates.map(x => x.mal_id).indexOf(anime.mal_id)].airing_from);
        });

        // Forces timeline to re-render
        this.items = this.newAnimeData;
      }
    },
    watch: {
      dates: {
        handler: 'matchDatesWithAnime',
        immediate: false
      }
  }
}
</script>

<style>
  .timeline-dot {
    width: 144px;
    height: 144px;
  }
</style>


