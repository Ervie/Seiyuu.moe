<template>
  <div>
    <v-container>
      <v-flex s12>
        <router-link		
            to="/seiyuu"		
            tag="span"		
            class="display-2"		
            style="cursor: pointer"		
            >	
        <v-card 
          class="linkCard">
          <v-img
                  src="/namikawa.png"
                  alt="Compare Seiyuu"
                  height="1000px"
                  class="linkCard">
          <v-container fill-height fluid>
              <v-layout fill-height>
                <v-flex xs12 align-end flexbox>
                  <span class="headline">Compare Seiyuu</span>
                </v-flex>
              </v-layout>
            </v-container>
          </v-img>
        </v-card>
        </router-link>
      </v-flex>
      </v-container>
      <v-container>
      <v-flex s12>
        <router-link		
            to="/anime"		
            tag="span"		
            class="display-2"		
            style="cursor: pointer"		
            >	
        <v-card 
          class="linkCard">
          <v-img
                  src="/rie.jpg"
                  alt="Compare Anime"
                  height="1000px"
                  class="linkCard">
          <v-container fill-height fluid>
              <v-layout fill-height>
                <v-flex xs12 align-end flexbox>
                  <span class="headline">Compare Anime</span>
                </v-flex>
              </v-layout>
            </v-container>
          </v-img>
        </v-card>
        </router-link>
      </v-flex>
      </v-container>
  </div>
</template>

<script>

export default {
  name: 'EntryPage'
}
</script>

<style>
  .linkCard:hover {
      filter: contrast(80%);
  }
</style>
