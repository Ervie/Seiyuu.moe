<template>
  <v-flex md6 lg3>
    <nuxt-link		
      :to="'/season/' + season.year + '/' + season.name.toLowerCase()"
      tag="span"		
      class="display-2"		
      style="cursor: pointer"		
      >	
        <v-card class="season-card">
          <v-card-title primary-title>
            <div>
              <h3 class="headline mb-0">{{ season.year }} {{ season.name}}</h3>
            </div>
          </v-card-title>
          <v-img
            :src="'/' + season.name.toLowerCase() + '.jpg'"
            aspect-ratio="0.8"
            height="300px"
          ></v-img>
        </v-card>
    </nuxt-link>
  </v-flex>
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
</style>
