<template>
    <v-container grid-list-xl text-xs-center style="min-height: 0;">
      <v-layout row wrap fill-height>
        <v-flex xs12 xl6>
          <router-link		
              to="/seiyuu"		
              tag="span"		
              class="display-2"		
              style="cursor: pointer"		
              >	
          <v-card 
            class="linkCard">
            <v-img
              src="/kayano.jpg"
              alt="Compare Seiyuu"
              :aspect-ratio="16/9" />
            <v-card-title primary-title>
            <div>
              <h3 class="headline mb-0">Compare seiyuu</h3>
            </div>
          </v-card-title>
          </v-card>
          </router-link>
        </v-flex>
        <v-flex s12 xl6>
          <router-link		
              to="/anime"		
              tag="span"		
              class="display-2"		
              style="cursor: pointer"		
              >	
          <v-card 
            class="linkCard">
            <v-img
              src="/gochuuMosaic.jpg"
              alt="Compare Anime"
              :aspect-ratio="16/9" />
            <v-card-title primary-title>
            <div>
              <h3 class="headline mb-0">Compare anime</h3>
            </div>
          </v-card-title>
          </v-card>
          </router-link>
        </v-flex>
        <v-flex xs12>
          <router-link		
              to="/season"		
              tag="span"		
              class="display-2"		
              style="cursor: pointer"		
              >	
          <v-card 
            class="linkCard">
            <v-img
              src="/fourSeasons.jpg"
              alt="Check season summary"
              :aspect-ratio="21/9" />
            <v-card-title primary-title>
            <div>
              <h3 class="headline mb-0">Check season summary</h3>
            </div>
          </v-card-title>
          </v-card>
          </router-link>
        </v-flex>
      </v-layout>
    </v-container>
</template>

<script>

export default {
  name: 'EntryPage'
}
</script>

<style>
  .linkCard:hover .v-image {
      filter: contrast(60%);
  }

  .linkCard:hover .v-card__title {
      background-color: deepskyblue;
  }
</style>
