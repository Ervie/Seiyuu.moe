<template>
    <nuxt-link		
      :to="'/season/' + season.year + '/' + season.name.toLowerCase()"
      tag="span"		
      class="display-2"		
      style="cursor: pointer"		
      >	
        <v-card class="season-card">
          <v-card-title class="season-card-title">
                {{ season.name}} {{ season.year }}
          </v-card-title>
          <v-img
            :src="'/' + season.name.toLowerCase() + '.jpg'"
            :height="$vuetify.breakpoint.mdAndDown ? 300 : 600"
          ></v-img>
        </v-card>
    </nuxt-link>
</template>

<script>
export default {
  name: 'SeasonCard',
  props: {
    season: {
      type: Object,
      required: true
    },
  },
}
</script>


<style>
  .season-card {
    margin: 3px;
  }

  .season-card:hover .v-image {
    filter: contrast(60%);
  }

  .season-card:hover .v-card__title {
    background-color: deepskyblue;
  }

  .season-card-title {
  justify-content: center;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
  font-weight: bold;
  position: relative;
  font-size: 18px;
  padding: 2px;
}    

</style>
