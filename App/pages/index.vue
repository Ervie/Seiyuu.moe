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
              src="https://danbooru.donmai.us/data/sample/sample-c045e913a0b74ce5d89593b951581727.jpg"
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
              src="https://raikou2.donmai.us/sample/49/64/__matsubara_honoka_alice_cartelet_oomiya_shinobu_ujimatsu_chiya_inokuma_youko_and_etc_gochuumon_wa_usagi_desu_ka_and_etc_drawn_by_psyche3313__sample-49640e981b8cd49cb5fb14d0ca8b839f.jpg"
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
