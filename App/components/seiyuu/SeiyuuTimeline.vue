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
            required: false
        },
    },
    computed: {
      animeData () {
        var data = this.items.map(x => x.anime);

        data.forEach(element => {
          element.aired_date = '';
        });

        return data;
      }
    },
    methods: {
      formatDate(inputDate) {
        var m = new Date(inputDate);
        return m.getUTCFullYear() + "/" +
            ("0" + (m.getUTCMonth()+1)).slice(-2) + "/" +
            ("0" + m.getUTCDate()).slice(-2);
      }
    },
    mounted () {
      var malIds = this.animeData.map(x => x.mal_id).join('&malId=');

      this.$axios.get(process.env.API_URL + '/api/Anime/AiringDates?malId=' + malIds)
        .then((response) => {
          this.animeData.forEach(anime => {
            anime.aired_date = this.formatDate(response.data[response.data.map(x => x.mal_id).indexOf(anime.mal_id)].airing_from);
          });
        })
        .catch((error) => {
          console.log(error);
        });
    }
}
</script>

<style>
  .timeline-dot {
    width: 144px;
    height: 144px;
  }
</style>


