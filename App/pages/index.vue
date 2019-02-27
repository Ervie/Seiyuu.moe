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
              src="https://konachan.com/sample/db2d8324e37052730c94999ae45921b7/Konachan.com%20-%20270697%20sample.jpg"
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
              src="https://konachan.com/sample/49640e981b8cd49cb5fb14d0ca8b839f/Konachan.com%20-%20239305%20sample.jpg"
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
