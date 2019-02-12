<template>
    <v-timeline>
    <v-timeline-item
      v-for="anime in animeData"
      :key="anime.mal_id"
      color="accent"
    >
      <span slot="opposite" class="title"> {{ anime.aired_date }} </span>
        <v-layout>
          
        <v-flex xs8>
        <div class="headline"> {{ anime.name }} </div>
        </v-flex>
        <v-flex xs4>
        <v-img :src="pathToImage(anime.image_url)" width="150px" height="250px"/>
        </v-flex>
        </v-layout>
    </v-timeline-item>
  </v-timeline>
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


