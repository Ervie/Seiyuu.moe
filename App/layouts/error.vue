<template functional>
  <v-container grid-list-md>
    <v-layout row wrap align-center justify-center>
      <v-flex xs12 sm6 md4 lg3 xl2>
        <v-card>
        <v-img
        id="error-image"
          src="/404.png"
        ></v-img>
          <v-card-title class="primary white--text subheading" primary-title>
            <span class="title">Oops!</span>
          </v-card-title>
          <v-card-text>
            Looks like you're in uncharted territory. We don't know about this page yet.
            Luckily, we know the way back.
          </v-card-text>
          <v-card-actions>
            <v-btn flat nuxt large class="primary white--text" to="/">Home page</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<style>
    #error-image {
        width: 250px;
        height: 250px;
    }
</style>
